using System.IO;
using JsonApi.Profile;
using JsonApi.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonApi.Tests
{
    internal static class TestExtensions
    {
        public static JObject ToJson(this IJsonWriter toBeWritten)
        {
            using (var stringWriter = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(stringWriter))
            {
                var serializer = JsonSerializer.CreateDefault(new JsonSerializerSettings
                    {
                        ContractResolver = new JsonApiContractResolver(new RecommendedProfile())
                    });
                toBeWritten.WriteJson(jsonWriter, serializer);
                stringWriter.Flush();

                return JObject.Parse(stringWriter.GetStringBuilder().ToString());
            }
        }
    }
}
