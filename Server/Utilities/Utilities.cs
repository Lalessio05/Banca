using Newtonsoft.Json;
using Server.Comunicazione;
using static System.Text.Encoding;


namespace Server.Utilities
{
    internal class Utilities
    {
        public static Richiesta GetRequestObject(Stream requestStream)
        {
            byte[] buffer = new byte[4096];
            requestStream.Read(buffer);
            string jsonBody = UTF8.GetString(buffer);
            return JsonConvert.DeserializeObject<Richiesta>(jsonBody) ?? throw new Exception("Invalid request");
        }
    }
}
