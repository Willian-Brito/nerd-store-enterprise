using Microsoft.Extensions.Options;
using NSE.Bff.Checkout.Extensions;

namespace NSE.Bff.Checkout.Services.Payment;

public class PaymentService : Service, IPaymentService
{
    private readonly HttpClient _httpClient;

    public PaymentService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(settings.Value.PaymentUrl);
    }
}