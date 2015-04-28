using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonApi.ObjectModel;
using JsonApi.Profile;
using NUnit.Framework;

namespace JsonApi.Tests.ObjectModel
{
    [TestFixture]
    public class ResourceObjectTests
    {
        [ResourceObject]
        private class ShouldSerializeSomethingType
        {
            public string Id = "1";
        }

        [Test]
        public void ShouldSerializeIdAndType()
        {
            var ro = new ResourceObject(new ShouldSerializeSomethingType(), new RecommendedProfile());
            var jobj = ro.ToJson();
            Assert.AreEqual("1", (string)jobj["id"]);
            Assert.AreEqual("ShouldSerializeSomethingTypes", (string)jobj["type"]);
        }

    }
}
