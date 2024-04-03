using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Comunicazione
{
    internal class Richiesta
    {
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string PIN { get; set; }
        public double Amount { get; set; }
        public Operazione Operazione { get; set; }
    }
}
