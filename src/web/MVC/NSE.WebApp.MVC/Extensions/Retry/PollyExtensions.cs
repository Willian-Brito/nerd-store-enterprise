using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace NSE.WebApp.MVC.Extensions.Retry;

public static class PollyExtensions
{
    public static AsyncRetryPolicy<HttpResponseMessage> WaitAndRetry()
     {
         var retry = HttpPolicyExtensions
             .HandleTransientHttpError()
             .WaitAndRetryAsync(new[]
             {
                 TimeSpan.FromSeconds(1),
                 TimeSpan.FromSeconds(5),
                 TimeSpan.FromSeconds(10)
             }, (outcome, timespan, retryCount, context) =>
             {
                 Console.ForegroundColor = ConsoleColor.Red;
                 Console.WriteLine($"Trying for the {retryCount} time!");
                 Console.ForegroundColor = ConsoleColor.White;
             });

         return retry;
     }
}