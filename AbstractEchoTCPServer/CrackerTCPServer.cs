using AbstractTCPServerClassLibrary.TCPServer;
using System.Collections.Concurrent;

namespace AbstractEchoTCPServer {
    public class CrackerTCPServer : AbstractTCPServer {
        private int _chunkSize = 100000;
        private int _nextClientStartpoint = 0;

        public CrackerTCPServer(int port, string name, int shutDownPort, string debugLevel) : base(port, name, shutDownPort, debugLevel) {

        }

        public override void TcpServerWork(StreamReader reader, StreamWriter writer) {
            List<string> fullDictionary = new List<string>();

            using (FileStream fs = new FileStream("webster-dictionary.txt", FileMode.Open, FileAccess.Read))

            using (StreamReader dictionary = new StreamReader(fs)) {
                while (!dictionary.EndOfStream) {
                    string dictionaryEntry = dictionary.ReadLine();                    
                    fullDictionary.Add(dictionaryEntry);                    
                }
            }           



            string line = reader.ReadLine();
            reader.re

            writer.WriteAsync()
            writer.WriteLine(line);
            _nextClientStartpoint += _chunkSize;
            writer.Flush();
        }
    }
}