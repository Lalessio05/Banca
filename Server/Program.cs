using System.Net;
using System.Net.Sockets;
namespace Server;

internal class Program
{
    static void Main(string[] args)
    {
        Server s = new(new(IPAddress.Parse("127.0.0.1"),9090));
        s.Listen();
    }
}
