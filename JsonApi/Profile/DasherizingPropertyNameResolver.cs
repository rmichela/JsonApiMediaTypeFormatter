using System.Collections.Concurrent;
using System.Text;

namespace JsonApi.Profile
{
    public class DasherizingPropertyNameResolver : IPropertyNameResolver
    {
        private static readonly ConcurrentDictionary<string, string> CachedNames = new ConcurrentDictionary<string, string>();  

        public string ResolvePropertyName(string propertyName)
        {
            return CachedNames.GetOrAdd(propertyName, Dasherize);
        }

        private string Dasherize(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new JsonApiSpecException("Property name cannot be null or empty");
            }

            char[] chars = propertyName.ToCharArray();
            var sb = new StringBuilder(propertyName.Length);

            sb.Append(chars[0]); // First character
            if (chars.Length > 1)
            {
                for (int i = 1; i < chars.Length - 1; i++)
                {
                    sb.Append(chars[i]);
                    if (char.IsUpper(chars[i + 1]))
                    {
                        sb.Append('-');
                    }
                }
                sb.Append(chars[chars.Length - 1]); // Last character
            }

            return sb.ToString().ToLower();
        }
    }
}
