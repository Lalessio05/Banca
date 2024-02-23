using BancaComuni;
using System.Net.Sockets;
using static System.Text.Encoding;
namespace Server;

internal class Server
{
    List<Cliente> Clienti = [new("Mattia", "Gambato", 300, "batanga")];

    TcpListener Listener { get; }
    public Server(TcpListener listener)
    {
        this.Listener = listener;
        Listener.Start();
    }
    public void Listen()
    {
        while (true)
        {
            var client = Listener.AcceptTcpClient();
            Task.Run(() =>
            {
                Span<byte> Buffer = new byte[2048];
                var stream = client.GetStream();
                stream.Read(Buffer);
                Domanda d = Domanda.Parse(Buffer);
                switch (d.Operazione)
                {
                    case Operazioni.Prelievo:
                        stream.Write(ManagePrelievo(d));
                        break;
                    case Operazioni.Deposito:
                        //Deposito();
                        break;
                    case Operazioni.Estratto_Conto:
                        //Estratto();
                        break;
                }
            });
        }
    }
    Span<byte> ManagePrelievo(Domanda d)
    {
        if (int.Parse(d.Parametri[0]) <= Clienti.First(x => x.Password == d.Password).Saldo)
            return UTF8.GetBytes(new Risposta(esitoPositivo: true, messaggio: $"T'apposto, ecco i tuoi {d.Parametri[0]} euri").ToString() ?? string.Empty);
        else
            return UTF8.GetBytes(new Risposta(esitoPositivo: false, messaggio: $"Hai soltanto {Clienti.First(x => x.Password == d.Password).Saldo} euro sul tuo conto. Prelievo fallito").ToString() ?? string.Empty);
    }
}
