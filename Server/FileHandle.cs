using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class FileHandle : ILogger
    {
        private static readonly string filePath = $@"./OperazioniUtenti/";

        public void LogTransaction(string cognome, string nome,Operazione op, double amount)
        {
            File.AppendAllText($"{filePath}/{cognome}_{nome}.txt",$"{op} di {amount}");
        }

    }
}
