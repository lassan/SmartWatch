using System;
using SmartWatch.Core.Gestures;

namespace SmartWatch.Core
{
    public class GesturesEventInvocator : IGesturesProvider
    {
        public event EventHandler ZoomIn;

        public event EventHandler ZoomOut;

        public event EventHandler ScrollLeft;

        public event EventHandler ScrollRight;

        public event EventHandler ScrollUp;

        public event EventHandler ScrollDown;

        protected virtual void OnZoomIn()
        {
            var handler = ZoomIn;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnZoomOut()
        {
            var handler = ZoomOut;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnScrollLeft()
        {
            var handler = ScrollLeft;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnScrollRight()
        {
            var handler = ScrollRight;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnScrollUp()
        {
            var handler = ScrollUp;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnScrollDown()
        {
            var handler = ScrollDown;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}