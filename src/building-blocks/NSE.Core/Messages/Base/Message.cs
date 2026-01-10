namespace NSE.Core.Messages.Base;

public abstract class Message
{
    protected Message()
    {
        MessageType = GetType().Name;
    }

    public string MessageType { get; protected set; }
    public Guid AggregateId { get; protected set; }
}