using System;
using System.Diagnostics;

namespace SmartWatch.Core.Gestures
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
            Debug.WriteLine("Zoom In");
            var handler = ZoomIn;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnZoomOut()
        {
            Debug.WriteLine("Zoom Out");
            var handler = ZoomOut;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnScrollLeft()
        {
            Debug.WriteLine("Scroll Left");
            var handler = ScrollLeft;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnScrollRight()
        {
            Debug.WriteLine("Scroll Right");
            var handler = ScrollRight;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnScrollUp()
        {
            Debug.WriteLine("Scroll Up");
            var handler = ScrollUp;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnScrollDown()
        {
            Debug.WriteLine("Scroll Down");
            var handler = ScrollDown;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}