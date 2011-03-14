using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace AirCannon.Framework.WPF
{
    /// <summary>
    ///   A mapping between a <see cref = "Type" /> and a <see cref = "Style" />.
    /// </summary>
    [ContentProperty("Style")]
    public class TypeStylePair
    {
        /// <summary>
        ///   Gets or sets the style.
        /// </summary>
        public Style Style { get; set; }

        /// <summary>
        ///   Gets or sets the type.
        /// </summary>
        public Type Type { get; set; }
    }

    /// <summary>
    ///   An implementation of <see cref = "StyleSelector" /> that allows styles to be selected based
    ///   on data context type.
    /// </summary>
    public class TypedStyleSelector : StyleSelector
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "TypedStyleSelector" /> class.
        /// </summary>
        public TypedStyleSelector()
        {
            Styles = new ObservableCollection<TypeStylePair>();
            DefaultStyle = new Style();
        }

        /// <summary>
        ///   Gets or sets the default style.
        /// </summary>
        public Style DefaultStyle { get; set; }

        /// <summary>
        ///   Gets or sets the collection of styles to use.
        /// </summary>
        public ObservableCollection<TypeStylePair> Styles { get; set; }

        /// <summary>
        ///   When overridden in a derived class, returns a <see cref = "T:System.Windows.Style" /> based on custom logic.
        /// </summary>
        /// <param name = "item">The content.</param>
        /// <param name = "container">The element to which the style will be applied.</param>
        /// <returns>
        ///   Returns an application-specific style to apply; otherwise, null.
        /// </returns>
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item == null)
            {
                return DefaultStyle;
            }

            var stylePair = Styles.FirstOrDefault(pair => pair.Type == item.GetType());

            if (stylePair != null)
            {
                return stylePair.Style;
            }

            return DefaultStyle;
        }
    }
}