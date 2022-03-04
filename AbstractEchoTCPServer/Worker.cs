using PasswordCrackerCentralized.model;
using PasswordCrackerCentralized.util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace CrackerServer {
    public static class Worker {

        public static List<UserInfo> Uncracked;
        public static List<UserInfoClearText> Results;
        public static BlockingCollection<List<string>> Chunks;
        public static int ChunksProcessed;
        public static int ChunkAmount;
        public static Stopwatch Stopwatch;
        public static bool FirstClient;


        private static int _chunkSize = 5000;
        static Worker() {

        }

        public static void DoWork() {
            string confPath = Environment.GetEnvironmentVariable("AbstractServerConf");

            XmlDocument configDoc = new XmlDocument();
            configDoc.Load(confPath + "\\CrackerServerConfig.xml");

            int serverPort = 7007;
            XmlNode serverPortNode = configDoc.DocumentElement.SelectSingleNode("ServerPort");
            if (serverPortNode != null) {
                string serverPortStr = serverPortNode.InnerText.Trim();
                serverPort = Convert.ToInt32(serverPortStr);
            }

            string serverName = "EchoTCPServer";
            XmlNode serverNameNode = configDoc.DocumentElement.SelectSingleNode("ServerName");
            if (serverNameNode != null) {
                serverName = serverNameNode.InnerText.Trim();
            }

            int shutDownPort = 7008;
            XmlNode shutDownPortNode = configDoc.DocumentElement.SelectSingleNode("ShutDownPort");
            if (shutDownPortNode != null) {
                string shutDownPortStr = shutDownPortNode.InnerText.Trim();
                shutDownPort = Convert.ToInt32(shutDownPortStr);
            }

            string debugLevel = "All";
            XmlNode debugLevelNode = configDoc.DocumentElement.SelectSingleNode("DebugLevel");
            if (debugLevelNode != null) {
                debugLevel = debugLevelNode.InnerText.Trim();
            }

            Chunks = new BlockingCollection<List<string>>();
            Uncracked = new List<UserInfo>();
            Results = new List<UserInfoClearText>();

            using (FileStream fs = new FileStream("webster-dictionary.txt", FileMode.Open, FileAccess.Read))

            using (StreamReader dictionary = new StreamReader(fs)) {
                var n = 0;
                while (!dictionary.EndOfStream) {
                    if (n % _chunkSize == 0) {
                        Chunks.Add(new List<string>());
                    }
                    string dictionaryEntry = dictionary.ReadLine();
                    Chunks.Last().Add(dictionaryEntry);
                    n++;
                }
            }

            ChunksProcessed = 0;
            ChunkAmount = Chunks.Count;

            Uncracked = PasswordFileHandler.ReadPasswordFile("passwords.txt");
            FirstClient = true;

            CrackerTCPServer tcpServer = new CrackerTCPServer(serverPort, serverName, shutDownPort, debugLevel);
        }

        public static void Finish() {
            Stopwatch.Stop();
            foreach (UserInfoClearText cracked in Results) {
                Console.WriteLine(cracked);
            }
            Console.WriteLine("Passwords cracked: " + Results.Count);
            Console.WriteLine("Time elapsed: " + Stopwatch.Elapsed);

            Environment.Exit(0);
        }
    }
}