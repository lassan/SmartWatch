using System;
using WobbrockLib;

namespace SmartWatch.Core
{
    public interface IArduino
    {
        bool IsEnabled { get; set; }

        void Connect();

        event EventHandler<TimePointF> DataRecieved;

        event EventHandler<bool> TapRecieved;
    }
}