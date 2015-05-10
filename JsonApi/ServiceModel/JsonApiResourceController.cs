using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace JsonApi.ServiceModel
{
    public abstract class JsonApiResourceController<T> : ApiController where T:class
    {

        /// <summary>
        /// GET a collection of T
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<JsonApiResponse<T>> Get();

        public virtual JsonApiResponse<T> Get(string id)
        {
            return Get().FirstOrDefault(r => r.Id == id) ?? JsonApiResponse<T>.NotFound;
        }
    }
}
