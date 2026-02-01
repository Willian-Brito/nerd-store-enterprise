using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using NSE.Core.Communication;

namespace NSE.Bff.Checkout.Services;

public abstract class Service
{
    protected StringContent GetContent(object dado)
    {
        return new StringContent(
            JsonSerializer.Serialize(dado),
            Encoding.UTF8,
            "application/json"
        );
    }

    protected async Task<T> DeserializeResponse<T>(HttpResponseMessage responseMessage)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        var content = await responseMessage.Content.ReadAsStringAsync();
        Console.WriteLine($"Content: {content}");

        return JsonSerializer.Deserialize<T>(content, options);
    }

    protected bool ManageHttpResponse(HttpResponseMessage Response)
    {
        if (Response.StatusCode == HttpStatusCode.BadRequest) return false;

        Response.EnsureSuccessStatusCode();
        return true;
    }

    protected ResponseResult Ok()
    {
        return new ResponseResult();
    }
}