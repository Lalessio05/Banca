using Newtonsoft.Json;

namespace Server.Utilities
{
    internal class FileHandle : ILogger
    {
        private readonly string UserTransactionPath = string.Empty;
        private readonly string PINPath = string.Empty;

        public FileHandle(string userTransactionPath, string pinPath)
        {
            if (!Directory.Exists(userTransactionPath))
                Directory.CreateDirectory(userTransactionPath);
            UserTransactionPath = userTransactionPath;
            if (!File.Exists(pinPath))
                throw new Exception("Il file contenente i dati delle carte non è stato trovato");
            PINPath = pinPath;
        }
        public void EditBalance(List<Account> accounts)
        {
            File.WriteAllText(PINPath, JsonConvert.SerializeObject(accounts, Formatting.Indented));
        }
        public string GetTransactionStatement(string nome, string cognome) => File.ReadAllText($"{UserTransactionPath}/{nome}_{cognome}.txt");

        public void LogTransaction(string nome, string cognome, Operazione op, double amount, string cardNumber, DateTime dataOperazione)
        {
            File.AppendAllText($"{UserTransactionPath}/{nome}_{cognome}.txt", 
                JsonConvert.SerializeObject(new Transaction(nome,cognome,op,amount,cardNumber,dataOperazione))
            );
        }
        public List<Account> GetAccounts()
        {
            try
            {
                return JsonConvert.DeserializeObject<List<Account>>(File.ReadAllText(PINPath)) ?? throw new Exception("File utenti vuoto");
            }
            catch
            {
                throw new Exception("Errore nella formattazione del file utenti");
            }
        }

        public IEnumerable<Transaction> GetLastDayTransactions(string nome, string cognome)
        {
            if (!File.Exists($"{UserTransactionPath}/{nome}_{cognome}.txt"))
                File.Create($"{UserTransactionPath}/{nome}_{cognome}.txt").Close();
            foreach (var riga in File.ReadAllLines($"{UserTransactionPath}/{nome}_{cognome}.txt"))
            {
                yield return JsonConvert.DeserializeObject<Transaction>(riga) ?? throw new Exception("Errore nella deserializzazione della riga");
            }
            
        }
    }
}
