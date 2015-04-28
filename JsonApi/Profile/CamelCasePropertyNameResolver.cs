using System.Collections.Concurrent;
using Newtonsoft.Json.Serialization;

namespace JsonApi.Profile
{
    public class CamelCasePropertyNameResolver : IPropertyNameResolver
    {
        private static readonly ConcurrentDictionary<string, string> CachedNames = new ConcurrentDictionary<string, string>();  
        private static readonly CamelCasePropertyNamesContractResolver Resolver = new CamelCasePropertyNamesContractResolver(); 

        public string ResolvePropertyName(string propertyName)
        {
            return CachedNames.GetOrAdd(propertyName, n => Resolver.GetResolvedPropertyName(n));
        }
    }
}
