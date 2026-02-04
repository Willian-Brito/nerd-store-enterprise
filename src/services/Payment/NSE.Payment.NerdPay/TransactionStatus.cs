namespace NSE.Payment.NerdPay;

public enum TransactionStatus
{
    Authorized = 1,
    Paid,
    Refused,
    Chargeback,
    Cancelled
}