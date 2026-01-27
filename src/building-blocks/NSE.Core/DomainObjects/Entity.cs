using NSE.Core.Data;
using NSE.Core.Messages.Base;

namespace NSE.Core.DomainObjects;

public abstract class Entity : AuditableEntity
{
    private List<Event> _events;
    public Guid Id { get; set; }
    public IReadOnlyCollection<Event> Notifications => _events?.AsReadOnly();

    protected Entity()
    {
        Id = Guid.NewGuid();
    }
    
    public void AddEvent(Event @event)
    {
        _events ??= new List<Event>();
        _events.Add(@event);
    }

    public void RemoveEvent(Event eventItem)
    {
        _events?.Remove(eventItem);
    }

    public void ClearEvents()
    {
        _events?.Clear();
    }
    
    public override bool Equals(object obj)
    {
        var compareTo = obj as Entity;

        if (ReferenceEquals(this, compareTo)) return true;
        if (ReferenceEquals(null, compareTo)) return false;

        return Id.Equals(compareTo.Id);
    }

    public static bool operator ==(Entity a, Entity b)
    {
        if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
            return true;

        if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(Entity a, Entity b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        return GetType().GetHashCode() * 907 + Id.GetHashCode();
    }

    public override string ToString()
    {
        return $"{GetType().Name} [Id={Id}]";
    }
}