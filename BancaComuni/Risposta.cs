using static System.Text.Encoding;
namespace BancaComuni;

public class Risposta(bool esitoPositivo, string messaggio)
{
    public bool EsitoPositivo => esitoPositivo;
    public string Messaggio => messaggio;
    public static Risposta Parse(Span<byte> item)
    {
        string[] response = UTF8.GetString(item).Split("|");
        return new(bool.Parse(response[0]), response[1]);
    }
    public override string ToString()
    {
        return $"{EsitoPositivo}|{Messaggio}";
    }
}
