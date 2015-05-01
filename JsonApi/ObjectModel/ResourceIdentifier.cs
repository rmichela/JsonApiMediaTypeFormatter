namespace JsonApi.ObjectModel
{
    public class ResourceIdentifier
    {
        public ResourceIdentifier(string type, string id)
        {
            Type = type;
            Id = id;
        }

        public string Type { get; private set; }
        public string Id { get; private set; }

        #region Equality 
        protected bool Equals(ResourceIdentifier other)
        {
            return string.Equals(Type, other.Type) && string.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((ResourceIdentifier)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ (Id != null ? Id.GetHashCode() : 0);
            }
        }

        public static bool operator ==(ResourceIdentifier left, ResourceIdentifier right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ResourceIdentifier left, ResourceIdentifier right)
        {
            return !Equals(left, right);
        }
        #endregion
    }
}
