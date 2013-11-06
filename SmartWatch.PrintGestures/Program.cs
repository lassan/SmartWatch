using System;
using SmartWatch.Core.Gestures;

namespace SmartWatch.PrintGestures
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //var gestures = new Core.ProximitySensors.Gestures();
            var gestures = new Core.Mocks.MockGestures();
            gestures.PinchIn += GesturesPinchIn;
            gestures.PinchOut += GesturesPinchOut;
            gestures.ScrollHorizontal += GesturesScrollHorizontal;
            gestures.ScrollVertical += GesturesScrollVertical;
            gestures.ScrollDiagonal += GesturesScrollDiagonal;

            Console.Read();
        }

        static void GesturesPinchOut(object sender, GestureParameters e)
        {
            Console.WriteLine("PinchOut from {0} to {1}", e.From, e.To);
        }

        #region Gesture Event Handler

        private static void GesturesScrollVertical(object sender, GestureParameters e)
        {
            Console.WriteLine("Vertical scroll from {0} to {1}", e.From, e.To);
        }

        private static void GesturesScrollDiagonal(object sender, GestureParameters e)
        {
            Console.WriteLine("Diagonal scroll from {0} to {1}", e.From, e.To);
        }

        private static void GesturesScrollHorizontal(object sender, GestureParameters e)
        {
            Console.WriteLine("Horizontal scroll from {0} to {1}", e.From, e.To);
        }

        private static void GesturesPinchIn(object sender, GestureParameters e)
        {
            Console.WriteLine("PinchIn from {0} to {1}", e.From, e.To);
        }

        #endregion
    }
}