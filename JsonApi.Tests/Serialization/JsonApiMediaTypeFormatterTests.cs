using System.Collections.Generic;
using System.Linq;
using JsonApi.Profile;
using JsonApi.Serialization;
using JsonApi.Tests.Controllers;
using NUnit.Framework;

namespace JsonApi.Tests.Serialization
{
    [TestFixture]
    public class JsonApiMediaTypeFormatterTests
    {
        [Test]
        public void FormatterShouldSetAcceptedMediaType()
        {
            var formatter = new JsonApiMediaTypeFormatter();
            Assert.True(formatter.SupportedMediaTypes.Any(mt => mt.MediaType == "application/vnd.api+json"));
        }

        [Test]
        public void RecommendedProfileIsDefaultProfile()
        {
            var formatter = new JsonApiMediaTypeFormatter();
            Assert.IsInstanceOf<RecommendedProfile>(formatter.Profile);
        }

        [Test]
        public void FormatterWontWriteString()
        {
            var formatter = new JsonApiMediaTypeFormatter();
            Assert.IsFalse(formatter.CanWriteType(typeof(string)));
        }

        [Test]
        public void FormatterWontWriteNumber()
        {
            var formatter = new JsonApiMediaTypeFormatter();
            Assert.IsFalse(formatter.CanWriteType(typeof(int)));
        }

        [Test]
        public void FormatterWillWriteObject()
        {
            var formatter = new JsonApiMediaTypeFormatter();
            Assert.IsTrue(formatter.CanWriteType(typeof(TestResource)));
        }

        [Test]
        public void FormatterWillWriteIEnumerable()
        {
            var formatter = new JsonApiMediaTypeFormatter();
            Assert.IsTrue(formatter.CanWriteType(typeof(IEnumerable<TestResource>)));
        }
    }
}
