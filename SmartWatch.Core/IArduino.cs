using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WobbrockLib;

namespace SmartWatch.Core
{
    interface IArduino
    {
        bool IsEnabled {get; set;}

        void Connect();

        event EventHandler<TimePointF> DataRecieved;

        event EventHandler<bool> TapRecieved;
    }
}
