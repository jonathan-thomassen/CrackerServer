using AbstractTCPServerClassLibrary.TCPServer;
using CrackerServer;
using PasswordCrackerCentralized.model;
using PasswordCrackerCentralized.util;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text.Json;

namespace AbstractEchoTCPServer {
    public class CrackerTCPServer : AbstractTCPServer {
        private int _chunkSize = 5000;

        public CrackerTCPServer(int port, string name, int shutDownPort, string debugLevel) : base(port, name, shutDownPort, debugLevel) {

        }

        public override void TcpServerWork(StreamReader reader, StreamWriter writer) {
            var connected = true;
            while (connected) {
                string line = reader.ReadLine();
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
                    TcpClient socket = new TcpClient("localhost", 12008);

                    //Gets the stream object from the socket. The stream object is able to recieve and send data
                    NetworkStream ns = socket.GetStream();
                    //The StreamWriter is an easier way to write data to a Stream, it uses the NetworkStream
                    StreamWriter stopWriter = new StreamWriter(ns);
                    //stopWriter.WriteLine(" ");
                    //stopWriter.Flush();
                    //Thread.Sleep(10);
                    //stopWriter.WriteLine(" ");
                    //stopWriter.Flush();
                    //Thread.Sleep(10);
                    //stopWriter.WriteLine(" ");
                    //stopWriter.Flush();
                    //Thread.Sleep(10);
                    //stopWriter.WriteLine(" ");
                    //stopWriter.Flush();
                    //Thread.Sleep(10);
                    //stopWriter.WriteLine(" ");
                    //stopWriter.Flush();

                    connected = false;

                    Worker.Finish();                    
                }
            }
        }
    }
}