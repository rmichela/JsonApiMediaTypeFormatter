using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JsonApi.ObjectModel;
using JsonApi.Profile;

namespace JsonApi.ServiceModel
{
    public abstract class JsonApiResponse
    {
        protected object Obj;
        public IDictionary<string, object> Metadata;

        public ResourceObject ToResourceObject(IJsonApiProfile profile)
        {
            var resource = new ResourceObject(Obj, profile);
            if (Metadata != null && Metadata.Any())
            {
                var metaExpando = (IDictionary<string, object>) resource.Meta;
                foreach (var kvp in Metadata)
                {
                    metaExpando.Add(kvp.Key, kvp.Value);
                }
            }
            return resource;
        }

        public string Id
        {
            get { return ResourceObject.GetResourceId(Obj); }
        }
    }

    public class JsonApiResponse<T> : JsonApiResponse where T:class
    {
        public static readonly JsonApiResponse<T> NotFound = new JsonApiResponse<T>(null);
        public static readonly IEnumerable<JsonApiResponse<T>> EmptyCollection = new List<JsonApiResponse<T>>(); 

        public JsonApiResponse(T obj)
        {
            ValidateResourceObjectAttribute();

            Obj = obj;
        }
        
        public static implicit operator JsonApiResponse<T>(T obj)
        {
            return new JsonApiResponse<T>(obj);
        }

        private void ValidateResourceObjectAttribute()
        {
            if (!typeof(T).IsDefined(typeof(ResourceObjectAttribute)))
            {
                throw new JsonApiSpecException("JsonApi response types must be resource objects as denoted by the [ResourceObject] attribute");
            }
        }
    }
}
