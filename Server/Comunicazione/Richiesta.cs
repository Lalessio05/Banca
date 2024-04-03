using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Comunicazione
{
    internal class Richiesta
    {
        public string Nome { get; set; } = string.Empty;
        public string Cognome { get; set; } = string.Empty;
        public byte[] PINBytes { get;set;} = new byte[1024];
        public string PIN => Crypt.Decrypt(PINBytes);
        public double Amount { get; set; }
        public Operazione Operazione { get; set; }
    }
}
