using Server.Comunicazione;

namespace Server;

class Program
{
    //TO-DO
    //Gestire caricamento e scaricamento lista transazioni
    static void Main(string[] args)
    {
        OnlineCommunication server = new TcpServer("127.0.0.1",8080);
        server.Run();
        Console.ReadLine();
    }
}
