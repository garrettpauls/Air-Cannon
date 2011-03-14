using AirCannon.Framework.Utilities;
using MbUnit.Framework;

namespace AirCannon.Framework.Tests.Utilities
{
    /// <summary>
    ///   Verifies that the <see cref = "Property" /> utility can correctly determine property names.
    /// </summary>
    [TestFixture]
    public class PropertyTests
    {
        private const string TEST_PROPERTY_NAME = "TestProperty";

        /// <summary>
        ///   Gets or sets the test property.
        /// </summary>
        public string TestProperty { get; set; }

        /// <summary>
        ///   Verifies that <see cref = "Property{TType}.Name{TProperty}" /> gets property names correctly.
        /// </summary>
        [Test]
        public void GenericNameTest()
        {
            string propertyName = Property<PropertyTests>.Name(p => p.TestProperty);

            Assert.AreEqual(TEST_PROPERTY_NAME, propertyName);
        }

        /// <summary>
        ///   Verifies that <see cref = "Property.Name{TProperty}" /> gets property names correctly.
        /// </summary>
        [Test]
        public void InlineNameTest()
        {
            string propertyName = Property.Name(() => TestProperty);

            Assert.AreEqual(TEST_PROPERTY_NAME, propertyName);
        }
    }
}