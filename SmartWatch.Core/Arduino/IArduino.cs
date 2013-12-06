using System;
using System.Collections.Generic;
using WobbrockLib;

namespace SmartWatch.Core
{
    public interface IArduino
    {
        bool IsEnabled { get; set; }

        void Connect();

        event EventHandler<List<TimePointF>> DataRecieved;

        event EventHandler<bool> TapRecieved;
    }
}