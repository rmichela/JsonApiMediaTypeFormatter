using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JsonApi.ObjectModel;
using JsonApi.Profile;

namespace JsonApi.ServiceModel
{
    public abstract class JsonApiResponse
    {
        public bool IsCollection { get; protected set; }
        protected IEnumerable<object> Obj;

        public ResourceDocument ToDocument(IJsonApiProfile profile)
        {
            if (IsCollection)
            {
                var resourceObjectList = Obj.Select(o => new ResourceObject(o, profile)).ToList();
                return new ResourceDocument(resourceObjectList, profile);
            }
            else
            {
                return new ResourceDocument(new ResourceObject(Obj.FirstOrDefault(), profile), profile);
            }
        }
    }

    public class JsonApiResponse<T> : JsonApiResponse where T:class
    {
        public static readonly JsonApiResponse<T> EmptyCollection = new JsonApiResponse<T>(new List<T>());
        public static readonly JsonApiResponse<T> NotFound = new JsonApiResponse<T>((T)null); 

        public JsonApiResponse(T obj)
        {
            ValidateResourceObjectAttribute();

            if (obj == null)
            {
                Obj = new List<T>();
            }
            else
            {
                Obj = new List<T> { obj };                
            }
            IsCollection = false;
        }

        public JsonApiResponse(IEnumerable<T> obj)
        {
            ValidateResourceObjectAttribute();

            if (obj == null)
            {
                Obj = new List<T>();
            }
            else
            {
                Obj = obj;                
            }
            IsCollection = true;
        }

        public static implicit operator JsonApiResponse<T>(T obj)
        {
            return new JsonApiResponse<T>(obj);
        }

        public static implicit operator JsonApiResponse<T>(List<T> obj)
        {
            return new JsonApiResponse<T>(obj);
        }

        public static implicit operator JsonApiResponse<T>(T[] obj)
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
