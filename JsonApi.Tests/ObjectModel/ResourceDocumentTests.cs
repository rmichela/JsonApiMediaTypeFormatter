using System.Collections.Generic;
using System.Linq;
using JsonApi.ObjectModel;
using JsonApi.Profile;
using NUnit.Framework;
using Newtonsoft.Json.Linq;

namespace JsonApi.Tests.ObjectModel
{
    [TestFixture]
    public class ResourceDocumentTests
    {
        private readonly IJsonApiProfile _p = new RecommendedProfile();

        [Test]
        public void DocumentRootMustBeJObject()
        {
            var r = new Resource {Id = 1};
            var d = new ResourceDocument(new ResourceObject(r, _p), _p);
            JToken json = d.ToJson();
            Assert.AreEqual(JTokenType.Object, json.Type);
        }

        [Test]
        public void DocumentRootMustContainPrimaryData()
        {
            var r = new Resource { Id = 1 };
            var d = new ResourceDocument(new ResourceObject(r, _p), _p);
            JToken json = d.ToJson();
            Assert.IsNotNull(json["data"]);
            Assert.IsNull(json["errors"]);
        }
        
        [Test]
        public void DocumentRootMustContainPrimaryObject()
        {
            var r = new Resource { Id = 1 };
            var d = new ResourceDocument(new ResourceObject(r, _p), _p);
            JToken json = d.ToJson();
            Assert.AreEqual(JTokenType.Object, json["data"].Type);
            Assert.IsTrue(IsResource(json["data"]));
        }

        [Test]
        public void DocumentRootMustContainPrimaryArraySingle()
        {
            var r = new Resource { Id = 1 };
            var d = new ResourceDocument(new List<ResourceObject> { new ResourceObject(r, _p) }, _p);
            JToken json = d.ToJson();
            Assert.AreEqual(JTokenType.Array, json["data"].Type);
            Assert.IsTrue(IsResource(json["data"][0]));
        }

        [Test]
        public void DocumentRootMustContainPrimaryArrayMultiple()
        {
            var r1 = new Resource { Id = 1 };
            var r2 = new Resource { Id = 2 };
            var d = new ResourceDocument(new List<ResourceObject>{new ResourceObject(r1, _p), new ResourceObject(r2, _p)}, _p);
            JToken json = d.ToJson();
            Assert.AreEqual(JTokenType.Array, json["data"].Type);
            Assert.IsTrue(IsResource(json["data"][0]));
            Assert.IsTrue(IsResource(json["data"][1]));
        }

        [Test]
        public void DocumentRootMustContainErrors()
        {
            var e = new Error { Id = "1" };
            var d = new ResourceDocument(new List<Error> { e }, _p);
            JToken json = d.ToJson();
            Assert.IsNotNull(json["errors"]);
            Assert.IsNull(json["data"]);
        }

        [Test]
        public void DocumentRootMustContainErrorsAsArray()
        {
            var e = new Error {Id = "1"};
            var d = new ResourceDocument(new List<Error> {e}, _p);
            JToken json = d.ToJson();
            Assert.AreEqual(JTokenType.Array, json["errors"].Type);
        }

        [Test]
        [ExpectedException(typeof(JsonApiSpecException))]
        public void TopLevelResourcesMustBeUnique()
        {
            var r1 = new Resource { Id = 1 };
            var r2 = new Resource { Id = 1 };
            var d = new ResourceDocument(new List<ResourceObject> { new ResourceObject(r1, _p), new ResourceObject(r2, _p) }, _p);
        }

        [Test]
        public void SimpleDocumentShouldLackIncludedSection()
        {
            var r = new Resource { Id = 1 };
            var d = new ResourceDocument(new ResourceObject(r, _p), _p);
            JToken json = d.ToJson();
            Assert.IsNull(json["included"]);
        }

        [Test]
        public void CompoundDocumentShouldHaveIncludedSection()
        {
            var r = new ResourceWithRelationship
            {
                Id = 1,
                ToOne = new Resource
                {
                    Id = 2
                }
            };
            var d = new ResourceDocument(new ResourceObject(r, _p), _p);
            JToken json = d.ToJson();
            Assert.IsNotNull(json["included"]);
        }

        [Test]
        public void CompoundDocumentMustHaveArrayOfResourcesAsIncludedSection()
        {
            var r = new ResourceWithRelationship
            {
                Id = 1,
                ToOne = new Resource
                {
                    Id = 2
                }
            };
            var d = new ResourceDocument(new ResourceObject(r, _p), _p);
            JToken json = d.ToJson();
            Assert.AreEqual(JTokenType.Array, json["included"].Type);
            Assert.IsTrue(IsResource(json["included"][0]));
        }

        [Test]
        public void CompoundDocumentWithoutSideloadsShouldLackIncludedSection()
        {
            var r = new ResourceWithRelationshipNoSideload
            {
                Id = 1,
                ToOne = new Resource
                {
                    Id = 2
                }
            };
            var d = new ResourceDocument(new ResourceObject(r, _p), _p);
            JToken json = d.ToJson();
            Assert.IsNull(json["included"]);
        }

        [Test]
        public void CompoundDocumentShouldAcumulateReferencedResources()
        {
            var r1 = new ResourceWithRelationship
            {
                Id = 1,
                ToOne = new Resource
                {
                    Id = 10
                }
            };
            var r2 = new ResourceWithRelationship
            {
                Id = 2,
                ToOne = new Resource
                {
                    Id = 20
                }
            };
            var d = new ResourceDocument(new List<ResourceObject> { new ResourceObject(r1, _p), new ResourceObject(r2, _p) }, _p);
            JToken json = d.ToJson();
            Assert.AreEqual(2, json["included"].Count());
        }

        [Test]
        public void CompoundDocumentShouldNotDuplicateIncludedResources()
        {
            var r1 = new ResourceWithRelationship
            {
                Id = 1,
                ToOne = new Resource
                {
                    Id = 10
                }
            };
            var r2 = new ResourceWithRelationship
            {
                Id = 2,
                ToOne = new Resource
                {
                    Id = 10
                }
            };
            var d = new ResourceDocument(new List<ResourceObject> { new ResourceObject(r1, _p), new ResourceObject(r2, _p) }, _p);
            JToken json = d.ToJson();
            Assert.AreEqual(1, json["included"].Count());
        }

        private bool IsResource(JToken json)
        {
            return json["id"] != null && json["type"] != null;
        }
    }
}
