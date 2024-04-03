using Newtonsoft.Json;
using System.Net;
using System.Text;
using static System.Text.Encoding;
namespace Server;
class HTTPServer
{
    private readonly HttpListener listener = new();
    private readonly Bank bank = new();
    private readonly ILogger Logger = new FileHandle();
    public HTTPServer(string prefix)
    {
        listener.Prefixes.Add(prefix);
    }
    public void Run()
    {
        listener.Start();
        Task.Run(HandleRequests);
    }

    private async Task HandleRequests()
    {
        while (true)
        {
            HttpListenerContext context = await listener.GetContextAsync();
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            byte[] buffer = new byte[4096];
            request.InputStream.Read(buffer);
            string jsonBody = UTF8.GetString(buffer);
            var richiesta = JsonConvert.DeserializeObject<Richiesta>(jsonBody);
            

            if (!bank.IsValidPin(richiesta.PIN))
            {
                RespondWithError(response, "PIN non valido.");
                continue;
            }

            switch (request.Url.AbsolutePath)
            {
                case "/prelievo":
                    HandlePOSTRequest(request.HttpMethod,richiesta, response, HandleWithdrawal);
                    break;
                case "/deposito":
                    HandlePOSTRequest(request.HttpMethod,richiesta, response, HandleDeposit);
                    break;
                case "/estratto-conto":
                    HandleStatement(request, response);
                    break;
                default:
                    RespondWithError(response, "Percorso non valido.");
                    break;
            }
        }
    }

    void HandlePOSTRequest(string metodo, Richiesta richiesta, HttpListenerResponse response, Action<HttpListenerResponse, Account, double> HandleOperation)
    {
        if (metodo != "POST")
        {
            RespondWithError(response, "Metodo non supportato.");
            return;
        }
        string requestBody;
        

        //if (!int.TryParse(request.QueryString["amount"], out int amount))
        //{
        //    RespondWithError(response, "Formato dell'importo non valido.");
        //    return;
        //}
        Account account = bank.GetAccount(richiesta.PIN);

        Logger.LogTransaction(richiesta.Nome, richiesta.Cognome, Enum.Parse<Operazione>("Prelievo"), richiesta.Amount);
        HandleOperation(response, account, richiesta.Amount);

    }
    private void HandleWithdrawal(HttpListenerResponse response, Account account, double amount)
    {
        
        if (account.Withdraw(amount))
        {
            RespondWithJson(response, amount);
        }
        else
        {
            RespondWithError(response, "Fondi insufficienti.");
        }

    }

    private void HandleDeposit(HttpListenerResponse response, Account account, double amount)
    {
        account.Deposit(amount);
        RespondWithJson(response, "Operation Succesfull");
    }

    private void HandleStatement(HttpListenerRequest request, HttpListenerResponse response)
    {
        if (request.HttpMethod == "GET")
        {
            Account account = bank.GetAccount(request.QueryString["pin"]);
            RespondWithJson(response, account.Transactions);
        }
        else
            RespondWithError(response, "Metodo non supportato.");
        
    }
    private static void RespondWithError(HttpListenerResponse response, string errorMessage)
    {
        response.StatusCode = (int)HttpStatusCode.BadRequest;
        byte[] buffer = Encoding.UTF8.GetBytes(errorMessage);
        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }

    private static void RespondWithJson(HttpListenerResponse response, object data)
    {
        string jsonData = JsonConvert.SerializeObject(data);
        byte[] buffer = Encoding.UTF8.GetBytes(jsonData);
        response.ContentType = "application/json";
        response.ContentEncoding = Encoding.UTF8;
        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }
}