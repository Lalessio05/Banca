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
        public string GetTransactionStatement(string nome, string cognome) => File.ReadAllText($"{UserTransactionPath}/{cognome}_{nome}.txt");

        public void LogTransaction(string cognome, string nome, Operazione op, double amount, string cardNumber)
        {
            File.AppendAllText($"{UserTransactionPath}/{cognome}_{nome}.txt", JsonConvert.SerializeObject(new
            {
                Nome = nome,
                Cognome = cognome,
                Operazione = op,
                Amount = amount,
                CardNumber = cardNumber
            }));
        }
        public List<Account> GetAccounts()
        {
            List<Account> accounts = [];
            try
            {
                string[] lines = File.ReadAllLines(PINPath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split(':');
                    if (parts.Length == 3 && double.TryParse(parts[2], out double balance))
                    {
                        accounts.Add(new Account(parts[1], balance, parts[0]));
                    }
                }
            }
            catch
            {
                throw new Exception("Errore nella formattazione del file utenti");
            }
            return accounts;
        }
    }
}
