using System;
using System.Timers;
using SmartWatch.Core.Gestures;

namespace SmartWatch.Core.Mocks
{
    /// <summary>
    ///     Mock object that invokes the event for a random gesture every second or so
    /// </summary>
    public class MockGestures : IGestures
    {
        private readonly Random _randomNumGenerator;

        public MockGestures()
        {
            _randomNumGenerator = new Random(5000);
            var timer = new Timer(1500);
            timer.Elapsed += timer_Elapsed;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var num = _randomNumGenerator.Next(0, 5);
            var x0   = _randomNumGenerator.Next(0, 9);
            var x1   = _randomNumGenerator.Next(0, 9);
            var y0 = _randomNumGenerator.Next(0, 9);
            var y1 = _randomNumGenerator.Next(0, 9);

            var gestureParams = new GestureParameters(x0, x1, y0, y1);
            switch (num)
            {
                case 0:
                    OnPinchIn(gestureParams);
                    break;
                case 1:
                    OnScrollHorizontal(gestureParams);
                    break;
                case 2:
                    OnScrollVertical(gestureParams);
                    break;
                case 3:
                    OnScrollDiagonal(gestureParams);
                    break;
                case 4:
                    OnPinchOut(gestureParams);
                    break;
                default:
                    throw new ArgumentException("Random number should be restricted to a maximum of 3.");
            }
        }

        #region Events

        public event EventHandler<GestureParameters> PinchIn;

        public event EventHandler<GestureParameters> PinchOut;

        public event EventHandler<GestureParameters> ScrollHorizontal;

        public event EventHandler<GestureParameters> ScrollVertical;

        public event EventHandler<GestureParameters> ScrollDiagonal;

        protected virtual void OnPinchIn(GestureParameters e)
        {
            var handler = PinchIn;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnPinchOut(GestureParameters e)
        {
            var handler = PinchOut;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnScrollHorizontal(GestureParameters e)
        {
            var handler = ScrollHorizontal;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnScrollVertical(GestureParameters e)
        {
            var handler = ScrollVertical;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnScrollDiagonal(GestureParameters e)
        {
            var handler = ScrollDiagonal;
            if (handler != null) handler(this, e);
        }

        #endregion
    }
}