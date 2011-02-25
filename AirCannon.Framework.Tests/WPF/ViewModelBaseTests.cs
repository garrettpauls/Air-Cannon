using System.Collections.Generic;
using AirCannon.Framework.Utilities;
using AirCannon.Framework.WPF;
using MbUnit.Framework;

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
        private readonly List<string> mChangedProperties = new List<string>();

        /// <summary>
        ///   Verifies <see cref = "OnBasePropertyChanged" /> is called correctly when a Model
        ///   property gets changed.
        /// </summary>
        [Test]
        public void OnBasePropertyChangedTest()
        {
            Model.TestProperty = "asdf";
            Assert.Count(1, mChangedProperties, "Exactly one property should have been changed");
            Assert.AreEqual(Property<ViewModelBaseTestsModel>.Name(p => p.TestProperty), mChangedProperties[0],
                            "The wrong property was called by OnBasePropertyChanged");
        }

        [SetUp]
        public void SetUp()
        {
            Model = new ViewModelBaseTestsModel();
            mChangedProperties.Clear();
        }

        #region Overrides of ViewModelBase<ViewModelBaseTests>

        /// <summary>
        ///   Called when the model has a property change.
        /// </summary>
        protected override void OnBasePropertyChanged(string propertyName)
        {
            mChangedProperties.Add(propertyName);
        }

        #endregion
    }
}