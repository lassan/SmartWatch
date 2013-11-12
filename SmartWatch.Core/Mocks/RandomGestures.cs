using System;
using System.Timers;
using SmartWatch.Core.Gestures;

namespace SmartWatch.Core.Mocks
{
    /// <summary>
    ///     Mock object that invokes the event for a random gesture every second or so
    /// </summary>
    public class RandomGestures : IGestures
    {
        private readonly Random _randomNumGenerator;

        public RandomGestures()
        {
            _randomNumGenerator = new Random(5000);
            var timer = new Timer(1500);
            timer.Elapsed += timer_Elapsed;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //var num = _randomNumGenerator.Next(0, 5);
            //var x0 = _randomNumGenerator.Next(0, 9);
            //var x1 = _randomNumGenerator.Next(0, 9);
            //var y0 = _randomNumGenerator.Next(0, 9);
            //var y1 = _randomNumGenerator.Next(0, 9);

            //var gestureParams = new ScrollParameters(x0, x1, y0, y1);
            //switch (num)
            //{
            //    case 0:
            //        OnPinchIn(gestureParams);
            //        break;
            //    case 1:
            //        OnScrollHorizontal(gestureParams);
            //        break;
            //    case 2:
            //        OnScrollVertical(gestureParams);
            //        break;
            //    case 3:
            //        OnScrollDiagonal(gestureParams);
            //        break;
            //    case 4:
            //        OnPinchOut(gestureParams);
            //        break;
            //    default:
            //        throw new ArgumentException("Random number should be restricted to a maximum of 3.");
            //}
        }

        #region Events

        public event EventHandler<PinchParameters> PinchIn;

        public event EventHandler<PinchParameters> PinchOut;

        public event EventHandler<ScrollParameters> ScrollHorizontal;

        public event EventHandler<ScrollParameters> ScrollVertical;

        public event EventHandler<ScrollParameters> ScrollDiagonal;

        protected virtual void OnPinchIn(PinchParameters e)
        {
            var handler = PinchIn;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnPinchOut(PinchParameters e)
        {
            var handler = PinchOut;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnScrollHorizontal(ScrollParameters e)
        {
            var handler = ScrollHorizontal;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnScrollVertical(ScrollParameters e)
        {
            var handler = ScrollVertical;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnScrollDiagonal(ScrollParameters e)
        {
            var handler = ScrollDiagonal;
            if (handler != null) handler(this, e);
        }

        #endregion
    }
}