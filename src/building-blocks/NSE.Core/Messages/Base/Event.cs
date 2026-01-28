using MediatR;

namespace NSE.Core.Messages.Base;

public class Event : Message, INotification
{
    public DateTime Timestamp { get; private set; }
    
    protected Event()
    {
        Timestamp = DateTime.Now;
    }
}