using System;

namespace SmartWatch.Core.Gestures
{
    public interface IGestures
    {
        event EventHandler<GestureParameters> Pinch;

        event EventHandler<GestureParameters> ScrollHorizontal;

        event EventHandler<GestureParameters> ScrollVertical;

        event EventHandler<GestureParameters> ScrollDiagonal;
    }
}