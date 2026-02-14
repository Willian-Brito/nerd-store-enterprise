using NSE.Core.DomainObjects;

namespace NSE.Core.Data;

public interface IAuditableEntity
{
    DateTime CreatedAt { get; }
    Guid? CreatedBy { get; }
    DateTime? UpdatedAt { get; }
    Guid? UpdatedBy { get; }
    DateTime? DeletedAt { get; }
    Guid? DeletedBy { get; }
}

public abstract class AuditableEntity : Entity, IAuditableEntity
{
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }
}