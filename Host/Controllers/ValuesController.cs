using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Host.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<Thing> Get()
        {
            return new Thing[]
                {
                    new Thing
                        {
                            Id = new Guid("7E73D99B-5328-4872-9538-912396515A7D"),
                            Value = "Bananas"
                        },
                    new Thing
                        {
                            Id = new Guid("9BFC9927-047D-45D0-99F1-BBBD1C76F439"),
                            Value = "Oranges"
                        }
                };
        }

        // GET api/values/5
        public Thing Get(int id)
        {
            return new Thing
                {
                    Id = new Guid("7E73D99B-5328-4872-9538-912396515A7D"),
                    Value = "Bananas"
                };
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }

    public class Thing
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
    }
}