﻿using Server.Utilities;
using System.Transactions;

namespace Server;
class Bank(List<Account> accounts)
{
    public readonly List<Account> accounts = accounts;
    public bool IsValidCardNumber(string cardNumber) => accounts.Any(x=>x.NumeroCarta == cardNumber);
    public bool IsRightPIN(string pin, string cardNumber) => accounts.First(x => x.NumeroCarta == cardNumber).Pin == pin;
    public Account GetAccount(string cardNumber)
    {
        if (IsValidCardNumber(cardNumber))
        {
            return accounts.First(x=>x.NumeroCarta == cardNumber);
        }
        throw new Exception("Invalid card number");
    }
    public void Withdraw(Account account, double amount)
    {
        if (account.Balance <= amount)
            throw new Exception("Saldo insufficiente sul conto");
        else if (amount < 0)
            throw new Exception("Impossibile prelevare somma negativa");
        account.Balance -= amount;

    }

    public void Deposit(Account account, double amount)
    {
        if (amount < 0)
            throw new Exception("Impossibile depositare una somma negativa");
        account.Balance += amount;       
    }
    public bool CheckWithdrawable(double amount, List<Transaction> lastDayTrasanction)
    {
        lastDayTrasanction = lastDayTrasanction.Where(x => x.Operazione == Operazione.Prelievo).ToList();
        return lastDayTrasanction.Aggregate((double)0, (x, y) => x += y.Amount)+amount < 500;
    }
}
