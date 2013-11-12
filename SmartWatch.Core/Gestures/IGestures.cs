using System;

namespace SmartWatch.Core.Gestures
{
    public interface IGestures
    {
        event EventHandler<PinchParameters> PinchIn;

        event EventHandler<PinchParameters> PinchOut;

        event EventHandler<GestureParameters> ScrollHorizontal;

        event EventHandler<GestureParameters> ScrollVertical;

        event EventHandler<GestureParameters> ScrollDiagonal;
    }
}