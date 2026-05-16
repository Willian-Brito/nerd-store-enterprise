namespace NSE.MessageBroker.Configuration;

public class QueueSettings
{
    public MessageBroker Provider { get; set; }

    public string ConnectionString { get; set; }
}