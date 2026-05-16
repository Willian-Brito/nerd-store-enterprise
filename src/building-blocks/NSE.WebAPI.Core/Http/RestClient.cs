using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NSE.WebAPI.Core.Http;

public class RestClient : IRestClient
{
    private static JsonSerializerOptions _options = new JsonSerializerOptions
    {
        IgnoreReadOnlyProperties = true,
        IgnoreReadOnlyFields = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true, 
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };
    
    private readonly IHttpClientFactory _httpClientFactory;
    
    public RestClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<TResult> PostAsync<T, TResult>(T @event, string token = null)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(@event), 
            Encoding.UTF8, 
            "application/json"
        );

        var httpRequestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            Content = content
        };

        if (!string.IsNullOrEmpty(token))
        {
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        
        var httpClient = _httpClientFactory.CreateClient(@event.GetType().Name);
        var responseMessage = await httpClient.SendAsync(httpRequestMessage);
        var responseContent = await responseMessage.Content.ReadAsStringAsync();
        var response = JsonSerializer.Deserialize<TResult>(responseContent, _options);
        
        return response;
    }
}