namespace Server.Utilities
{
    internal class FileHandle : ILogger
    {
        private static readonly string filePath = @"C:\Users\Catri\Documents\OperazioniUtenti";

        public void LogTransaction(string cognome, string nome, Operazione op, double amount)
        {
            File.AppendAllText($"{filePath}/{cognome}_{nome}.txt", $"{op} di {amount}\n");
        }

    }
}
