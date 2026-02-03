using System.Text.Json.Serialization;

namespace NSE.Core.Messages.Base;

public abstract class Message
{
    protected Message()
    {
        MessageType = GetType().Name;
    }

    [JsonIgnore]
    public string MessageType { get; protected set; }

    [JsonIgnore]
    public Guid AggregateId { get; protected set; }
}