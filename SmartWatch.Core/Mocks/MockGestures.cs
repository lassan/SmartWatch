﻿using System;
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
            var timer = new Timer(1000);
            timer.Elapsed += timer_Elapsed;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var num = _randomNumGenerator.Next(0, 4);
            var from = _randomNumGenerator.Next(0, 255);
            var to = _randomNumGenerator.Next(0, 255);

            var gestureParams = new GestureParameters(from, to);
            switch (num)
            {
                case 0:
                    OnPinch(gestureParams);
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
                default:
                    throw new ArgumentException("Random number should be restricted to a maximum of 3.");
            }
        }

        #region Events

        public event EventHandler<GestureParameters> Pinch;

        public event EventHandler<GestureParameters> ScrollHorizontal;

        public event EventHandler<GestureParameters> ScrollVertical;

        public event EventHandler<GestureParameters> ScrollDiagonal;

        protected virtual void OnPinch(GestureParameters e)
        {
            var handler = Pinch;
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