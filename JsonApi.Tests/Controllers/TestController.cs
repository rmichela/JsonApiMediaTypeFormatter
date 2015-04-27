using System.Web.Http;
using JsonApi.ObjectModel;

namespace JsonApi.Tests.Controllers
{
    public class TestController : ApiController
    {
        public TestResource Get()
        {
            return new TestResource
                {
                    Id = 1,
                    Value = "Test"
                };
        }
    }

    [ResourceObject]
    public class TestResource
    {
        [ResourceId]
        public int Id { get; set; }
        public string Value { get; set; }
    }
}
