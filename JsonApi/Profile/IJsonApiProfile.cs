namespace JsonApi.Profile
{
    public interface IJsonApiProfile
    {
        IInflector Inflector { get; }
        IPropertyNameResolver PropertyNameResolver { get; }
    }
}
