using System.Collections.Generic;
using System.ComponentModel;
using AirCannon.Framework.WPF;
using MbUnit.Framework;

namespace AirCannon.Framework.Tests.WPF
{
    /// <summary>
    ///   Tests for <see cref = "NotifyPropertyChangedBase" />.
    /// </summary>
    [TestFixture]
    public class NotifyPropertyChangedBaseTests : NotifyPropertyChangedBase
    {
        private const string TEST_PROPERTY_NAME = "TestProperty";
        private readonly List<string> mChangedProperties = new List<string>();

        private int mOtherTestProperty;

        private string mTestProperty;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "NotifyPropertyChangedBaseTests" /> class.
        /// </summary>
        public NotifyPropertyChangedBaseTests()
        {
            PropertyChanged += _HandlePropertyChanged;
        }

        public int OtherTestProperty
        {
            get { return mOtherTestProperty; }
            set { SetPropertyValue(ref mOtherTestProperty, value, () => OtherTestProperty); }
        }

        public string TestProperty
        {
            get { return mTestProperty; }
            set { SetPropertyValue(ref mTestProperty, value, () => TestProperty); }
        }

        /// <summary>
        ///   Verifies that the <see cref = "NotifyPropertyChangedBase.RaisePropertyChanged{TProp}" /> 
        ///   method works correctly.
        /// </summary>
        [Test]
        public void RaisePropertyChangedTest()
        {
            RaisePropertyChanged(() => TestProperty);
            RaisePropertyChanged(() => OtherTestProperty);

            Assert.Count(2, mChangedProperties, "Two properties should have been changed");
            Assert.AreEqual(TEST_PROPERTY_NAME, mChangedProperties[0], "{0} should have been raised first",
                            TEST_PROPERTY_NAME);
            Assert.AreEqual("OtherTestProperty", mChangedProperties[1],
                            "OtherTestProperty should have been raised second");
        }

        /// <summary>
        ///   Verifies that the <see cref = "NotifyPropertyChangedBase.SetPropertyValue{TProp}" /> 
        ///   method works correctly.
        /// </summary>
        [Test]
        public void SetPropertyValueTest()
        {
            const string NEW_VALUE = "test";

            bool result = SetPropertyValue(ref mTestProperty, NEW_VALUE, () => TestProperty);
            Assert.IsTrue(result, "{0} should have been changed", TEST_PROPERTY_NAME);
            Assert.Count(1, mChangedProperties, "One property should have been changed");
            Assert.AreEqual(TEST_PROPERTY_NAME, mChangedProperties[0],
                            "{0} should have been changed", TEST_PROPERTY_NAME);

            mChangedProperties.Clear();

            result = SetPropertyValue(ref mTestProperty, NEW_VALUE, () => TestProperty);
            Assert.IsFalse(result, "{0} should not have changed", TEST_PROPERTY_NAME);
            Assert.Count(0, mChangedProperties, "No properties should have been changed");
        }

        /// <summary>
        ///   Resets all the class fields before each test is run.
        /// </summary>
        [SetUp]
        public void TestSetup()
        {
            mChangedProperties.Clear();
            Assert.Count(0, mChangedProperties, "Failed to reset changed properties list");

            mTestProperty = string.Empty;
            mOtherTestProperty = 0;
        }

        /// <summary>
        ///   Adds the changed property to the list of changed properties.
        /// </summary>
        private void _HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            mChangedProperties.Add(e.PropertyName);
        }
    }
}