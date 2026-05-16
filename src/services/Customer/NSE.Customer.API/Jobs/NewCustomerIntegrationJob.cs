using NSE.Core.Bus;
using NSE.Core.Messages.Base;
using NSE.Core.Messages.Integration;
using NSE.Customer.API.Application.Commands;
using NSE.MessageBroker.Abstractions;

namespace NSE.Customer.API.Jobs;

public class NewCustomerIntegrationJob : BackgroundService
{
    private readonly IQueue _queue; 
    private readonly IRpcBus _rpcBus;
    private readonly IServiceProvider _serviceProvider;
    
    public NewCustomerIntegrationJob(
        IServiceProvider serviceProvider, 
        IQueue queue,
        IRpcBus rpcBus
    )
    {
        _serviceProvider = serviceProvider;
        _queue = queue;
        _rpcBus = rpcBus;
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        SetResponder();
        return Task.CompletedTask;
    }
    
    // Kafka Implementation (Comment)
    private void SetResponder()
    {
        _rpcBus.RespondAsync<UserRegisteredIntegrationEvent, ResponseMessage>(
            async message => await AddCustomer(message)
        );
        _rpcBus.AdvancedBus.Connected += OnConnect;
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