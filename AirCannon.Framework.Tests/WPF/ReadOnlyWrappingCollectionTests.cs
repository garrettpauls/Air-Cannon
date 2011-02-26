using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using AirCannon.Framework.WPF;
using MbUnit.Framework;

namespace AirCannon.Framework.Tests.WPF
{
    /// <summary>
    ///   A test base class for <see cref = "ReadOnlyWrappingCollectionTests" />
    /// </summary>
    public class TestBase
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "TestBase" /> class.
        /// </summary>
        /// <param name = "number">The number.</param>
        public TestBase(int number)
        {
            Number = number;
        }

        public int Number { get; private set; }

        public bool Equals(TestBase other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Number == Number;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (TestBase)) return false;
            return Equals((TestBase) obj);
        }

        public override int GetHashCode()
        {
            return Number;
        }
    }

    /// <summary>
    ///   A wrapper for <see cref = "TestBase" />
    /// </summary>
    public class TestWrapper
    {
        private readonly TestBase mBase;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "TestWrapper" /> class.
        /// </summary>
        /// <param name = "base">The @base.</param>
        public TestWrapper(TestBase @base)
        {
            mBase = @base;
        }

        public TestBase Base
        {
            get { return mBase; }
        }

        public bool Equals(TestWrapper other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.mBase, mBase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (TestWrapper)) return false;
            return Equals((TestWrapper) obj);
        }

        public override int GetHashCode()
        {
            return (mBase != null ? mBase.GetHashCode() : 0);
        }
    }

    /// <summary>
    ///   Tests for <see cref = "ReadOnlyWrappingCollectionTests" />
    /// </summary>
    [TestFixture]
    public class ReadOnlyWrappingCollectionTests
    {
        private ObservableCollection<TestBase> mBackingCollection;
        private List<NotifyCollectionChangedEventArgs> mCollectionChangedEvents;
        private ReadOnlyWrappingCollection<TestWrapper, TestBase> mWrappingCollection;

        /// <summary>
        ///   Verifies <see cref = "ReadOnlyWrappingCollectionTests" /> handles adding to the backing collection correctly.
        /// </summary>
        [Test]
        public void AddTest()
        {
            var item1 = new TestBase(1);
            var item2 = new TestBase(2);

            mBackingCollection.Add(item1);
            mBackingCollection.Add(item2);

            _AssertBackingAndWrappedAreSynced();

            Assert.Count(2, mCollectionChangedEvents);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, mCollectionChangedEvents[0].Action);
            Assert.Count(1, mCollectionChangedEvents[0].NewItems);
            Assert.AreEqual(_Wrap(item1), mCollectionChangedEvents[0].NewItems[0]);
            Assert.IsNull(mCollectionChangedEvents[0].OldItems);

            Assert.AreEqual(NotifyCollectionChangedAction.Add, mCollectionChangedEvents[1].Action);
            Assert.Count(1, mCollectionChangedEvents[1].NewItems);
            Assert.AreEqual(_Wrap(item2), mCollectionChangedEvents[1].NewItems[0]);
            Assert.IsNull(mCollectionChangedEvents[1].OldItems);
        }

        /// <summary>
        ///   Verifies <see cref = "ReadOnlyWrappingCollectionTests" /> handles moving items in the backing collection correctly.
        /// </summary>
        [Test]
        public void MoveTest()
        {
            var item1 = new TestBase(1);
            var item2 = new TestBase(2);
            var item3 = new TestBase(3);

            _Setup(item1, item2, item3);

            mBackingCollection.Move(1, 2);

            Assert.AreElementsEqual(new[] {item1, item3, item2}, mBackingCollection);

            _AssertBackingAndWrappedAreSynced();

            Assert.Count(1, mCollectionChangedEvents);
            Assert.AreEqual(NotifyCollectionChangedAction.Move, mCollectionChangedEvents[0].Action);
            Assert.Count(1, mCollectionChangedEvents[0].OldItems);
            Assert.AreEqual(_Wrap(item2), mCollectionChangedEvents[0].OldItems[0]);
            Assert.Count(1, mCollectionChangedEvents[0].NewItems);
            Assert.AreEqual(_Wrap(item2), mCollectionChangedEvents[0].NewItems[0]);
            Assert.AreEqual(1, mCollectionChangedEvents[0].OldStartingIndex);
            Assert.AreEqual(2, mCollectionChangedEvents[0].NewStartingIndex);
        }

        /// <summary>
        ///   Verifies <see cref = "ReadOnlyWrappingCollectionTests" /> handles removal of items from the backing collection correctly.
        /// </summary>
        [Test]
        public void RemoveTest()
        {
            var item1 = new TestBase(1);
            var item2 = new TestBase(2);
            var item3 = new TestBase(3);

            _Setup(item1, item2, item3);

            mBackingCollection.Remove(item2);

            Assert.Count(2, mBackingCollection);
            Assert.AreSame(item1, mBackingCollection[0]);
            Assert.AreSame(item3, mBackingCollection[1]);

            _AssertBackingAndWrappedAreSynced();

            Assert.Count(1, mCollectionChangedEvents);
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, mCollectionChangedEvents[0].Action);
            Assert.Count(1, mCollectionChangedEvents[0].OldItems);
            Assert.AreEqual(_Wrap(item2), mCollectionChangedEvents[0].OldItems[0]);
            Assert.IsNull(mCollectionChangedEvents[0].NewItems);
        }

        /// <summary>
        ///   Verifies <see cref = "ReadOnlyWrappingCollectionTests" /> handles replacing items in the backing collection correctly.
        /// </summary>
        [Test]
        public void ReplaceTest()
        {
            var item1 = new TestBase(1);
            var item2 = new TestBase(2);
            var item3 = new TestBase(3);

            _Setup(item1, item2, item3);

            mBackingCollection[0] = item2;

            Assert.AreElementsEqual(new[] {item2, item2, item3}, mBackingCollection);

            _AssertBackingAndWrappedAreSynced();

            Assert.Count(1, mCollectionChangedEvents);
            Assert.AreEqual(NotifyCollectionChangedAction.Replace, mCollectionChangedEvents[0].Action);
            Assert.Count(1, mCollectionChangedEvents[0].OldItems);
            Assert.AreEqual(_Wrap(item1), mCollectionChangedEvents[0].OldItems[0]);
            Assert.Count(1, mCollectionChangedEvents[0].NewItems);
            Assert.AreEqual(_Wrap(item2), mCollectionChangedEvents[0].NewItems[0]);
        }

        /// <summary>
        ///   Verifies <see cref = "ReadOnlyWrappingCollectionTests" /> handles clearing the backing collection correctly.
        /// </summary>
        [Test]
        public void ResetTest()
        {
            var item1 = new TestBase(1);
            var item2 = new TestBase(2);
            var item3 = new TestBase(3);

            _Setup(item1, item2, item3);

            mBackingCollection.Clear();

            Assert.Count(0, mBackingCollection);

            _AssertBackingAndWrappedAreSynced();

            Assert.Count(1, mCollectionChangedEvents);
            Assert.AreEqual(NotifyCollectionChangedAction.Reset, mCollectionChangedEvents[0].Action);
            Assert.IsNull(mCollectionChangedEvents[0].OldItems);
            Assert.IsNull(mCollectionChangedEvents[0].NewItems);
        }

        /// <summary>
        ///   Setup before each test is run.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _Setup();
        }

        /// <summary>
        ///   Verifies the wrapping collection is correctly synced with the backing collection.
        /// </summary>
        private void _AssertBackingAndWrappedAreSynced()
        {
            Assert.AreEqual(mBackingCollection.Count, mWrappingCollection.Count);
            Assert.AreElementsEqual(mBackingCollection, mWrappingCollection.Select(_Unwrap));
        }


        /// <summary>
        ///   Handles the <see cref = "INotifyCollectionChanged.CollectionChanged" /> event of the wrapping collection
        ///   and logs any events.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        private void _HandleWrappingCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            mCollectionChangedEvents.Add(e);
        }

        /// <summary>
        ///   Creates a fresh unit test setup.
        /// </summary>
        /// <param name = "initialItems">Any initial items to be added to the backing collection.</param>
        private void _Setup(params TestBase[] initialItems)
        {
            mBackingCollection = new ObservableCollection<TestBase>(initialItems);
            mWrappingCollection =
                new ReadOnlyWrappingCollection<TestWrapper, TestBase>(
                    mBackingCollection, _Wrap, _Unwrap);
            mCollectionChangedEvents = new List<NotifyCollectionChangedEventArgs>();

            mWrappingCollection.CollectionChanged += _HandleWrappingCollectionChanged;

            _AssertBackingAndWrappedAreSynced();
        }

        /// <summary>
        ///   Unwraps a <see cref = "TestWrapper" /> into a <see cref = "TestBase" />.
        /// </summary>
        private static TestBase _Unwrap(TestWrapper arg)
        {
            return arg.Base;
        }

        /// <summary>
        ///   Wraps a <see cref = "TestBase" /> into a <see cref = "TestWrapper" />.
        /// </summary>
        private static TestWrapper _Wrap(TestBase arg)
        {
            return new TestWrapper(arg);
        }
    }
}