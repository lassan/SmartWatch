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
            arduino.OpenConnection();
        }

        void arduino_DataRecieved(object sender, int e)
        {
            throw new NotImplementedException();
        }


        #region Events

        public event EventHandler<PinchParameters> PinchIn;

        public event EventHandler<PinchParameters> PinchOut;

        public event EventHandler<GestureParameters> ScrollHorizontal;

        public event EventHandler<GestureParameters> ScrollVertical;

        public event EventHandler<GestureParameters> ScrollDiagonal;

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