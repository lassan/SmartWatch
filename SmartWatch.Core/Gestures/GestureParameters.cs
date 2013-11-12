namespace SmartWatch.Core.Gestures
{
    /// <summary>
    ///     TODO Perhaps it makes more sense to have a discrete set of parameters
    /// </summary>
    public class ScrollParameters
    {
        public ScrollParameters(Point startPoint, Point endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public Point StartPoint { get; private set; }
        public Point EndPoint { get; private set; }
    }

    public class PinchParameters
    {
        public PinchParameters(Point startPoint0, Point startPoint1, Point endPoint0, Point endPoint1)
        {
            StartPoint0 = startPoint0;
            StartPoint1 = startPoint1;
            EndPoint0 = endPoint0;
            EndPoint1 = endPoint1;
        }

        public Point StartPoint0 { get; private set; }
        public Point StartPoint1 { get; private set; }
        public Point EndPoint0 { get; private set; }
        public Point EndPoint1 { get; private set; }
    }

    public struct Point
    {
        public int x;
        public int y;

        public Point(int xIn, int yIn)
        {
            x = xIn;
            y = yIn;
        }
    }
}