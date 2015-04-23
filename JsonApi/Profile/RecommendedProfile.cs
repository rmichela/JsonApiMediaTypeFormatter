namespace JsonApi.Profile
{
    public class RecommendedProfile : IJsonApiProfile
    {
        public virtual IInflector Inflector { get {return new SystemDataInflector();}}
    }
}
