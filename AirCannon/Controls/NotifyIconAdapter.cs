using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using AirCannon.Framework.Utilities;
using Control=System.Windows.Controls.Control;
using MouseEventArgs=System.Windows.Forms.MouseEventArgs;
using NotifyIcon=System.Windows.Forms.NotifyIcon;
using WpfMouseEventArgs=System.Windows.Input.MouseEventArgs;

namespace AirCannon.Controls
{
    public class NotifyIconAdapter : Control, IDisposable
    {
        private readonly NotifyIcon mNotifyIcon;
        private const string ICON_URI = "pack://application:,,,/Resources/Icons/application_go.png";

        public NotifyIconAdapter()
        {
            mNotifyIcon = new NotifyIcon();
            mNotifyIcon.Icon = Resource.FromUri(ICON_URI).AsIcon();
            mNotifyIcon.Visible = true;

            mNotifyIcon.MouseClick += _HandleNotifyIconMouseClick;
            mNotifyIcon.MouseDoubleClick += _HandleNotifyIconMouseDoubleClick;
        }

        private void _HandleNotifyIconMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _RaiseDoubleClickEvent();
            }
        }

        private void _HandleNotifyIconMouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                _RaiseRightClickEvent();
            }
        }

        public static readonly RoutedEvent NotifyIconDoubleClickEvent =
            EventManager.RegisterRoutedEvent("NotifyIconDoubleClickEvent", RoutingStrategy.Bubble,
                                             typeof (RoutedEventHandler), typeof (NotifyIconAdapter));

        public static readonly RoutedEvent NotifyIconRightClickEvent =
            EventManager.RegisterRoutedEvent("NotifyIconRightClick", RoutingStrategy.Bubble,
                                             typeof (RoutedEventHandler), typeof (NotifyIconAdapter));

        public event RoutedEventHandler NotifyIconRightClick
        {
            add { AddHandler(NotifyIconRightClickEvent, value); }
            remove { RemoveHandler(NotifyIconRightClickEvent, value); }
        }

        public event RoutedEventHandler NotifyIconDoubleClick
        {
            add { AddHandler(NotifyIconDoubleClickEvent, value); }
            remove { RemoveHandler(NotifyIconDoubleClickEvent, value); }
        }

        private void _RaiseRightClickEvent()
        {
            RaiseEvent(new RoutedEventArgs(NotifyIconRightClickEvent));
        }

        private void _RaiseDoubleClickEvent()
        {
            RaiseEvent(new RoutedEventArgs(NotifyIconDoubleClickEvent));
        }

        public void Dispose()
        {
            mNotifyIcon.Icon = null;
            mNotifyIcon.Visible = false;
            mNotifyIcon.Dispose();
        }
    }
}
