namespace NSE.Payment.NerdPay;

public class NerdPayService
{
    public readonly string ApiKey;
    public readonly string EncryptionKey;

    public NerdPayService(string apiKey, string encryptionKey)
    {
        ApiKey = apiKey;
        EncryptionKey = encryptionKey;
    }
}