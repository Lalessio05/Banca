using Newtonsoft.Json;
using System.Net;
using System.Text;
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

            string pin = request.QueryString["pin"];

            if (!bank.IsValidPin(pin))
            {
                RespondWithError(response, "PIN non valido.");
                continue;
            }

            switch (request.Url.AbsolutePath)
            {
                case "/prelievo":
                    HandlePOSTRequest(request, response, HandleWithdrawal);
                    break;
                case "/deposito":
                    HandlePOSTRequest(request, response, HandleDeposit);
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

    void HandlePOSTRequest(HttpListenerRequest request, HttpListenerResponse response, Action<HttpListenerResponse, Account, double> HandleOperation)
    {
        if (request.HttpMethod != "POST")
        {
            RespondWithError(response, "Metodo non supportato.");
            return;
        }
        string requestBody;
        using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
        {
            requestBody = reader.ReadToEnd();
        }

        if (!int.TryParse(request.QueryString["amount"], out int amount))
        {
            RespondWithError(response, "Formato dell'importo non valido.");
            return;
        }
        Account account = bank.GetAccount(request.QueryString["pin"]);

        Logger.LogTransaction(request.QueryString["nome"], request.QueryString["cognome"], Enum.Parse<Operazione>("Prelievo"), amount);
        HandleOperation(response, account, amount);

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