using Newtonsoft.Json;
namespace Server;
class Account(string pin, int startingBalance, string numeroCarta)
{
    public string Pin => pin;
    public string NumeroCarta => numeroCarta;
    public double Balance { get; set; } = startingBalance;

    private readonly List<string> _transactions = [];
    public string Transactions => JsonConvert.SerializeObject(_transactions);

    public void Deposit(double amount)
    {
        Balance += amount;
    }

    public bool Withdraw(double amount)
    {
        if (Balance >= amount)
        {
            Balance -= amount;
            return true;
        }
        return false;
    }
}