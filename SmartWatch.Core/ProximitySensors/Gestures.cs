using System;
using SmartWatch.Core.Gestures;

namespace SmartWatch.Core.ProximitySensors
{
    public class Gestures : IGestures
    {
        public Gestures()
        {
            var arduino = new Arduino();
            arduino.DataRecieved += arduino_DataRecieved;
        }

        private void arduino_DataRecieved(object sender, EventArgs e)
        {
            //Decide what that data means and what the gesture is, then raise the appropriate event
            throw new NotImplementedException();
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