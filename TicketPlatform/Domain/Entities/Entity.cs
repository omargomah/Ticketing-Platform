namespace Domain.Entities
{
    public abstract class Entity : IEquatable<Entity>
    {
        protected Entity(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; private init; }

        public override bool Equals(object? obj)
        {
            if(obj is not Entity entity)
                return false;
  
            return Equals(other: entity);
        }

        public bool Equals(Entity? other)
        {
            if (other is null)
                return false;

            if (other.GetType() != GetType())
                return false;

            if (other is not Entity entity)
                return false;

            if (Id == Guid.Empty || other.Id == Guid.Empty)
                return false;

            return entity.Id == Id;
        }

        public override int GetHashCode()
        {
            return  Id.GetHashCode() * 41;
        }
        public static bool operator ==(Entity? left, Entity? right)
        {
            if (left is null || right is null)
                return false;
            return left.Equals(other: right);
        }
        public static bool operator !=(Entity? left, Entity? right)
        {
            return !(left == right);
        }


    }
}
