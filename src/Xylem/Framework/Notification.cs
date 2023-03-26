
using System.Collections.Generic;
using System;
using Xylem.Functional;
using Xylem.Graphics;

namespace Xylem.Framework
{
    public static class NotificationManager
    {
        public static TooltipRenderer Renderer { get; set; } = XylemTooltipRenderer.Instance;

        public static int NotificationLimit { get; set; } = 5;

        public static Provider<int> X { get; set; } = () => 5;
        public static Provider<int> Y { get; set; } = () => XylemModule.Container.RH - (Renderer.MeasureHeight());

        public static int DisplaySeconds { get; set; } = 5;

        private static readonly LinkedList<string> _notifications;
        private static readonly LinkedList<DateTime> _notificationTimes;

        static NotificationManager()
        {
            _notifications = new LinkedList<string>();
            _notificationTimes = new LinkedList<DateTime>();
        }

        public static void EnqueueNotification(string text)
        {
            _notifications.AddLast(text);
            _notificationTimes.AddLast(DateTime.Now);

            if (NotificationLimit < _notifications.Count)
            {
                _notifications.RemoveFirst();
                _notificationTimes.RemoveFirst();
            }
        }

        internal static void UpdateNotifications()
        {
            if (_notifications.Count == 0)
                return;

            LinkedListNode<string> notification = _notifications.First;
            LinkedListNode<DateTime> time = _notificationTimes.First;

            while (notification != null && time != null)
            {
                TimeSpan span = DateTime.Now - time.Value;

                if (DisplaySeconds <= span.TotalSeconds)
                {
                    _notifications.Remove(notification);
                    _notificationTimes.Remove(time);
                }

                notification = notification.Next;
                time = time.Next;
            }
        }

        internal static void RenderNotifications()
        {
            int rx = X();
            int ry = Y();

            LinkedListNode<string> node = _notifications.Last;

            while (node != null)
            {
                Renderer.Render(new Vectors.Vec2i(rx, ry), node.Value);
                ry -= Renderer.MeasureHeight();
                node = node.Previous;
            }
        }
    }
}