using Newtonsoft.Json;
using System.Text.Json.Nodes;

namespace Server.Utilities
{
    internal class FileHandle : ILogger
    {
        private static readonly string filePath = @"C:\Users\Catri\Documents\OperazioniUtenti";

        public void LogTransaction(string cognome, string nome, Operazione op, double amount, string cardNumber)
        {
            File.AppendAllText($"{filePath}/{cognome}_{nome}.txt",JsonConvert.SerializeObject(new
            {
                Nome = nome,
                Cognome = cognome,
                Operazione = op,
                Amount = amount,
                CardNumber = cardNumber
            }));
        }

    }
}
