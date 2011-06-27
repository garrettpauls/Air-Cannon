using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using SysDragDrop=System.Windows.DragDrop;

namespace AirCannon.Framework.WPF
{
    /// <summary>
    ///   An extension class for managing Drag and Drop via commands.
    /// </summary>
    public static class DragDrop
    {
        private static readonly Dictionary<UIElement, Point> mStartPoints;
        public static readonly DependencyProperty EnabledProperty;
        public static readonly DependencyProperty BeginDragCommandProperty;
        public static readonly DependencyProperty DropCommandProperty;
        public static readonly DependencyProperty DragOverCommandProperty;

        static DragDrop()
        {
            mStartPoints = new Dictionary<UIElement, Point>();
            EnabledProperty = DependencyProperty.RegisterAttached(
                "Enabled", typeof (bool), typeof (DragDrop),
                new PropertyMetadata(false, _HandleEnabledChanged));
            BeginDragCommandProperty = DependencyProperty.RegisterAttached(
                "BeginDragCommand", typeof (ICommand), typeof (DragDrop));
            DropCommandProperty = DependencyProperty.RegisterAttached(
                "DropCommand", typeof (ICommand), typeof (DragDrop),
                new PropertyMetadata(_HandleDropCommandChanged));
            DragOverCommandProperty = DependencyProperty.RegisterAttached(
                "DragOverCommand", typeof (ICommand), typeof (DragDrop),
                new PropertyMetadata(_HandleDragOverCommandChanged));
        }

        /// <summary>
        ///   Creates an <see cref = "IDataObject" /> from a <see cref = "UIElement" />,
        ///   adding data keys for the UIElement type and the DataContext type, if 
        ///   the element has a DataContext.
        /// </summary>
        public static IDataObject CreateDataObject(UIElement element)
        {
            var data = new DataObject(element);
            data.SetData(element.GetType(), element);
            if (element is FrameworkElement && ((FrameworkElement) element).DataContext != null)
            {
                var dataContext = ((FrameworkElement) element).DataContext;
                data.SetData(dataContext.GetType(), dataContext);
            }

            return data;
        }

        public static ICommand GetBeginDragCommand(UIElement element)
        {
            return (ICommand) element.GetValue(BeginDragCommandProperty);
        }

        public static ICommand GetDragOverCommand(UIElement element)
        {
            return (ICommand) element.GetValue(DragOverCommandProperty);
        }

        public static ICommand GetDropCommand(UIElement element)
        {
            return (ICommand) element.GetValue(DropCommandProperty);
        }

        public static bool GetEnabled(UIElement element)
        {
            return (bool) element.GetValue(EnabledProperty);
        }

        /// <summary>
        ///   This command is called when the mouse is clicked on an element and drug beyond the 
        ///   system threshold, starting a drag and drop action.
        ///   It is passed one paramter of type <see cref = "UIElement" /> and should call
        ///   <see cref = "System.Windows.DragDrop.DoDragDrop" />.
        /// </summary>
        public static void SetBeginDragCommand(UIElement element, ICommand command)
        {
            element.SetValue(BeginDragCommandProperty, command);
        }

        /// <summary>
        ///   This command is called when the element is drug over.
        ///   The command is passed one parameter of type <see cref = "DragEventArgs" />.
        /// </summary>
        public static void SetDragOverCommand(UIElement element, ICommand command)
        {
            element.SetValue(DragOverCommandProperty, command);
        }

        /// <summary>
        ///   The command to be executed when the user drops an item onto this object.
        /// </summary>
        public static void SetDropCommand(UIElement element, ICommand command)
        {
            element.SetValue(DropCommandProperty, command);
        }

        /// <summary>
        ///   Whether this object can be drag and dropped or not.
        /// </summary>
        public static void SetEnabled(UIElement element, bool enabled)
        {
            element.SetValue(EnabledProperty, enabled);
        }

        /// <summary>
        ///   Starts a DragDrop action using either the user provided command 
        ///   or a default action if no command is provided.
        /// </summary>
        private static void _DoDragDrop(UIElement element)
        {
            ICommand command = GetBeginDragCommand(element);
            if (command != null)
            {
                if (command.CanExecute(element))
                {
                    command.Execute(element);
                }
            }
            else
            {
                var data = CreateDataObject(element);
                SysDragDrop.DoDragDrop(element, data, DragDropEffects.All);
            }
        }

        private static void _HandleDragOver(object sender, DragEventArgs e)
        {
            ICommand command = GetDragOverCommand((UIElement) sender);
            if (command != null && command.CanExecute(e))
            {
                command.Execute(e);
            }
        }

        private static void _HandleDragOverCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = (UIElement) d;
            element.DragOver -= _HandleDragOver;
            if (e.NewValue != null)
            {
                element.DragOver += _HandleDragOver;
            }
        }

        private static void _HandleDrop(object sender, DragEventArgs e)
        {
            var command = GetDropCommand((UIElement) sender);
            if (command != null && command.CanExecute(e))
            {
                command.Execute(e);
            }
        }

        private static void _HandleDropCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = (UIElement) d;
            element.AllowDrop = e.NewValue != null;
            element.Drop -= _HandleDrop;
            if (e.NewValue != null)
            {
                element.Drop += _HandleDrop;
            }
        }

        private static void _HandleEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = (UIElement) d;
            element.PreviewMouseLeftButtonDown -= _HandlePreviewMouseLeftButtonDown;
            element.MouseLeave -= _HandleMouseLeave;
            element.MouseMove -= _HandleMouseMove;

            if (e.NewValue != null)
            {
                element.PreviewMouseLeftButtonDown += _HandlePreviewMouseLeftButtonDown;
                element.MouseLeave += _HandleMouseLeave;
                element.MouseMove += _HandleMouseMove;
            }
        }

        /// <summary>
        ///   Starts a DragDrop action when the element is left and a drag is in progress.
        /// </summary>
        private static void _HandleMouseLeave(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed ||
                !mStartPoints.ContainsKey((UIElement) sender))
            {
                return;
            }
            UIElement element = (UIElement) sender;
            mStartPoints.Remove(element);

            _DoDragDrop(element);
        }

        /// <summary>
        ///   Starts a DragDrop action when the mouse is drug past the 
        ///   <see cref = "SystemParameters.MinimumHorizontalDragDistance" /> or
        ///   <see cref = "SystemParameters.MinimumVerticalDragDistance" />.
        /// </summary>
        private static void _HandleMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed ||
                !mStartPoints.ContainsKey((UIElement) sender))
            {
                return;
            }

            UIElement element = (UIElement) sender;
            var diff = mStartPoints[element] - e.GetPosition(null);
            if (Math.Abs(diff.X) <= SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(diff.Y) <= SystemParameters.MinimumVerticalDragDistance)
            {
                return;
            }
            mStartPoints.Remove(element);

            _DoDragDrop(element);
        }

        /// <summary>
        ///   Marks the starting point of a drag action.
        /// </summary>
        private static void _HandlePreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mStartPoints[(UIElement) sender] = e.GetPosition(null);
        }
    }
}