using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;

namespace JsonApi.Profile
{
    public class DefaultInflector : IInflector
    {
        public string Pluralize(string word)
        {
            var inflector = PluralizationService.CreateService(CultureInfo.CurrentCulture);
            return inflector.Pluralize(word);
        }

        public string Singularize(string word)
        {
            var inflector = PluralizationService.CreateService(CultureInfo.CurrentCulture);
            return inflector.Singularize(word);
        }
    }
}
