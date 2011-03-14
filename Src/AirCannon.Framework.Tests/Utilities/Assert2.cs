using System.Collections.Generic;
using MbUnit.Framework;

namespace AirCannon.Framework.Tests.Utilities
{
    /// <summary>
    ///   Additional methods for <see cref = "Assert" />
    /// </summary>
    public static class Assert2
    {
        /// <summary>
        ///   Determines whether the given dictionary contains a pair with the 
        ///   <see cref = "expectedKey" /> and <see cref = "expectedValue" />.
        /// </summary>
        /// <typeparam name = "TKey">The type of the key.</typeparam>
        /// <typeparam name = "TValue">The type of the value.</typeparam>
        /// <param name = "dictionary">The dictionary.</param>
        /// <param name = "expectedKey">The expected key.</param>
        /// <param name = "expectedValue">The expected value.</param>
        public static void ContainsKeyAndValue<TKey, TValue>(
            IDictionary<TKey, TValue> dictionary, TKey expectedKey, TValue expectedValue)
        {
            Assert.ContainsKey(dictionary, expectedKey);
            Assert.AreEqual(expectedValue, dictionary[expectedKey],
                            "Expected {0} for value of key {1}",
                            expectedValue, expectedKey);
        }
    }
}