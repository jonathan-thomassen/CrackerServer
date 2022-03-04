using AbstractTCPServerClassLibrary.TCPServer;
using CrackerServer;
using PasswordCrackerCentralized.model;
using PasswordCrackerCentralized.util;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text.Json;

namespace CrackerServer {
    public class CrackerTCPServer : AbstractTCPServer {
        private int _chunkSize = 5000;

        public CrackerTCPServer(int port, string name, int shutDownPort, string debugLevel) : base(port, name, shutDownPort, debugLevel) {

        }

        public override void TcpServerWork(StreamReader reader, StreamWriter writer) {
            while (true) {
                string line = reader.ReadLine();
                if (Worker.FirstClient == true) {
                    Worker.Stopwatch = Stopwatch.StartNew();
                    Worker.FirstClient = false;
                }
                switch (line.ToLower()) {
                    case "passwords":
                        writer.WriteLine(JsonSerializer.Serialize(Worker.Uncracked));
                        break;
                    case "nextchunk":
                        writer.WriteLine(JsonSerializer.Serialize(Worker.Chunks.Take()));
                        break;
                    case "finished":
                        var n = int.Parse(reader.ReadLine());
                        for (int i = 0; i < n; i++) {
                            UserInfoClearText newlyCracked = JsonSerializer.Deserialize<UserInfoClearText>(reader.ReadLine());
                            Worker.Uncracked.Remove(Worker.Uncracked.Find(ui => ui.Username == newlyCracked.UserName));
                            Worker.Results.Add(newlyCracked);
                        }
                        Worker.ChunksProcessed++;
                        break;
                    default:
                        break;
                }
                writer.Flush();

                if (Worker.Uncracked.Count == 0 || Worker.ChunksProcessed == Worker.ChunkAmount) {
                    Worker.Finish();                    
                }
            }
        }
    }
}