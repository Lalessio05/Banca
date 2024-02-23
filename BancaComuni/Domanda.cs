using static System.Text.Encoding;
namespace BancaComuni
{
    public class Domanda(Operazioni op, string password, params string[] parametri)
    {
        public string Password => password;
        public Operazioni Operazione => op;
        public string[] Parametri => parametri;

        public override string ToString()
        {
            return $"{(int)Operazione}|{Password}|{string.Join(',',Parametri)}";
        }
        public static Domanda Parse(Span<byte> d)
        {
            string[] question = UTF8.GetString(d).Split("|");
            return new(Enum.Parse<Operazioni>(question[0]), question[1], (question.Length > 2) ? question[2].Split(",") :[string.Empty]);
        }

    }
}
