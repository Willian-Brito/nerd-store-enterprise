namespace NSE.Payment.API.Models;

public enum TransactionStatus
{
    Authorized = 1,
    Paid,
    Denied,
    Refund,
    Canceled
}