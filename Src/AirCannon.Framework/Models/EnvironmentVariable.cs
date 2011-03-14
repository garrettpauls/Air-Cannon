using AirCannon.Framework.WPF;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AirCannon.Framework.Models
{
    public class EnvironmentVariable : NotifyPropertyChangedBase
    {
        private string mKey;
        private string mValue;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "EnvironmentVariable" /> class.
        /// </summary>
        public EnvironmentVariable() : this(string.Empty, string.Empty)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "EnvironmentVariable" /> class.
        /// </summary>
        /// <param name = "key">The key.</param>
        /// <param name = "value">The value.</param>
        public EnvironmentVariable(string key, string value)
        {
            Key = key;
            Value = value;
        }

        /// <summary>
        ///   Gets or sets the key.
        /// </summary>
        public string Key
        {
            get { return mKey; }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }
                SetPropertyValue(ref mKey, value, () => Key);
            }
        }

        /// <summary>
        ///   Gets or sets the value.
        /// </summary>
        public string Value
        {
            get { return mValue; }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }
                SetPropertyValue(ref mValue, value, () => Value);
            }
        }

        /// <summary>
        ///   Determines whether the specified <see cref = "EnvironmentVariable" /> is equal to this instance.
        /// </summary>
        /// <param name = "other">The <see cref = "EnvironmentVariable" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref = "EnvironmentVariable" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(EnvironmentVariable other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Equals(other.mKey, mKey) && Equals(other.mValue, mValue);
        }

        /// <summary>
        ///   Determines whether the specified <see cref = "System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name = "obj">The <see cref = "System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref = "System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != typeof (EnvironmentVariable))
            {
                return false;
            }
            return Equals((EnvironmentVariable) obj);
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (mKey.GetHashCode()*397) ^ mValue.GetHashCode();
            }
        }
    }
}