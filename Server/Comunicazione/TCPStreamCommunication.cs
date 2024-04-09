using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Comunicazione
{
    internal class TCPStreamCommunication(NetworkStream stream) : IDisposable
    {
        private readonly StreamReader reader = new(stream);
        private readonly StreamWriter writer = new(stream);

        public string? GetRequestData()
        {
            return reader.ReadLine();
        }
        public void RespondWithError(string errorMessage)
        {
            writer.WriteLine($"ERROR: {errorMessage}");
            writer.Flush();
        }

        public void RespondWithJson( object data)
        {
            string jsonData = JsonConvert.SerializeObject(data);
            writer.WriteLine(jsonData);
            writer.Flush();
        }
        public void Dispose()
        {
            reader.Dispose();
            writer.Dispose();
        }
    }
}
