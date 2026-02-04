using System.Security.Cryptography;
using System.Text;

namespace NSE.Payment.NerdPay;

public class CardHash
{
    public string CardHolderName { get; set; }
    public string CardNumber { get; set; }
    public string CardExpirationDate { get; set; }
    public string CardCvv { get; set; }
    private readonly NerdPayService _nerdPayService;

    public CardHash(NerdPayService nerdPayService)
    {
        _nerdPayService = nerdPayService;
    }

    public string Generate()
    {
        using var aesAlg = Aes.Create();

        aesAlg.IV = Encoding.Default.GetBytes(_nerdPayService.EncryptionKey);
        aesAlg.Key = Encoding.Default.GetBytes(_nerdPayService.ApiKey);

        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(CardHolderName + CardNumber + CardExpirationDate + CardCvv);
        }
        var cardHash = Encoding.ASCII.GetString(msEncrypt.ToArray());
        return cardHash;
    }
}