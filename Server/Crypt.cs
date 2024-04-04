using System.Security.Cryptography;
using System.Text;

namespace Server;

public class Crypt
{
    static private RSACryptoServiceProvider rsa;
    private static readonly string publicKeyFile = @"C:\Users\User\Documents\publicKey.xml";
    private static readonly string privateKeyFile = @"C:\Users\User\Documents\privateKey.xml";

    static Crypt()
    {
        rsa = new RSACryptoServiceProvider();
        if (File.Exists(publicKeyFile) && File.Exists(privateKeyFile))
        {
            LoadKeys();
        }
        else
        {
            throw new FileNotFoundException("I file delle chiavi non esistono.");
        }
    }

    private static void LoadKeys()
    {
        string publicKey = File.ReadAllText(publicKeyFile);
        string privateKey = File.ReadAllText(privateKeyFile);

        rsa.FromXmlString(privateKey);
    }

    // Metodo per codificare un messaggio
    static public byte[] Encrypt(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        return rsa.Encrypt(data, false);
    }

    // Metodo per decodificare un messaggio
    static public string Decrypt(byte[] encryptedData)
    {
        byte[] decryptedData = rsa.Decrypt(encryptedData, false);
        return Encoding.UTF8.GetString(decryptedData);
    }
}