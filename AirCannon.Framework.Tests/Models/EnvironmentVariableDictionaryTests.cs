using System.Collections.Generic;
using System.Collections.Specialized;
using AirCannon.Framework.Models;
using AirCannon.Framework.Tests.Utilities;
using MbUnit.Framework;

namespace AirCannon.Framework.Tests.Models
{
    /// <summary>
    ///   Verifies that <see cref = "EnvironmentVariableDictionary" /> works correctly.
    /// </summary>
    [TestFixture]
    public class EnvironmentVariableDictionaryTests
    {
        private readonly List<NotifyCollectionChangedEventArgs> mCollectionChangedEvents =
            new List<NotifyCollectionChangedEventArgs>();

        private EnvironmentVariableDictionary mDictionary = new EnvironmentVariableDictionary();

        /// <summary>
        ///   Verifies the <see cref = "EnvironmentVariableDictionary.CollectionChanged" /> event
        ///   is called when items are added.
        /// </summary>
        [Test]
        public void CollectionChangedAddTest()
        {
            var kvp = new KeyValuePair<string, string>("a", "b");
            mDictionary.Add(kvp);
            Assert.Count(1, mCollectionChangedEvents, "CollectionChanged should have been fired once");
            var e = mCollectionChangedEvents[0];
            Assert.AreEqual(NotifyCollectionChangedAction.Add, e.Action);
            Assert.Count(1, e.NewItems, "One item should have been added");
            Assert.AreEqual(kvp, e.NewItems[0]);
            Assert.IsNull(e.OldItems, "There should be no old items");

            mDictionary.Clear();
            mCollectionChangedEvents.Clear();

            mDictionary.Add(kvp.Key, kvp.Value);
            Assert.Count(1, mCollectionChangedEvents, "CollectionChanged should have been fired once");
            e = mCollectionChangedEvents[0];
            Assert.AreEqual(NotifyCollectionChangedAction.Add, e.Action);
            Assert.Count(1, e.NewItems, "One item should have been added");
            Assert.AreEqual(kvp, e.NewItems[0]);
            Assert.IsNull(e.OldItems, "There should be no old items");

            mDictionary.Clear();
            mCollectionChangedEvents.Clear();

            mDictionary[kvp.Key] = kvp.Value;
            Assert.Count(1, mCollectionChangedEvents, "CollectionChanged should have been fired once");
            e = mCollectionChangedEvents[0];
            Assert.AreEqual(NotifyCollectionChangedAction.Add, e.Action);
            Assert.Count(1, e.NewItems, "One item should have been added");
            Assert.AreEqual(kvp, e.NewItems[0]);
            Assert.IsNull(e.OldItems, "There should be no old items");
        }

        /// <summary>
        ///   Verifies the <see cref = "EnvironmentVariableDictionary.CollectionChanged" /> event
        ///   is called when items are removed.
        /// </summary>
        [Test]
        public void CollectionChangedRemoveTest()
        {
            var kvp = new KeyValuePair<string, string>("a", "b");
            mDictionary.Add(kvp);
            mCollectionChangedEvents.Clear();

            mDictionary.Remove(kvp);
            Assert.Count(1, mCollectionChangedEvents, "CollectionChanged should have been fired once");
            var e = mCollectionChangedEvents[0];

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, e.Action);
            Assert.Count(1, e.OldItems, "One old item should have been removed");
            Assert.AreEqual(kvp, e.OldItems[0]);

            mDictionary.Add(kvp);
            mCollectionChangedEvents.Clear();


