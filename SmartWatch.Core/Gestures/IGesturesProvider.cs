using System;

namespace SmartWatch.Core.Gestures
{
    public interface IGesturesProvider
    {
        event EventHandler ZoomIn;

        event EventHandler ZoomOut;

        event EventHandler ScrollLeft;

        event EventHandler ScrollRight;

        event EventHandler ScrollUp;

        event EventHandler ScrollDown;
    }
}