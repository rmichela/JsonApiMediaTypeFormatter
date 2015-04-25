﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using JsonApi;
using JsonApi.ObjectModel;

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
                            ThingGuid = new Guid("7E73D99B-5328-4872-9538-912396515A7D"),
                            Value = "Bananas"
                        },
                    new Thing
                        {
                            ThingGuid = new Guid("9BFC9927-047D-45D0-99F1-BBBD1C76F439"),
                            Value = "Oranges"
                        }
                };
        }

        // GET api/values/5
        public Thing Get(int id)
        {
            return new Thing
                {
                    ThingGuid = new Guid("7E73D99B-5328-4872-9538-912396515A7D"),
                    Value = "Bananas",
                    Sub = new SubThing { V1 = 20, V2 = 40 },
                    Rel1 = new Thing
                        {
                            ThingGuid = new Guid("B47666DB-EBED-4522-80D8-5F8140DDDF38"),
                            Value = "Pairs"
                        },
                    Rel2 = new List<Thing>
                    {
                        new Thing
                        {
                            ThingGuid = new Guid("C1500A37-82CE-4DF0-86AF-018AE441CA4C"),
                            Value = "Apples"
                        },
                        new Thing
                        {
                            ThingGuid = new Guid("62A4C9F3-7EB9-45B2-AA4C-DDAAA210C200"),
                            Value = "Cherries"
                        }
                    }
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

//    [ResourceObject(Type = "Thingy")]
    [ResourceObject]
    public class Thing
    {
        [ResourceId]
        public Guid ThingGuid { get; set; }
        public string Value { get; set; }
        public SubThing Sub { get; set; }
        [ResourceRelationship(Sideload = false)]
        public Thing Rel1 { get; set; }
        [ResourceRelationship(Sideload = true)]
        public List<Thing> Rel2 { get; set; }
    }

    public class SubThing
    {
        public int V1 { get; set; }
        public int V2 { get; set; }
    }
}