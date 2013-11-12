using System;

namespace SmartWatch.Core.Gestures
{
    public interface IGestures
    {
        event EventHandler<PinchParameters> PinchIn;

        event EventHandler<PinchParameters> PinchOut;

        event EventHandler<ScrollParameters> ScrollHorizontal;

        event EventHandler<ScrollParameters> ScrollVertical;

        event EventHandler<ScrollParameters> ScrollDiagonal;
    }
}