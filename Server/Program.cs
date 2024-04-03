using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Server;

class Program
{
    static void Main(string[] args)
    {
        HTTPServer server = new("http://localhost:8080/");
        server.Run();
        Console.ReadLine(); // Keep the console application running
    }
}
