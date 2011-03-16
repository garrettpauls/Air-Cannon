using System.Collections.Generic;
using System.Collections.Specialized;
using AirCannon.Framework.Models;
using AirCannon.Framework.Utilities;
using NUnit.Framework;

namespace AirCannon.Framework.Tests.Models
{
    /// <summary>
    ///   Verifies that <see cref = "EnvironmentVariableCollection" /> works correctly.
    /// </summary>
    [TestFixture]
    public class EnvironmentVariableDictionaryTests
    {
        #region Setup/Teardown

        /// <summary>
        ///   Sets up each test with a new dictionary and clear list of events.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            mEnvVars = new EnvironmentVariableCollection();
            mCollectionChangedEvents.Clear();
            mEnvVars.CollectionChanged += _HandleCollectionChanged;
        }

        #endregion

        private readonly List<NotifyCollectionChangedEventArgs> mCollectionChangedEvents =
            new List<NotifyCollectionChangedEventArgs>();

        private EnvironmentVariableCollection mEnvVars = new EnvironmentVariableCollection();

        private void _AssertContainsKeyAndValue(EnvironmentVariableCollection envVars, string key1, string value)
        {
        }

        /// <summary>
        ///   Handles the CollectionChanged event of the dictionary.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        private void _HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            mCollectionChangedEvents.Add(e);
        }

        /// <summary>
        ///   Verifies the <see cref = "EnvironmentVariableCollection.CollectionChanged" /> event
        ///   is called when items are added.
        /// </summary>
        [Test]
        public void CollectionChangedAddTest()
        {
            var kvp = new EnvironmentVariable("a", "b");
            mEnvVars.Add(kvp);
            Assert.That(mCollectionChangedEvents.Count, Is.EqualTo(1),
                        "CollectionChanged should have been fired once");
            var e = mCollectionChangedEvents[0];
            Assert.AreEqual(NotifyCollectionChangedAction.Add, e.Action);
            Assert.That(e.NewItems.Count, Is.EqualTo(1), "One item should have been added");
            Assert.AreEqual(kvp, e.NewItems[0]);
            Assert.IsNull(e.OldItems, "There should be no old items");

            mEnvVars.Clear();
            mCollectionChangedEvents.Clear();

            mEnvVars[kvp.Key] = kvp.Value;
            Assert.That(mCollectionChangedEvents.Count, Is.EqualTo(1),
                        "CollectionChanged should have been fired once");
            e = mCollectionChangedEvents[0];
            Assert.AreEqual(NotifyCollectionChangedAction.Add, e.Action);
            Assert.That(e.NewItems.Count, Is.EqualTo(1), "One item should have been added");
            Assert.AreEqual(kvp, e.NewItems[0]);
            Assert.IsNull(e.OldItems, "There should be no old items");
        }

        /// <summary>
        ///   Verifies the <see cref = "EnvironmentVariableCollection.CollectionChanged" /> event
        ///   is called when items are removed.
        /// </summary>
        [Test]
        public void CollectionChangedRemoveTest()
        {
            var kvp = new EnvironmentVariable("a", "b");
            mEnvVars.Add(kvp);
            mCollectionChangedEvents.Clear();

            mEnvVars.Remove(kvp);
            Assert.That(mCollectionChangedEvents.Count, Is.EqualTo(1),
                        "CollectionChanged should have been fired once");
            var e = mCollectionChangedEvents[0];

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, e.Action);
            Assert.That(e.OldItems.Count, Is.EqualTo(1), "One old item should have been removed");
            Assert.AreEqual(kvp, e.OldItems[0]);
        }

        /// <summary>
        ///   Verifies the <see cref = "EnvironmentVariableCollection.CollectionChanged" /> event
        ///   is called when the collection is cleared.
        /// </summary>
        [Test]
        public void CollectionChangedResetTest()
        {
            var kvp1 = new EnvironmentVariable("a", "b");
            var kvp2 = new EnvironmentVariable("d", "e");
            mEnvVars.Add(kvp1);
            mEnvVars.Add(kvp2);
            mCollectionChangedEvents.Clear();

            mEnvVars.Clear();
            Assert.That(mCollectionChangedEvents.Count, Is.EqualTo(1),
                        "CollectionChanged should have been fired once");
            Assert.AreEqual(NotifyCollectionChangedAction.Reset, mCollectionChangedEvents[0].Action);
        }

        /// <summary>
        ///   Verifies that <see cref = "EnvironmentVariableCollection" /> compare equality correctly.
        /// </summary>
        [Test]
        public void EqualityTest()
        {
            const string KEY1 = "KEY1";
            const string KEY2 = "KEY2";
            const string KEY3 = "KEY3";

            var other = new EnvironmentVariableCollection();

            Assert.AreEqual(mEnvVars, other, "Empty dictionaries should be equal");

            mEnvVars[KEY1] = KEY1;
            mEnvVars[KEY2] = KEY2;
            other[KEY1] = KEY1;
            other[KEY2] = KEY2;

            Assert.AreEqual(mEnvVars, other, "Dictionaries with the same key-value pairs should be equal");

            other[KEY1] = KEY2;
            Assert.AreNotEqual(mEnvVars, other, "Dictionaries with different key-value pairs should not be equal");

            other[KEY1] = KEY1;
            mEnvVars[KEY3] = KEY3;
            Assert.AreNotEqual(mEnvVars, other, "Dictionaries with different keys should not be equal");
        }

        /// <summary>
        ///   Verifies the <see cref = "EnvironmentVariableCollection.ItemChanged" /> event
        ///   is called when items are replaced.
        /// </summary>
        [Test]
        public void ItemChangedTest()
        {
            const string KEY = "a";
            const string VALUE1 = "a";
            const string VALUE2 = "b";

            var itemChangedEvents = new List<ItemChangedEventArgs<EnvironmentVariable>>();
            mEnvVars.ItemChanged += (sender, e) => itemChangedEvents.Add(e);

            mEnvVars[KEY] = VALUE1;
            mCollectionChangedEvents.Clear();

            mEnvVars[KEY] = VALUE2;
            Assert.That(itemChangedEvents.Count, Is.EqualTo(1),
                        "CollectionChanged should have been fired once");
            var evt = itemChangedEvents[0];

            Assert.AreEqual(KEY, evt.Item.Key);
            Assert.AreEqual(VALUE2, evt.Item.Value);
            Assert.AreEqual(Property<EnvironmentVariable>.Name(p => p.Value), evt.PropertyName);
        }

        /// <summary>
        ///   Verifies that <see cref = "EnvironmentVariableCollection.UpdateWith" /> works correctly.
        /// </summary>
        [Test]
        public void UpdateWithTest()
        {
            const string KEY1 = "key1";
            const string VALUE1 = "value1";
            const string KEY2 = "key2";
            const string VALUE2 = "value2";
            const string KEY3 = "key3";
            const string VALUE3 = "value3";

            mEnvVars[KEY1] = VALUE1;
            mEnvVars[KEY2] = VALUE2;


            Assert.That(mEnvVars, Contains.Item(new EnvironmentVariable(KEY1, VALUE1)));
            Assert.That(mEnvVars, Contains.Item(new EnvironmentVariable(KEY2, VALUE2)));
            Assert.IsFalse(mEnvVars.ContainsKey(KEY3));

            var result = mEnvVars.UpdateWith(new[]
                                                 {
                                                     new EnvironmentVariable(KEY2, KEY2),
                                                     new EnvironmentVariable(KEY3, VALUE3)
                                                 });

            Assert.That(mEnvVars, Contains.Item(new EnvironmentVariable(KEY1, VALUE1)));
            Assert.That(mEnvVars, Contains.Item(new EnvironmentVariable(KEY2, KEY2)));
            Assert.That(mEnvVars, Contains.Item(new EnvironmentVariable(KEY3, VALUE3)));

            Assert.AreSame(mEnvVars, result, "UpdateWith should return the updated dictionary");
        }
    }
}