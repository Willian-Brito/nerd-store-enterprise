using NSE.Core.Messages.Base;
using FluentValidation.Results;

namespace NSE.Core.Bus;

public interface IMessageBus
{
    Task PublishEvent<T>(T @event) where T : Event;
    Task<ValidationResult> SendCommand<T>(T command) where T : Command;
}