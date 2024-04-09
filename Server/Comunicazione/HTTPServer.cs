//using Newtonsoft.Json;
//using Server.Utilities;
//using System.Net;
//using System.Text;
//using static Server.Utilities.Utilities;
//namespace Server.Comunicazione;
//class HTTPServer : OnlineCommunication
//{
//    private readonly HttpListener listener = new();
//    private readonly Bank bank = new();
//    private readonly ILogger Logger = new FileHandle();
//    public HTTPServer(string ip, int port)
//    {
//        listener.Prefixes.Add($"{ip}:{port}");
//    }
//    public override void Run()
//    {
//        listener.Start();
//        Task.Run(HandleRequests);
//    }

//    protected override async Task HandleRequests()
//    {
//        while (true)
//        {
//            HttpListenerContext context = await listener.GetContextAsync();
//            _ = Task.Run(() => HandleClient(context));
//        }
//    }
//    void HandleClient(HttpListenerContext context)
//    {
//        HttpListenerRequest request = context.Request;
//        HttpListenerResponse response = context.Response;

//        var richiesta = GetRequestObject(request.InputStream);

//        if (!bank.IsValidCardNumber(richiesta.PIN))
//        {
//            RespondWithError(response, "PIN non valido.");
//            return;
//        }
//        switch (request.Url?.AbsolutePath)
//        {
//            case "/prelievo":
//                HandlePOSTRequest(request.HttpMethod, richiesta, response, HandleWithdrawal);
//                break;
//            case "/deposito":
//                HandlePOSTRequest(request.HttpMethod, richiesta, response, HandleDeposit);
//                break;
//            case "/estratto-conto":
//                HandleStatement(request, richiesta, response);
//                break;
//            default:
//                RespondWithError(response, "Percorso non valido.");
//                break;
//        }
//    }

//    void HandlePOSTRequest(string metodo, Richiesta richiesta, HttpListenerResponse response, Action<HttpListenerResponse, Account, double> HandleOperation)
//    {
//        if (metodo != "POST")
//        {
//            RespondWithError(response, "Metodo non supportato.");
//            return;
//        }
//        Account account = bank.GetAccount(richiesta.PIN);

//        HandleOperation(response, account, richiesta.Amount);
//        Logger.LogTransaction(richiesta.Nome, richiesta.Cognome, Enum.Parse<Operazione>("Prelievo"), richiesta.Amount, richiesta.NumeroCarta);

//    }
//    private void HandleWithdrawal(HttpListenerResponse response, Account account, double amount)
//    {

//        if (account.Withdraw(amount))
//        {
//            RespondWithJson(response, amount);
//        }
//        else
//        {
//            RespondWithError(response, "Fondi insufficienti.");
//        }

//    }

//    private void HandleDeposit(HttpListenerResponse response, Account account, double amount)
//    {
//        account.Deposit(amount);
//        RespondWithJson(response, "Operation Succesfull");
//    }
//    private void HandleStatement(HttpListenerRequest request, Richiesta richiesta, HttpListenerResponse response)
//    {
//        if (request.HttpMethod == "GET")
//        {
//            RespondWithJson(response, Logger.GetTransactionStatement(richiesta.Nome, richiesta.Cognome));
//        }
//        else
//            RespondWithError(response, "Metodo non supportato.");

//    }
//    private static void RespondWithError(HttpListenerResponse response, string errorMessage)
//    {
//        response.StatusCode = (int)HttpStatusCode.BadRequest;
//        byte[] buffer = Encoding.UTF8.GetBytes(errorMessage);
//        response.OutputStream.Write(buffer, 0, buffer.Length);
//        response.OutputStream.Close();
//    }
//    private static void RespondWithJson(HttpListenerResponse response, object data)
//    {
//        string jsonData = JsonConvert.SerializeObject(data);
//        byte[] buffer = Encoding.UTF8.GetBytes(jsonData);
//        response.ContentType = "application/json";
//        response.ContentEncoding = Encoding.UTF8;
//        response.OutputStream.Write(buffer, 0, buffer.Length);
//        response.OutputStream.Close();
//    }
//}