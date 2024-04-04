using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Utilities
{
    internal interface ILogger
    {
        void LogTransaction(string nome, string cognome, Operazione op, double amount, string cardNumber);
    }
}
