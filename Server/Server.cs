using BancaComuni;
using System.Net.Sockets;
using static System.Text.Encoding;
namespace Server;

internal class Server
{
    List<Cliente> Clienti = [new("Mattia", "Gambato", 300, "1234")];

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
                        stream.Write(ManageDeposito(d));
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
        if (int.Parse(d.Parametri[0]) <= Clienti.First(x => x.PIN == d.PIN).Saldo)
        {
            Clienti.First(x => x.PIN == d.PIN).Saldo -= double.Parse(d.Parametri[0]);
            return UTF8.GetBytes(new Risposta(esitoPositivo: true, messaggio: $"T'apposto, ecco i tuoi {d.Parametri[0]} euri.").ToString() ?? string.Empty);
        }
        else
            return UTF8.GetBytes(new Risposta(esitoPositivo: false, messaggio: $"Hai soltanto {Clienti.First(x => x.PIN == d.PIN).Saldo} euro sul tuo conto. Prelievo fallito.").ToString() ?? string.Empty);
    }

    Span<byte> ManageDeposito(Domanda d)
    {
        Clienti.First(x => x.PIN == d.PIN).Saldo += double.Parse(d.Parametri[0]);
        return UTF8.GetBytes(new Risposta(esitoPositivo: true, messaggio: $"Deposito eseguito correttamente, richiedere estratto conto per vedere il saldo aggiornato.").ToString());
    }
}
