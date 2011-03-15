using System;
using System.Windows;
using System.Windows.Controls;
using AirCannon.Framework.WPF;
using NUnit.Framework;

namespace AirCannon.Framework.Tests.WPF
{
    /// <summary>
    ///   Tests for <see cref = "TypedStyleSelector" />.
    /// </summary>
    [TestFixture]
    public class TypedStyleSelectorTests
    {
        private readonly Style mButtonStyle = new Style(typeof (Button));
        private readonly Style mDefaultStyle = new Style();
        private readonly Style mMenuItemStyle = new Style(typeof (MenuItem));
        private readonly TypedStyleSelector mSelector;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "TypedStyleSelectorTests" /> class.
        /// </summary>
        public TypedStyleSelectorTests()
        {
            mSelector = new TypedStyleSelector();

            mSelector.Styles.Add(new TypeStylePair
                                     {
                                         Type = typeof (Button),
                                         Style = mButtonStyle
                                     });
            mSelector.Styles.Add(new TypeStylePair
                                     {
                                         Type = typeof (MenuItem),
                                         Style = mMenuItemStyle
                                     });
            mSelector.DefaultStyle = mDefaultStyle;
        }

        /// <summary>
        ///   Verifies that if a style associated with the given object's type hasn't been registered
        ///   the default style is returned.
        /// </summary>
        [Test]
        public void DefaultStyleSelectionTest()
        {
            Assert.AreSame(mDefaultStyle, mSelector.SelectStyle(null, null));
            Assert.AreSame(mDefaultStyle, mSelector.SelectStyle(string.Empty, null));
        }

        /// <summary>
        ///   Verifies that the style associated with the given object's type is returned.
        /// </summary>
        [Test, STAThread]
        public void StyleSelectionTest()
        {
            Assert.AreSame(mButtonStyle, mSelector.SelectStyle(new Button(), null));
            Assert.AreSame(mMenuItemStyle, mSelector.SelectStyle(new MenuItem(), null));
        }
    }
}