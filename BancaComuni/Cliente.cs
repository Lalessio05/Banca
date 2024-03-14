namespace BancaComuni;

public class Cliente(string nome, string cognome, double saldoIniziale, string pin)
{
    public double Saldo { get; set; } = saldoIniziale;
    public string Nome => nome;
    public string Cognome => cognome;
    public string PIN { get; set; } = pin;
}
