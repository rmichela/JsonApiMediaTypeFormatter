namespace JsonApi.Profile
{
    public interface IInflector
    {
        string Pluralize(string word);
        string Singularize(string word);
    }
}
