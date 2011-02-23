using System;
using System.Collections.Generic;
using System.Linq;

namespace AirCannon.Framework.Models
{
    /// <summary>
    ///   Represents a collection of environment variables.
    /// </summary>
    public class EnvironmentVariableDictionary : Dictionary<string, string>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "EnvironmentVariableDictionary" /> class.
        /// </summary>
        public EnvironmentVariableDictionary()
            : this(new KeyValuePair<string, string>[] {})
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "EnvironmentVariableDictionary" /> class.
        /// </summary>
        /// <param name = "initialValues">The initial values to use to populate the collection.</param>
        public EnvironmentVariableDictionary(IEnumerable<KeyValuePair<string, string>> initialValues)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            foreach (var value in initialValues)
            {
                Add(value.Key, value.Value);
            }
        }

        /// <summary>
        ///   Updates the dictionary with the given <see cref = "KeyValuePair{TKey,TValue}" />s, 
        ///   overwriting any existing values and adding any new values.
        /// </summary>
        /// <param name = "environmentVariables">The environment variables to add to the dictionary.</param>
        /// <returns>The dictionary.</returns>
        public EnvironmentVariableDictionary UpdateWith(params KeyValuePair<string, string>[] environmentVariables)
        {
            return UpdateWith(environmentVariables.AsEnumerable());
        }

        /// <summary>
        ///   Updates the dictionary with the given <see cref = "KeyValuePair{TKey,TValue}" />s, 
        ///   overwriting any existing values and adding any new values.
        /// </summary>
        /// <param name = "environmentVariables">The environment variables to add to the dictionary.</param>
        /// <returns>The dictionary.</returns>
        public EnvironmentVariableDictionary UpdateWith(IEnumerable<KeyValuePair<string, string>> environmentVariables)
        {
            foreach (var variable in environmentVariables)
            {
                this[variable.Key] = variable.Value;
            }

            return this;
        }
    }
}