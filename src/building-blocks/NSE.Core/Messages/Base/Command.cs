using FluentValidation.Results;
using MediatR;

namespace NSE.Core.Messages.Base;

public abstract class Command : Message, IRequest<ValidationResult>
{
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;
    public ValidationResult ValidationResult { get; set; }

    public abstract bool IsValid();
}