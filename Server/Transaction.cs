namespace Server
{
    //nome,cognome,op,amount,cardNumber
    internal class Transaction(string nome, string cognome, Operazione op, double amount, string cardNumber, DateTime dataOperazione)
    {
        public string Nome => nome;
        public string Cognome => cognome;
        public Operazione Operazione => op;
        public double Amount => amount;
        public string CardNumber => cardNumber;
        public DateTime DataOperazione => dataOperazione;
    }
}
