using JsonApi.ObjectModel;
using JsonApi.Profile;
using NUnit.Framework;
using Newtonsoft.Json.Linq;

namespace JsonApi.Tests.ObjectModel
{
    [TestFixture]
    public class ResourceObjectTests
    {
        private readonly IJsonApiProfile _p = new RecommendedProfile();

        [Test]
        [ExpectedException(typeof(JsonApiSpecException))]
        public void ResourceMustHaveResourceObjectAttribute()
        {
            var r = new ResourceWithoutAttribute {Id = 1};
            var ro = new ResourceObject(r, _p);
        }

        [Test]
        public void ShouldSerializeIdAndType()
        {
            var r = new Resource {Id = 1};
            var ro = new ResourceObject(r, _p);
            JToken json = ro.ToJson();
            Assert.AreEqual("1", (string)json["id"]);
            Assert.AreEqual("Resources", (string)json["type"]);
        }

        [Test]
        [ExpectedException(typeof(JsonApiSpecException))]
        public void ResourceMustHaveId()
        {
            var r = new ResourceWithoutId {Value = 10};
            var ro = new ResourceObject(r, _p);
        }

        [Test]
        public void ShouldSerializeIdWithIdAttribute()
        {
            var r = new ResourceWithoutIdWithAttribute { Value = 1 };
            var ro = new ResourceObject(r, _p);
            JToken json = ro.ToJson();
            Assert.AreEqual("1", (string)json["id"]);
        }

        [Test]
        public void ShouldSerializeIdAndTypeAsString()
        {
            var r = new Resource { Id = 1 };
            var ro = new ResourceObject(r, _p);
            JToken json = ro.ToJson();
            Assert.AreEqual(JTokenType.String, json["id"].Type);
            Assert.AreEqual(JTokenType.String, json["type"].Type);
        }

        [Test]
        public void ShouldSerializeResourceAttributes()
        {
            var r = new Resource {Id = 1, AttributeI = 10, AttributeS = "10"};
            var ro = new ResourceObject(r, _p);
            JToken json = ro.ToJson();
            Assert.AreEqual(10, (int)json["attribute-i"]);
            Assert.AreEqual(JTokenType.Integer, json["attribute-i"].Type);
            Assert.AreEqual("10", (string)json["attribute-s"]);
            Assert.AreEqual(JTokenType.String, json["attribute-s"].Type);
        }

        [Test]
        public void ShouldSerializeComplexAttributes()
        {
            var r = new ResourceWithComplexAttribute
                {
                    Id = 1,
                    AttributeC = new ComplexAttribute
                        {
                            AttributeI = 10,
                            AttributeS = "10"
                        }
                };
            var ro = new ResourceObject(r, _p);
            JToken json = ro.ToJson();
            Assert.AreEqual(JTokenType.Object, json["attribute-c"].Type);
            Assert.AreEqual(10, (int)json["attribute-c"]["attribute-i"]);
            Assert.AreEqual("10", (string)json["attribute-c"]["attribute-i"]);
        }

        [Test]
        [ExpectedException(typeof(JsonApiSpecException))]
        public void ShouldErrorSerializingIllegalComplexAttributeWithId()
        {
            var r = new ResourceWithIllegalComplexAttributes
                {
                    Id = 1,
                    WithId = new ComplexAttributeWithId {Id = 1}
                };
            var ro = new ResourceObject(r, _p);
            JToken json = ro.ToJson();
        }

        [Test]
        [ExpectedException(typeof(JsonApiSpecException))]
        public void ShouldErrorSerializingIllegalComplexAttributeWithType()
        {
            var r = new ResourceWithIllegalComplexAttributes
            {
                Id = 1,
                WithType = new ComplexAttributeWithType { Type = 1 }
            };
            var ro = new ResourceObject(r, _p);
            JToken json = ro.ToJson();
        }

        [Test]
        [ExpectedException(typeof(JsonApiSpecException))]
        public void ShouldErrorSerializingIllegalComplexAttributeWithLinks()
        {
            var r = new ResourceWithIllegalComplexAttributes
            {
                Id = 1,
                WithLinks = new ComplexAttributeWithLinks { Links = 1 }
            };
            var ro = new ResourceObject(r, _p);
            JToken json = ro.ToJson();
        }

        [Test]
        [ExpectedException(typeof(JsonApiSpecException))]
        public void ShouldErrorSerializingIllegalComplexAttributeWithMeta()
        {
            var r = new ResourceWithIllegalComplexAttributes
            {
                Id = 1,
                WithMeta = new ComplexAttributeWithMeta { Meta = 1 }
            };
            var ro = new ResourceObject(r, _p);
            JToken json = ro.ToJson();
        }

        [Test]
        [ExpectedException(typeof(JsonApiSpecException))]
        public void ShouldErrorSerializingComplexAttributeIllegalNested()
        {
            var r = new ResourceWithIllegalComplexAttributes
            {
                Id = 1,
                WithNested = new ComplexAttributeWithIllegalAttribute
                    {
                        WithId = new ComplexAttributeWithId { Id = 1 }
                    }
            };
            var ro = new ResourceObject(r, _p);
            JToken json = ro.ToJson();
        }
    }
}
