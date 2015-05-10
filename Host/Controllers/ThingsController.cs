﻿using System;
using System.Collections.Generic;
using System.Web.Http;
using JsonApi.ObjectModel;
using JsonApi.ServiceModel;

namespace Host.Controllers
{
    public class ThingsController : JsonApiResourceController<Thing>
    {
        // GET api/values
        public override IEnumerable<JsonApiResponse<Thing>> Get()
        {
            var sharedThing2 = new Thing
            {
                ThingGuid = new Guid("5718EA37-8FE4-4B0F-8126-0B07BCF3C29B"),
                Value = "Shared"
            };

            var sharedThing = new Thing
            {
                ThingGuid = new Guid("7E73D99B-5328-4872-9538-912396515A7D"),
                Value = "Bananas",
                Sub = new SubThing { V1 = 20, V2 = 40 },
                Rel1 = new Thing
                {
                    ThingGuid = new Guid("B47666DB-EBED-4522-80D8-5F8140DDDF38"),
                    Value = "Pairs"
                },
                Rel2 = new []
                    {
                        new Thing
                        {
                            ThingGuid = new Guid("C1500A37-82CE-4DF0-86AF-018AE441CA4C"),
                            Value = "Apples",
                            Rel1 = sharedThing2
                        },
                        new Thing
                        {
                            ThingGuid = new Guid("62A4C9F3-7EB9-45B2-AA4C-DDAAA210C200"),
                            Value = "Cherries",
                            Rel1 = sharedThing2
                        }
                    }
            };

            return new List<JsonApiResponse<Thing>>
                {
                    new JsonApiResponse<Thing>(new Thing
                        {
                            ThingGuid = new Guid("7E73D99B-5328-4872-9538-912396515A7D"),
                            Value = "Bananas",
                            Rel1 = sharedThing
                        })
                    {
                        Metadata = new Dictionary<string, object>
                        {
                            {"metaFoo", 42}
                        }
                    },
                    new Thing
                        {
                            ThingGuid = new Guid("9BFC9927-047D-45D0-99F1-BBBD1C76F439"),
                            Value = "Oranges",
                            Rel2 = new []{sharedThing}
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

    [ResourceObject(Type = "Thingy")]
    public class Thing
    {
        [ResourceId]
        public Guid ThingGuid { get; set; }
        public string Value { get; set; }
        public SubThing Sub { get; set; }

        [ResourceRelationship(Sideload = true)]
        public Thing Rel1 { get; set; }

        [ResourceRelationship(Sideload = true)]
        public Thing[] Rel2 { get; set; }
    }

    public class SubThing
    {
        public int V1 { get; set; }
        public int V2 { get; set; }
    }
}