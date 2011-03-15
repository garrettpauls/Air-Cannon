using System.Collections.Generic;
using System.Linq;
using AirCannon.Framework.Utilities;
using AirCannon.Framework.WPF;
using NUnit.Framework;

namespace AirCannon.Framework.Tests.WPF
{
    /// <summary>
    ///   A test model for <see cref = "ViewModelBaseTests" />.
    /// </summary>
    public class ViewModelBaseTestsModel : NotifyPropertyChangedBase
    {
        private string mTestProperty;
        private string mUnusedProperty;

        /// <summary>
        ///   Gets or sets the test property.
        /// </summary>
        public string TestProperty
        {
            get { return mTestProperty; }
            set { SetPropertyValue(ref mTestProperty, value, () => TestProperty); }
        }

        /// <summary>
        ///   Not used by any tests, here to add the possibility of failure.
        /// </summary>
        public string UnusedProperty
        {
            get { return mUnusedProperty; }
            set { SetPropertyValue(ref mUnusedProperty, value, () => UnusedProperty); }
        }
    }

    /// <summary>
    ///   Verifies <see cref = "ViewModelBase{T}" /> works correctly.
    /// </summary>
    [TestFixture]
    public class ViewModelBaseTests : ViewModelBase<ViewModelBaseTestsModel>
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Model = new ViewModelBaseTestsModel();
            mChangedProperties.Clear();
        }

        #endregion

        private readonly List<string> mChangedProperties = new List<string>();

        /// <summary>
        ///   Gets the passthrough property names.
        /// </summary>
        protected override IEnumerable<string> PassthroughPropertyNames
        {
            get
            {
                return new[]
                           {
                               Property<ViewModelBaseTestsModel>.Name(p => p.TestProperty)
                           };
            }
        }

        /// <summary>
        ///   Called when the model has a property change.
        /// </summary>
        protected override void OnBasePropertyChanged(string propertyName)
        {
            mChangedProperties.Add(propertyName);
            base.OnBasePropertyChanged(propertyName);
        }

        /// <summary>
        ///   Verifies <see cref = "OnBasePropertyChanged" /> is called correctly when a Model
        ///   property gets changed.
        /// </summary>
        [Test]
        public void OnBasePropertyChangedTest()
        {
            Model.TestProperty = "asdf";
            Assert.That(mChangedProperties.Count, Is.EqualTo(1),
                        "Exactly one property should have been changed");
            Assert.AreEqual(Property<ViewModelBaseTestsModel>.Name(p => p.TestProperty), mChangedProperties[0],
                            "The wrong property was called by OnBasePropertyChanged");
        }

        /// <summary>
        ///   Verifies that properties in <see cref = "PassthroughPropertyNames" /> get passed
        ///   through as new property changed events and those that aren't included don't.
        /// </summary>
        [Test]
        public void PassthroughPropertyNamesTest()
        {
            string passthroughedProperty = Property<ViewModelBaseTestsModel>.Name(p => p.TestProperty);
            Model.TestProperty = string.Empty;
            Model.UnusedProperty = string.Empty;

            var propertyChangedEvents = new List<string>();
            PropertyChanged += (sender, e) => propertyChangedEvents.Add(e.PropertyName);

            Assert.That(PassthroughPropertyNames.Count(), Is.EqualTo(1),
                        "PassthroughPropertyNames should only include TestProperty");
            Assert.AreEqual(passthroughedProperty, PassthroughPropertyNames.First(),
                            "PassthroughPropertyNames should only include TestProperty");

            Model.TestProperty = "asdf";
            Model.UnusedProperty = "lll";

            Assert.That(propertyChangedEvents.Count, Is.EqualTo(1),
                        "Only TestProperty should have been passed through");
            Assert.AreEqual(passthroughedProperty, propertyChangedEvents[0],
                            "Only TestProperty should have been passed through as a property changed event");
        }
    }
}