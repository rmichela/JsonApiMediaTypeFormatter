using System.Web.Http;

namespace JsonApi.ServiceModel
{
    public abstract class JsonApiResourceController<T> : ApiController where T:class
    {

        /// <summary>
        /// GET a collection of T
        /// </summary>
        /// <returns></returns>
        public virtual JsonApiResponse<T> Get()
        {
            return JsonApiResponse<T>.EmptyCollection;
        }

        public virtual JsonApiResponse<T> Get(string id)
        {
            return JsonApiResponse<T>.NotFound;
        }
    }
}
