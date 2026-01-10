using MediatR;

namespace NSE.Core.Messages.Base;

public class Event : Message, INotification
{
    protected Event()
    {
        Timestamp = DateTime.Now;
    }

    public DateTime Timestamp { get; private set; }
}