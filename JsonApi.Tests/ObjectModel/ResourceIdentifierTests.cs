using JsonApi.ObjectModel;
using NUnit.Framework;

namespace JsonApi.Tests.ObjectModel
{
    [TestFixture]
    public class ResourceIdentifierTests
    {
        [Test]
        public void ConstructorShouldSetProperties()
        {
            var identifier = new ResourceIdentifier("thing", "10");
            Assert.AreEqual("thing", identifier.Type);
            Assert.AreEqual("10", identifier.Id);
        }

        [Test]
        public void EqualsShouldBeEqual()
        {
            var identifier1 = new ResourceIdentifier("thing", "10");
            var identifier2 = new ResourceIdentifier("thing", "10");
            Assert.IsTrue(identifier1.Equals(identifier2));
        }

        [Test]
        public void HashCodeShouldBeEqual()
        {
            var identifier1 = new ResourceIdentifier("thing", "10");
            var identifier2 = new ResourceIdentifier("thing", "10");
            Assert.AreEqual(identifier1.GetHashCode(), identifier2.GetHashCode());
        }

        [Test]
        public void EqualsOpShouldBeEqual()
        {
            var identifier1 = new ResourceIdentifier("thing", "10");
            var identifier2 = new ResourceIdentifier("thing", "10");
            Assert.IsTrue(identifier1 == identifier2);
        }

        [Test]
        public void NotEqualsOpShouldBeInequal()
        {
            var identifier1 = new ResourceIdentifier("thing", "10");
            var identifier2 = new ResourceIdentifier("thing", "20");
            Assert.IsTrue(identifier1 != identifier2);
        }
    }
}
