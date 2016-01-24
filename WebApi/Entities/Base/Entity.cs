using System;

namespace WebApiTemplate.Entities
{
    public interface IEntity
    {
    }

    public interface IEntityWithTypedId<IdT> : IEntity
    {
        IdT Id { get; }
    }

    public interface IHaveAuditInformation
    {
        DateTime CreatedAt { get; set; }
        string CreatedBy { get; set; }
        DateTime ModifiedAt { get; set; }
        string ModifiedBy { get; set; }
    }

    public abstract class Entity : EntityWithTypedId<int>
    {
        public Entity()
        {
            Id = 0;
        }

        public Entity(int id)
        {
            Id = id;
        }
    }

    public abstract class EntityWithTypedId<IdT> : IEntityWithTypedId<IdT>, IHaveAuditInformation
    {
        public virtual IdT Id { get; set; }


        public virtual DateTime CreatedAt { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime ModifiedAt { get; set; }
        public virtual string ModifiedBy { get; set; }

        public virtual bool Equals(IEntityWithTypedId<IdT> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Id, Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if ((obj as IEntityWithTypedId<IdT>) == null) return false;
            return Equals((IEntityWithTypedId<IdT>) obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }

        public static bool operator ==(EntityWithTypedId<IdT> left, EntityWithTypedId<IdT> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityWithTypedId<IdT> left, EntityWithTypedId<IdT> right)
        {
            return !Equals(left, right);
        }
    }
}
