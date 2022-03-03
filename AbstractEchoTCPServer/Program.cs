// See https://aka.ms/new-console-template for more information

using AbstractEchoTCPServer;
using System.Xml;

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


CrackerTCPServer tcpServer = new CrackerTCPServer(serverPort, serverName, shutDownPort, debugLevel);