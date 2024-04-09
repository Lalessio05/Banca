using Newtonsoft.Json;
using Server.Utilities;
using System.Net;
using System.Net.Sockets;

namespace Server.Comunicazione
{
    internal class TcpServer : OnlineCommunication
    {
        private readonly TcpListener listener;
        private readonly Bank bank;
        private readonly ILogger Logger;

        public TcpServer(string ipAddress, int port, string pinFilePath = @"C:\Users\User\Documents\pins.txt", string userTransactionPath = @"C:\Users\User\Documents\OperazioniUtenti")
        {
            listener = new(IPAddress.Parse(ipAddress), port);
            Logger = new FileHandle(userTransactionPath,pinFilePath);
            bank = new(Logger.GetAccounts());
        }
        public override void Run()
        {
            listener.Start();
            Task.Run(HandleRequests);
        }

        protected override async Task HandleRequests()
        {
            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                _ = Task.Run(() => HandleClient(client));
            }
        }

        private void HandleClient(TcpClient client)
        {
            using TCPStreamCommunication streamCommunication = new(client.GetStream());
            try
            {
                string requestData = streamCommunication.GetRequestData() ?? throw new Exception("Request data is missing");
                var richiesta = JsonConvert.DeserializeObject<Richiesta>(requestData) ?? throw new Exception("Can't convert object sent by client");
                if (!bank.IsValidCardNumber(richiesta.NumeroCarta))
                {
                    streamCommunication.RespondWithError("Numero di carta inesistente.");
                    return;
                }
                if (!bank.IsRightPIN(richiesta.PIN, richiesta.NumeroCarta))
                {
                    streamCommunication.RespondWithError("Pin errato.");
                    return;
                }
                switch (richiesta.Operazione)
                {
                    case Operazione.Prelievo:
                        HandleOperation(richiesta, streamCommunication, HandleWithdrawal);
                        break;
                    case Operazione.Deposito:
                        HandleOperation(richiesta, streamCommunication, HandleDeposit);
                        break;
                    case Operazione.Estratto_Conto:
                        HandleStatement(richiesta, streamCommunication);
                        break;
                    default:
                        streamCommunication.RespondWithError("Percorso non valido.");
                        break;
                }
            }
            catch (Exception ex)
            {
                streamCommunication.RespondWithError(ex.Message);
            }

            client.Close();
        }

        private void HandleOperation(Richiesta richiesta, TCPStreamCommunication streamCommunication, Action<Account, double> operation)
        {
            Account account = bank.GetAccount(richiesta.NumeroCarta);

            operation(account, richiesta.Amount);
            Logger.LogTransaction(richiesta.Nome, richiesta.Cognome, richiesta.Operazione, richiesta.Amount, richiesta.NumeroCarta);

            streamCommunication.RespondWithJson("Operation Succesfull");
        }

        private void HandleWithdrawal(Account account, double amount)
        {
            bank.Withdraw(account, amount);
        }

        private void HandleDeposit(Account account, double amount)
        {
            bank.Deposit(account, amount);
        }

        private void HandleStatement(Richiesta richiesta, TCPStreamCommunication streamCommunication)
        {
            streamCommunication.RespondWithJson(Logger.GetTransactionStatement(richiesta.Nome, richiesta.Cognome));
        }
    }
}
