using System;
using SmartWatch.Core.Gestures;
using SmartWatch.Core.Mocks;

namespace SmartWatch.PrintGestures
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //var gestures = new Core.ProximitySensors.Gestures();
            var gestures = new RandomGestures();
            gestures.PinchIn += GesturesPinchIn;
            gestures.PinchOut += GesturesPinchOut;
            gestures.ScrollHorizontal += GesturesScrollHorizontal;
            gestures.ScrollVertical += GesturesScrollVertical;
            gestures.ScrollDiagonal += GesturesScrollDiagonal;

            Console.Read();
        }

        #region Gesture Event Handler

        private static void GesturesPinchOut(object sender, ScrollParameters e)
        {
            Console.WriteLine("PinchOut:\t\t x0: {0} \t x1: {1} \t y0: {2} \t y1: {3}", e.X0, e.X1, e.Y0, e.Y1);
        }

        private static void GesturesScrollVertical(object sender, ScrollParameters e)
        {
            Console.WriteLine("Vertical Scroll:\t x0: {0} \t x1: {1} \t y0: {2} \t y1: {3}", e.X0, e.X1, e.Y0, e.Y1);
        }

        private static void GesturesScrollDiagonal(object sender, ScrollParameters e)
        {
            Console.WriteLine("Diagonal Scroll:\t x0: {0} \t x1: {1} \t y0: {2} \t y1: {3}", e.X0, e.X1, e.Y0, e.Y1);
        }

        private static void GesturesScrollHorizontal(object sender, ScrollParameters e)
        {
            Console.WriteLine("Horizontal Scroll:\t x0: {0} \t x1: {1} \t y0: {2} \t y1: {3}", e.X0, e.X1, e.Y0, e.Y1);
        }

        private static void GesturesPinchIn(object sender, ScrollParameters e)
        {
            Console.WriteLine("PinchIn:\t\t x0: {0} \t x1: {1} \t y0: {2} \t y1: {3}", e.X0, e.X1, e.Y0, e.Y1);
        }

        #endregion
    }
}