using FluentValidation.Results;
using MediatR;
using NSE.Core.Messages.Base;

namespace NSE.Core.Bus;

public class MessageBus : IMessageBus
{
    private readonly IMediator _mediator;

    public MessageBus(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public async Task PublishEvent<T>(T @event) where T : Event
    {
        await _mediator.Publish(@event);
    }

    public async Task<ValidationResult> SendCommand<T>(T command) where T : Command
    {
        return await _mediator.Send(command);
    }
}