using AbstractTCPServerClassLibrary.TCPServer;
using PasswordCrackerCentralized.model;
using PasswordCrackerCentralized.util;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;

namespace AbstractEchoTCPServer {
    public class CrackerTCPServer : AbstractTCPServer {
        private int _chunkSize = 5000;

        public CrackerTCPServer(int port, string name, int shutDownPort, string debugLevel) : base(port, name, shutDownPort, debugLevel) {

        }

        public override void TcpServerWork(StreamReader reader, StreamWriter writer) {
            Stopwatch stopwatch = Stopwatch.StartNew();

            BlockingCollection<List<string>> chunks = new BlockingCollection<List<string>>();
            List<UserInfo> uncracked = new List<UserInfo>();
            List<UserInfoClearText> results = new List<UserInfoClearText>();

            

            using (FileStream fs = new FileStream("webster-dictionary.txt", FileMode.Open, FileAccess.Read))

            using (StreamReader dictionary = new StreamReader(fs)) {
                var n = 0;
                while (!dictionary.EndOfStream) {
                    if (n % _chunkSize == 0) {
                        chunks.Add(new List<string>());
                    }
                    string dictionaryEntry = dictionary.ReadLine();
                    chunks.Last().Add(dictionaryEntry);
                    n++;
                }
            }

            int chunksProcessed = 0;
            int chunkAmount = chunks.Count;

            uncracked = PasswordFileHandler.ReadPasswordFile("passwords.txt");

            var running = true;
            while (running) {
                string line = reader.ReadLine();
                switch (line.ToLower()) {
                    case "passwords":                        
                        writer.WriteLine(JsonSerializer.Serialize(uncracked));
                        break;
                    case "nextchunk":
                        writer.WriteLine(JsonSerializer.Serialize(chunks.Take()));
                        break;
                    case "finished":
                        var n = int.Parse(reader.ReadLine());
                        for (int i = 0; i < n; i++) {
                            UserInfoClearText newlyCracked = JsonSerializer.Deserialize<UserInfoClearText>(reader.ReadLine());
                            uncracked.Remove(uncracked.Find(ui => ui.Username == newlyCracked.UserName));
                            results.Add(newlyCracked);
                        }
                        chunksProcessed++;
                        break;
                    default:
                        break;
                }
                writer.Flush();

                if (uncracked.Count == 0 || chunksProcessed == chunkAmount) {
                    running = false;
                    stopwatch.Stop();
                }
            }

            foreach (UserInfoClearText cracked in results) {
                Console.WriteLine(cracked);
            }
            Console.WriteLine("Passwords cracked: " + results.Count);
            Console.WriteLine("Time elapsed: " + stopwatch.Elapsed);
        }
    }
}