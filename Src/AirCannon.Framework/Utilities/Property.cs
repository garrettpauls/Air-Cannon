using System;
using System.Linq.Expressions;

namespace AirCannon.Framework.Utilities
{
    /// <summary>
    ///   A utility for determining the name of a property based on a member expression.
    /// </summary>
    public static class Property
    {
        /// <summary>
        ///   Gets the name of a property from a member expression.
        /// </summary>
        /// <example>
        ///   Property.Name(() => this.SomeProperty) = "SomeProperty"
        /// </example>
        /// <typeparam name = "TProperty">The return type of the property.</typeparam>
        /// <param name = "propertySelector">
        ///   A member expression used to determine the property name, in the format `() => Property`
        /// </param>
        /// <returns>The name of the property.</returns>
        public static string Name<TProperty>(Expression<Func<TProperty>> propertySelector)
        {
            return ((MemberExpression) propertySelector.Body).Member.Name;
        }
    }

    /// <summary>
    ///   A utility for determining the name of a property based on a member expression.
    /// </summary>
    /// <typeparam name = "TType">The type that contains the property.</typeparam>
    public static class Property<TType>
    {
        /// <summary>
        ///   Gets the name of a property from a member expression.
        /// </summary>
        /// <example>
        ///   Property.Name{SomeClass}(p => p.SomeProperty) = "SomeProperty"
        /// </example>
        /// <typeparam name = "TProperty">The return type of the property.</typeparam>
        /// <param name = "propertySelector">
        ///   A member expression used to determine the property name, in the format `p => p.Property`
        /// </param>
        /// <returns>The name of the property.</returns>
        public static string Name<TProperty>(Expression<Func<TType, TProperty>> propertySelector)
        {
            return ((MemberExpression) propertySelector.Body).Member.Name;
        }
    }
}