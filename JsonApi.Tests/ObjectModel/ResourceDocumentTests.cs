using System.Collections.Generic;
using JsonApi.ObjectModel;
using JsonApi.Profile;
using NUnit.Framework;
using Newtonsoft.Json.Linq;

namespace JsonApi.Tests.ObjectModel
{
    [TestFixture ]
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

        private bool IsResource(JToken json)
        {
            return json["id"] != null && json["type"] != null;
        }
    }
}
