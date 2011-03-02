using System.Collections.Generic;
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
        /// <summary>
        ///   Verifies that <see cref = "EnvironmentVariableDictionary" /> compare equality correctly.
        /// </summary>
        [Test]
        public void EqualityTest()
        {
            const string KEY1 = "KEY1";
            const string KEY2 = "KEY2";
            const string KEY3 = "KEY3";

            var dictionary = new EnvironmentVariableDictionary();
            var other = new EnvironmentVariableDictionary();

            Assert.AreEqual(dictionary, other, "Empty dictionaries should be equal");

            dictionary[KEY1] = KEY1;
            dictionary[KEY2] = KEY2;
            other[KEY1] = KEY1;
            other[KEY2] = KEY2;

            Assert.AreEqual(dictionary, other, "Dictionaries with the same key-value pairs should be equal");

            other[KEY1] = KEY2;
            Assert.AreNotEqual(dictionary, other, "Dictionaries with different key-value pairs should not be equal");

            other[KEY1] = KEY1;
            dictionary[KEY3] = KEY3;
            Assert.AreNotEqual(dictionary, other, "Dictionaries with different keys should not be equal");
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

            var envVars = new EnvironmentVariableDictionary
                              {
                                  {KEY1, VALUE1},
                                  {KEY2, VALUE2},
                              };

            Assert2.ContainsKeyAndValue(envVars, KEY1, VALUE1);
            Assert2.ContainsKeyAndValue(envVars, KEY2, VALUE2);
            Assert.DoesNotContainKey(envVars, KEY3);

            var result = envVars.UpdateWith(new[]
                                                {
                                                    new KeyValuePair<string, string>(KEY2, KEY2),
                                                    new KeyValuePair<string, string>(KEY3, VALUE3)
                                                });

            Assert2.ContainsKeyAndValue(envVars, KEY1, VALUE1);
            Assert2.ContainsKeyAndValue(envVars, KEY2, KEY2);
            Assert2.ContainsKeyAndValue(envVars, KEY3, VALUE3);

            Assert.AreSame(envVars, result, "UpdateWith should return the updated dictionary");
        }
    }
}