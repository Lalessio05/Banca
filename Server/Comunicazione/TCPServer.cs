using Newtonsoft.Json;
using Server.Utilities;
using System.Net;
using System.Net.Sockets;

namespace Server.Comunicazione
{
    public class TcpServer(string ipAddress, int port) : IOnlineCommunication
    {
        private readonly TcpListener listener = new(IPAddress.Parse(ipAddress), port);
        private readonly Bank bank = new();
        private readonly ILogger logger = new FileHandle();

        public void Run()
        {
            listener.Start();
            Task.Run(HandleRequests);
        }

        private async Task HandleRequests()
        {
            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                HandleClient(client);
            }
        }

        private void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            using StreamReader reader = new(stream);
            using StreamWriter writer = new(stream);
            {
                try
                {
                    string requestData = reader.ReadLine();
                    var richiesta = JsonConvert.DeserializeObject<Richiesta>(requestData);

                    if (!bank.IsValidPin(richiesta.PIN))
                    {
                        RespondWithError(writer, "PIN non valido.");
                        return;
                    }

                    switch (richiesta.Operazione)
                    {
                        case Operazione.Prelievo:
                            HandleOperation(richiesta, writer, HandleWithdrawal);
                            break;
                        case Operazione.Deposito:
                            HandleOperation(richiesta, writer, HandleDeposit);
                            break;
                        case Operazione.Estratto_Conto:
                            HandleStatement(richiesta, writer);
                            break;
                        default:
                            RespondWithError(writer, "Percorso non valido.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    RespondWithError(writer, ex.Message);
                }
            }

            client.Close();
        }

        private void HandleOperation(Richiesta richiesta, StreamWriter writer, Action<Account, double> operation)
        {
            Account account = bank.GetAccount(richiesta.PIN);

            operation(account, richiesta.Amount);
            logger.LogTransaction(richiesta.Nome, richiesta.Cognome, richiesta.Operazione, richiesta.Amount);

            RespondWithJson(writer, "Operation Succesfull");
        }

        private void HandleWithdrawal(Account account, double amount)
        {
            if (!account.Withdraw(amount))
            {
                throw new Exception("Fondi insufficienti.");
            }
        }

        private void HandleDeposit(Account account, double amount)
        {
            account.Deposit(amount);
        }

        private void HandleStatement(Richiesta richiesta, StreamWriter writer)
        {
            Account account = bank.GetAccount(richiesta.PIN);
            RespondWithJson(writer, account.Transactions);
        }


        private static void RespondWithError(StreamWriter writer, string errorMessage)
        {
            writer.WriteLine($"ERROR: {errorMessage}");
        }

        private static void RespondWithJson(StreamWriter writer, object data)
        {
            string jsonData = JsonConvert.SerializeObject(data);
            writer.WriteLine(jsonData);
        }
    }
}
