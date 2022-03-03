using AbstractTCPServerClassLibrary.TCPServer;
using System.Collections.Concurrent;

namespace AbstractEchoTCPServer {
    public class CrackerTCPServer : AbstractTCPServer {
        private int _chunkSize = 5000;
        private int _nextClientStartpoint = 0;

        public CrackerTCPServer(int port, string name, int shutDownPort, string debugLevel) : base(port, name, shutDownPort, debugLevel) {

        }

        public override void TcpServerWork(StreamReader reader, StreamWriter writer) {
            List<string> fullDictionary = new List<string>();
            BlockingCollection<List<string>> chunks = new BlockingCollection<List<string>>();

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


            string line = reader.ReadLine();
            //reader.re

            //writer.WriteAsync()
            writer.WriteLine(line);
            _nextClientStartpoint += _chunkSize;
            writer.Flush();
        }
    }
}