using JsonApi.ObjectModel;
using JsonApi.Profile;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace JsonApi.Tests.ObjectModel
{
    [TestFixture]
    public class MetadataTests
    {
        private readonly IJsonApiProfile _p = new RecommendedProfile();

        [Test]
        public void NoMetadataShouldNotSerializeMetaAttribute()
        {
            var r = new Resource { Id = 1 };
            var ro = new ResourceObject(r, _p);
            JToken json = ro.ToJson();
            Assert.IsNull(json["meta"]);
        }

        [Test]
        public void ResourceObjectWithMetadaShouldSerializeMetaAttribute()
        {
            var r = new Resource { Id = 1 };
            var ro = new ResourceObject(r, _p);
            ro.Meta.MyMeta = "foo";
            JToken json = ro.ToJson();
            Assert.AreEqual("foo", (string)json["meta"]["my-meta"]);
        }

        [Test]
        public void ResourceDocumentWithMetadaShouldSerializeMetaAttribute()
        {
            var r = new Resource { Id = 1 };
            var d = new ResourceDocument(new ResourceObject(r, _p), _p);
            d.Meta.MyMeta = "foo";
            JToken json = d.ToJson();
            Assert.AreEqual("foo", (string)json["meta"]["my-meta"]);
        }

        [Test]
        public void LinkObjectWithMetadaShouldSerializeMetaAttribute()
        {
            var l = LinkObject.Empty(LinkType.ToOne);
            l.Meta.MyMeta = "foo";
            JToken json = l.ToJson();
            Assert.AreEqual("foo", (string)json["meta"]["my-meta"]);
        }
    }
}
