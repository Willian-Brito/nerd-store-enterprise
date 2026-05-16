using System.Threading.Tasks;

namespace NSE.WebAPI.Core.Http;

public interface IRestClient
{
    Task<TResult> PostAsync<T, TResult>(T @event, string token = null);
}