using System.Security.Cryptography;
using System.Text;

namespace Server;

public class Crypt
{
    static private RSACryptoServiceProvider rsa;
    private static readonly string publicKeyFilePath = @"C:\Users\User\Documents\publicKey.xml";
    private static readonly string privateKeyFilePath = @"C:\Users\User\Documents\privateKey.xml";

    static Crypt()
    {
        rsa = new RSACryptoServiceProvider();
        if (!File.Exists(publicKeyFilePath) && File.Exists(privateKeyFilePath))
            throw new FileNotFoundException("I file delle chiavi non esistono.");
    }

    

    // Metodo per codificare un messaggio
    static public byte[] Encrypt(string message)
    {
        rsa.FromXmlString(File.ReadAllText(publicKeyFilePath));
        byte[] data = Encoding.UTF8.GetBytes(message);
        return rsa.Encrypt(data, false);
    }

    // Metodo per decodificare un messaggio
    static public string Decrypt(byte[] encryptedData)
    {
        rsa.FromXmlString(File.ReadAllText(privateKeyFilePath));
        byte[] decryptedData = rsa.Decrypt(encryptedData, false);
        return Encoding.UTF8.GetString(decryptedData);
    }
}