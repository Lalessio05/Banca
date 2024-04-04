using Server;
namespace Server;
class Bank
{
    private readonly Dictionary<string, Account> accounts = [];
    private const string pinFilePath = @"C:\Users\User\Documents\pins.txt";

    public Bank()
    {
        LoadAccounts();
    }

    private void LoadAccounts()
    {
        try
        {
            if (File.Exists(pinFilePath))
            {
                string[] lines = File.ReadAllLines(pinFilePath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split(':');
                    if (parts.Length == 3 && int.TryParse(parts[1], out int balance))
                    {
                        accounts.Add(parts[2], new Account(parts[0], balance, parts[2]));
                    }
                }
            }
        }
        catch 
        {
            throw new Exception("Errore nella formattazione del file utenti");
        }
    }

    public bool IsValidCardNumber(string cardNumber) => accounts.ContainsKey(cardNumber);
    public bool IsRightPIN(string pin, string cardNumber) => accounts.First(x=>x.Value.NumeroCarta == cardNumber).Value.Pin == pin;

    public Account GetAccount(string cardNumber)
    {
        if (IsValidCardNumber(cardNumber))
        {
            return accounts[cardNumber];
        }
        throw new Exception("Invalid card number");
    }
}