using AbstractTCPServerClassLibrary.TCPServer;
using PasswordCrackerCentralized.model;
using PasswordCrackerCentralized.util;
using System.Collections.Concurrent;

namespace AbstractEchoTCPServer {
    public class CrackerTCPServer : AbstractTCPServer {
        private int _chunkSize = 5000;
        private int _nextClientStartpoint = 0;

        public CrackerTCPServer(int port, string name, int shutDownPort, string debugLevel) : base(port, name, shutDownPort, debugLevel) {

        }

        public override void TcpServerWork(StreamReader reader, StreamWriter writer) {
            BlockingCollection<List<string>> chunks = new BlockingCollection<List<string>>();
            List<UserInfo> users = new List<UserInfo>();
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

            users = PasswordFileHandler.ReadPasswordFile("passwords.txt");

            string line = reader.ReadLine();

            switch(line.ToLower()) {
                case "passwords":
                    break;
                case "nextchunk":
                    break;
                case "finished":
                    break;
                default:
                    break;
            }

            writer.Flush();
        }
    }
}