            mDictionary.Remove(kvp.Key);
            Assert.Count(1, mCollectionChangedEvents, "CollectionChanged should have been fired once");
            e = mCollectionChangedEvents[0];

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, e.Action);
            Assert.Count(1, e.OldItems, "One old item should have been removed");
            Assert.AreEqual(kvp, e.OldItems[0]);
        }

        /// <summary>
        ///   Verifies the <see cref = "EnvironmentVariableDictionary.CollectionChanged" /> event
        ///   is called when items are replaced.
        /// </summary>
        [Test]
        public void CollectionChangedReplaceTest()
        {
            const string KEY = "a";
            const string VALUE1 = "a";
            const string VALUE2 = "b";

            mDictionary[KEY] = VALUE1;
            mCollectionChangedEvents.Clear();

            mDictionary[KEY] = VALUE2;
            Assert.Count(1, mCollectionChangedEvents, "CollectionChanged should have been fired once");
            var e = mCollectionChangedEvents[0];

            Assert.AreEqual(NotifyCollectionChangedAction.Replace, e.Action);
            Assert.Count(1, e.OldItems, "One old item should have been replaced");
            Assert.AreEqual(KEY, ((KeyValuePair<string, string>) e.OldItems[0]).Key);
            Assert.AreEqual(VALUE1, ((KeyValuePair<string, string>) e.OldItems[0]).Value);

            Assert.Count(1, e.NewItems, "One new item should have been added");
            Assert.AreEqual(KEY, ((KeyValuePair<string, string>) e.NewItems[0]).Key);
            Assert.AreEqual(VALUE2, ((KeyValuePair<string, string>) e.NewItems[0]).Value);
        }

        /// <summary>
        ///   Verifies the <see cref = "EnvironmentVariableDictionary.CollectionChanged" /> event
        ///   is called when the collection is cleared.
        /// </summary>
        [Test]
        public void CollectionChangedResetTest()
        {
            var kvp1 = new KeyValuePair<string, string>("a", "b");
            var kvp2 = new KeyValuePair<string, string>("d", "e");
            mDictionary.Add(kvp1);
            mDictionary.Add(kvp2);
            mCollectionChangedEvents.Clear();

            mDictionary.Clear();
            Assert.Count(1, mCollectionChangedEvents, "CollectionChanged should have been fired once");
            Assert.AreEqual(NotifyCollectionChangedAction.Reset, mCollectionChangedEvents[0].Action);
        }

        /// <summary>
        ///   Verifies that <see cref = "EnvironmentVariableDictionary" /> compare equality correctly.
        /// </summary>
        [Test]
        public void EqualityTest()
        {
            const string KEY1 = "KEY1";
            const string KEY2 = "KEY2";
            const string KEY3 = "KEY3";

            var other = new EnvironmentVariableDictionary();

            Assert.AreEqual(mDictionary, other, "Empty dictionaries should be equal");

            mDictionary[KEY1] = KEY1;
            mDictionary[KEY2] = KEY2;
            other[KEY1] = KEY1;
            other[KEY2] = KEY2;

            Assert.AreEqual(mDictionary, other, "Dictionaries with the same key-value pairs should be equal");

            other[KEY1] = KEY2;
            Assert.AreNotEqual(mDictionary, other, "Dictionaries with different key-value pairs should not be equal");

            other[KEY1] = KEY1;
            mDictionary[KEY3] = KEY3;
            Assert.AreNotEqual(mDictionary, other, "Dictionaries with different keys should not be equal");
        }

        /// <summary>
        ///   Sets up each test with a new dictionary and clear list of events.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            mDictionary = new EnvironmentVariableDictionary();
            mCollectionChangedEvents.Clear();
            mDictionary.CollectionChanged += _HandleCollectionChanged;
        }

        /// <summary>
        ///   Verifies that <see cref = "EnvironmentVariableDictionary.UpdateWith" /> works correctly.
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

            mDictionary[KEY1] = VALUE1;
            mDictionary[KEY2] = VALUE2;

            Assert2.ContainsKeyAndValue(mDictionary, KEY1, VALUE1);
            Assert2.ContainsKeyAndValue(mDictionary, KEY2, VALUE2);
            Assert.DoesNotContainKey(mDictionary, KEY3);

            var result = mDictionary.UpdateWith(new[]
                                                    {
                                                        new KeyValuePair<string, string>(KEY2, KEY2),
                                                        new KeyValuePair<string, string>(KEY3, VALUE3)
                                                    });

            Assert2.ContainsKeyAndValue(mDictionary, KEY1, VALUE1);
            Assert2.ContainsKeyAndValue(mDictionary, KEY2, KEY2);
            Assert2.ContainsKeyAndValue(mDictionary, KEY3, VALUE3);

            Assert.AreSame(mDictionary, result, "UpdateWith should return the updated dictionary");
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
    }
}