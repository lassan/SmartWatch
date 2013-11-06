using System;
using SmartWatch.Core.Gestures;

namespace SmartWatch.PrintGestures
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IGestures gestures = new Core.ProximitySensors.Gestures();
            //var gestures = new Core.Mocks.MockGestures();
            gestures.Pinch += GesturesPinch;
            gestures.ScrollHorizontal += GesturesScrollHorizontal;
            gestures.ScrollVertical += GesturesScrollVertical;
            gestures.ScrollDiagonal += GesturesScrollDiagonal;

            Console.Read();
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

        private static void GesturesPinch(object sender, GestureParameters e)
        {
            Console.WriteLine("Pinch from {0} to {1}", e.From, e.To);
        }

        #endregion
    }
}