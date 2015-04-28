using JsonApi.Profile;
using NUnit.Framework;

namespace JsonApi.Tests.Profile
{
    [TestFixture]
    public class DefaultInflectorTests
    {
        [Test]
        public void InflectorPluralizes()
        {
            var inflector = new DefaultInflector();
            Assert.AreEqual("bananas", inflector.Pluralize("banana"));
        }

        [Test]
        public void InflectorSingularizes()
        {
            var inflector = new DefaultInflector();
            Assert.AreEqual("banana", inflector.Singularize("bananas"));
        }
    }
}
