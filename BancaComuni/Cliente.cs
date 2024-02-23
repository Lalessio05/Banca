namespace BancaComuni;

public class Cliente(string nome, string cognome, double saldoIniziale, string password)
{
    public double Saldo { get; set; } = saldoIniziale;
    public string Nome => nome;
    public string Cognome => cognome;
    public string Password { get; set; } = password;
}
