using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AirCannon.Framework.Tests.Utilities
{
    /// <summary>
    ///   Verifies the methods on <see cref = "Assert2" /> work correctly.
    /// </summary>
    [TestFixture]
    public class Assert2Tests
    {
        /// <summary>
        ///   Verifies that <see cref = "Assert2.ContainsKeyAndValue{TKey,TValue}" /> throws the 
        ///   correct exception when the expected key is missing.
        /// </summary>
        [Test, ExpectedException(typeof (AssertionException))]
        public void ContainsKeyAndValueKeyMissingTest()
        {
            var dictionary = new Dictionary<string, string> {{"A", "B"}};

            Assert2.ContainsKeyAndValue(dictionary, "B", "B");
        }

        /// <summary>
        ///   Verifies that <see cref = "Assert2.ContainsKeyAndValue{TKey,TValue}" /> does not 
        ///   throw an exception when the key and value are in the dictionary.
        /// </summary>
        [Test]
        public void ContainsKeyAndValueSuccessfulTest()
        {
            var dictionary = new Dictionary<string, string> {{"A", "B"}};

            Assert2.ContainsKeyAndValue(dictionary, "A", "B");
        }

        /// <summary>
        ///   Verifies that <see cref = "Assert2.ContainsKeyAndValue{TKey,TValue}" /> throws the
        ///   correct exception when the expected value is wrong.
        /// </summary>
        [Test, ExpectedException(typeof (AssertionException))]
        public void ContainsKeyAndValueValueWrongTest()
        {
            var dictionary = new Dictionary<string, string> {{"A", "A"}};

            Assert2.ContainsKeyAndValue(dictionary, "A", "B");
        }
    }
}