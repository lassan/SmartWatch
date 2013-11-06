using System;

namespace SmartWatch.Core.Gestures
{
    public interface IGestures
    {
        event EventHandler<GestureParameters> PinchIn;

        event EventHandler<GestureParameters> PinchOut;

        event EventHandler<GestureParameters> ScrollHorizontal;

        event EventHandler<GestureParameters> ScrollVertical;

        event EventHandler<GestureParameters> ScrollDiagonal;
    }
}