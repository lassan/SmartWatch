using System;
using System.Text;
using SmartWatch.Core.Gestures;
using SmartWatch.Core.Mocks;

namespace SmartWatch.PrintGestures
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //var gestures = new GestureRecognition();
            var gestures = new RandomGesturesProvider();
            gestures.ZoomIn += GesturesZoomIn;
            gestures.ZoomOut += GesturesZoomOut;
            gestures.ScrollLeft += GesturesScrollLeft;
            gestures.ScrollRight += GesturesScrollRight;
            gestures.ScrollDown += GesturesScrollDown;
            gestures.ScrollUp += GesturesScrollUp;

            Console.ReadLine();
        }

        #region Gesture Event Handler

        private static void GesturesScrollUp(object sender, EventArgs e)
        {
            Console.WriteLine("Scroll Up");
        }

        private static void GesturesScrollDown(object sender, EventArgs e)
        {
            Console.WriteLine("Scroll Down");
        }

        private static void GesturesZoomOut(object sender, EventArgs eventArgs)
        {
           Console.WriteLine("Zoom out");
        }

        private static void GesturesScrollRight(object sender, EventArgs eventArgs)
        {
            Console.WriteLine("Scroll Right");
        }

        private static void GesturesScrollLeft(object sender, EventArgs eventArgs)
        {
            Console.WriteLine("Scroll Left");
        }

        private static void GesturesZoomIn(object sender, EventArgs eventArgs)
        {
            Console.WriteLine("Zoom In");
        }

        #endregion
    }
}