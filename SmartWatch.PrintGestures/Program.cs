using System;
using System.Text;
using SmartWatch.Core.Gestures;
using SmartWatch.Core.ProximitySensors;

namespace SmartWatch.PrintGestures
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var gestures = new GestureRecognition();
            //var gestures = new RandomGestures();
            gestures.PinchIn += GesturesPinchIn;
            gestures.PinchOut += GesturesPinchOut;
            gestures.ScrollHorizontal += GesturesScrollHorizontal;
            gestures.ScrollVertical += GesturesScrollVertical;
            gestures.ScrollDiagonal += GesturesScrollDiagonal;

        }

        private static string PinchStringBuilder(PinchParameters e, string heading)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(heading);
            stringBuilder.AppendFormat("\tStart: [{0} {1} ; {2} {3}]",
                e.StartPoint0.x, e.StartPoint0.y,
                e.StartPoint1.x, e.StartPoint1.y);
            stringBuilder.AppendFormat("\tEnd: [{0} {1}; {2} {3}]",
                e.EndPoint0.x, e.EndPoint0.y, e.EndPoint1.x,
                e.EndPoint1.y);
            return stringBuilder.ToString();
        }

        private static string GetScrollDisplayString(ScrollParameters e, string heading)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(heading);
            stringBuilder.AppendFormat("\tStart: [{0} {1}]", e.StartPoint.x, e.StartPoint.y);
            stringBuilder.AppendFormat("\tEnd: [{0} {1}]", e.EndPoint.x, e.EndPoint.y);
            return stringBuilder.ToString();
        }

        #region Gesture Event Handler

        private static void GesturesPinchOut(object sender, PinchParameters e)
        {
            Console.WriteLine(PinchStringBuilder(e, "Pinch Out"));
        }

        private static void GesturesScrollVertical(object sender, ScrollParameters e)
        {
            Console.WriteLine(GetScrollDisplayString(e, "Vertical Scroll"));
        }

        private static void GesturesScrollDiagonal(object sender, ScrollParameters e)
        {
            Console.WriteLine(GetScrollDisplayString(e, "Diagonal Scroll"));
        }

        private static void GesturesScrollHorizontal(object sender, ScrollParameters e)
        {
            Console.WriteLine(GetScrollDisplayString(e, "Horizontal Scroll"));
        }

        private static void GesturesPinchIn(object sender, PinchParameters e)
        {
            Console.WriteLine(PinchStringBuilder(e, "Pinch In"));
        }

        #endregion
    }
}