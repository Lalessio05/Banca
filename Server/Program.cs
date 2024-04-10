using Server.Comunicazione;

namespace Server;

class Program
{
    static void Main(string[] args)
    {
        OnlineCommunication server = new TcpServer("127.0.0.1",9090);
        server.Run();
        Console.ReadLine();
    }
}
