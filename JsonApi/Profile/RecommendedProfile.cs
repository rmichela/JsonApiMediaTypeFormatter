namespace JsonApi.Profile
{
    public class RecommendedProfile : IJsonApiProfile
    {
        public virtual IInflector Inflector { get {return new DefaultInflector();}}
        public virtual IPropertyNameResolver PropertyNameResolver { get {return new DasherizingPropertyNameResolver();} }
    }
}
