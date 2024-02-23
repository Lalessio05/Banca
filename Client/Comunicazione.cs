using System.Net.Sockets;
using static System.Text.Encoding;
using BancaComuni;
namespace Client;

internal class Comunicazione : IDisposable
{
    static public TcpClient Client { get; set; } = new TcpClient("localhost", 9090);
    static public NetworkStream Stream { get; set; } = Client.GetStream();

    public static void EseguiOperazione(Operazioni operazione, Cliente c, params string[] parametriOperazione)
    {
        Stream.Write(UTF8.GetBytes($"{(int)operazione}|{c.Password}|{string.Join(',',parametriOperazione)}"));
        Span<byte> Buffer = new byte[2048];

        _ = Stream.Read(Buffer);
        var risposta = Risposta.Parse(Buffer);
    }

    public void Dispose()
    {
        Client.Dispose();
    }
}
