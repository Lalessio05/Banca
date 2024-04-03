using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Server.Comunicazione;

namespace Server;

class Program
{
    static void Main(string[] args)
    {
        IOnlineCommunication server = new TcpServer("127.0.0.1",8080);
        server.Run();
        Console.ReadLine(); // Keep the console application running
    }
}
