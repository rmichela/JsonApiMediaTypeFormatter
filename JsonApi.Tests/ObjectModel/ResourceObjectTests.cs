using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
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
        public void ShouldSerializeAlternateType()
        {
            var r = new ResourceWithAlternateType { Id = 1 };
            var ro = new ResourceObject(r, _p);
            JToken json = ro.ToJson();
            Assert.AreEqual("Bananas", (string)json["type"]);
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

        [Test]
        [ExpectedException(typeof(JsonApiSpecException))]
        public void ShouldErrorWhenLinkAndPropertyShareName()
        {
            var r = new ResourceWithConflictingRelationship
                {
                    Conflict = new Resource {Id = 1},
                    CoNfLiCt = 1
                };
            var ro = new ResourceObject(r, _p);
            JToken json = ro.ToJson();
        }

        [Test]
        [ExpectedException(typeof (JsonApiSpecException))]
        public void ShouldErrorWhenResourceHasTypeAttribute()
        {
            var r = new ResourceWithType {Id = 1, Type = 1};
            var ro = new ResourceObject(r, _p);
        }

        [Test]
        [ExpectedException(typeof(JsonApiSpecException))]
        public void ShouldErrorWhenResourceHasLinksAttribute()
        {
            var r = new ResourceWithLinks { Id = 1, Links = 1 };
            var ro = new ResourceObject(r, _p);
        }

        [Test]
        [ExpectedException(typeof(JsonApiSpecException))]
        public void ShouldErrorWhenResourceHasMetaAttribute()
        {
            var r = new ResourceWithMeta { Id = 1, Meta = 1 };
            var ro = new ResourceObject(r, _p);
        }

        [Test]
        [ExpectedException(typeof(JsonApiSpecException))]
        public void ShouldErrorWhenResourceHasSelfAttribute()
        {
            var r = new ResourceWithSelf { Id = 1, Self = 1 };
            var ro = new ResourceObject(r, _p);
        }

        [Test]
        public void SimpleObjectsShouldNotHaveLinks()
        {
            var r = new Resource { Id = 1 };
            var ro = new ResourceObject(r, _p);
            var links = ro.ExtractAndRewireResourceLinks();
            Assert.IsEmpty(links);
            Assert.IsEmpty(ro.Links);

            JToken json = ro.ToJson();
            Assert.Null(json["links"]);
        }

        [Test]
        public void LinkedObjectsShouldHaveLinks()
        {
            var r = new ResourceWithRelationship
            {
                Id = 1,
                ToOne = new Resource { Id = 2 }
            };
            var ro = new ResourceObject(r, _p);
            var links = ro.ExtractAndRewireResourceLinks();
            Assert.IsNotEmpty(links);
            Assert.IsNotEmpty(ro.Links);

            JToken json = ro.ToJson();
            Assert.NotNull(json["links"]);
        }

        [Test]
        public void LinkedObjectShouldHandleSingleLinkage()
        {
            var r = new ResourceWithRelationship
            {
                Id = 1,
                ToOne = new Resource { Id = 2 }
            };
            var ro = new ResourceObject(r, _p);
            ro.ExtractAndRewireResourceLinks();
            var link = ro.Links.First(l => l.LinkType == LinkType.ToOne);

            Assert.AreEqual("2", link.Resources.First().ResourceIdentifier.Id);
            Assert.AreEqual("Resources", link.Resources.First().ResourceIdentifier.Type);
            Assert.AreEqual(LinkType.ToOne, link.LinkType);
        }

        [Test]
        public void LinkedObjectShouldHandleMultipleLinkage()
        {
            var r = new ResourceWithRelationship
            {
                Id = 1,
                ToMany = new List<Resource>
                {
                    new Resource {Id = 2}
                }
            };
            var ro = new ResourceObject(r, _p);
            ro.ExtractAndRewireResourceLinks();
            var link = ro.Links.First(l => l.LinkType == LinkType.ToMany);

            Assert.AreEqual("2", link.Resources.First().ResourceIdentifier.Id);
            Assert.AreEqual("Resources", link.Resources.First().ResourceIdentifier.Type);
            Assert.AreEqual(LinkType.ToMany, link.LinkType);
        }

        [Test]
        public void EmptyToOneShouldSerializeAsNull()
        {
            var r = new ResourceWithRelationship { Id = 1 };
            var ro = new ResourceObject(r, _p);
            ro.ExtractAndRewireResourceLinks();

            var link = ro.Link("ToOne");
            var json = link.ToJson();
            Assert.AreEqual(JTokenType.Null, json["linkage"].Type);
        }

        [Test]
        public void PopulatedToOneShouldSerializeAsObject()
        {
            var r = new ResourceWithRelationship
            {
                Id = 1,
                ToOne = new Resource { Id = 2 }
            };
            var ro = new ResourceObject(r, _p);
            ro.ExtractAndRewireResourceLinks();

            var link = ro.Link("ToOne");
            var json = link.ToJson();
            Assert.AreEqual(JTokenType.Object, json["linkage"].Type);
        }

        [Test]
        public void EmptyToManyShouldSerializeAsEmptyArray()
        {
            var r = new ResourceWithRelationship { Id = 1 };
            var ro = new ResourceObject(r, _p);
            ro.ExtractAndRewireResourceLinks();

            var link = ro.Link("ToMany");
            var json = link.ToJson();
            Assert.AreEqual(JTokenType.Array, json["linkage"].Type);
            Assert.IsEmpty(json["linkage"]);
        }

        [Test]
        public void PopulatedToManyShouldSerializeAsArray()
        {
            var r = new ResourceWithRelationship
            {
                Id = 1,
                ToMany = new List<Resource>
                {
                    new Resource {Id = 2}
                }
            };
            var ro = new ResourceObject(r, _p);
            ro.ExtractAndRewireResourceLinks();

            var link = ro.Link("ToMany");
            var json = link.ToJson();
            Assert.AreEqual(JTokenType.Array, json["linkage"].Type);
            Assert.IsNotEmpty(json["linkage"]);
        }
    }
}
