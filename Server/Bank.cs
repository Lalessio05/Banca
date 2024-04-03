using Server;
namespace Server;
class Bank
{
    private readonly Dictionary<string, Account> accounts = new();
    private const string pinFilePath = @"C:\Users\User\Documents\pins.txt";

    public Bank()
    {
        LoadAccounts();
    }

    private void LoadAccounts()
    {
        if (File.Exists(pinFilePath))
        {
            string[] lines = File.ReadAllLines(pinFilePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                if (parts.Length == 2 && int.TryParse(parts[1], out int balance))
                {
                    accounts.Add(parts[0], new Account(parts[0], balance));
                }
            }
        }
    }

    public bool IsValidPin(string pin)
    {
        return accounts.ContainsKey(pin);
    }

    public Account GetAccount(string pin)
    {
        if (IsValidPin(pin))
        {
            return accounts[pin];
        }
        return null;
    }
}