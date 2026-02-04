using FluentValidation.Results;
using NSE.Core.Bus;
using NSE.Core.Messages.Base;
using NSE.Core.Messages.Integration;
using NSE.Customer.API.Application.Commands;
using NSE.Queue.Abstractions;

namespace NSE.Customer.API.Services;

public class NewCustomerIntegrationJob : BackgroundService
{
    private readonly IQueue _queue; 
    private readonly IServiceProvider _serviceProvider;
    
    public NewCustomerIntegrationJob(
        IServiceProvider serviceProvider,
        IQueue queue
    )
    {
        _serviceProvider = serviceProvider;
        _queue = queue;
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        SetResponder();
        return Task.CompletedTask;
    }
    
    private void SetResponder()
    {
        _queue.RespondAsync<UserRegisteredIntegrationEvent, ResponseMessage>(
            async message => await AddCustomer(message)
        );
        _queue.AdvancedBus.Connected += OnConnect;
    }
    
    private void OnConnect(object s, EventArgs e)
    {
        SetResponder();
    }
    
    private async Task<ResponseMessage> AddCustomer(UserRegisteredIntegrationEvent message)
    {
        var customerCommand = new NewCustomerCommand(
            message.Id, 
            message.Name, 
            message.Email, 
            message.SocialNumber
        );

        using var scope = _serviceProvider.CreateScope();
        var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
        var result = await messageBus.SendCommand(customerCommand);

        return new ResponseMessage(result);
    }
}