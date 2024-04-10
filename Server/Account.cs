namespace Server;
class Account(string pin, double startingBalance, string numeroCarta)
{
    public string Pin => pin;
    public string NumeroCarta => numeroCarta;
    public double Balance { get; set; } = startingBalance;
}