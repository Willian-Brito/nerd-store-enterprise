using FluentValidation.Results;
using System.Text.Json.Serialization;
using MediatR;

namespace NSE.Core.Messages.Base;

public abstract class Command : Message, IRequest<ValidationResult>
{
    [JsonIgnore]
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;

    [JsonIgnore]
    public ValidationResult ValidationResult { get; set; }

    public Command()
    {
        ValidationResult = new ValidationResult();
    }
    
    public abstract bool IsValid();
}