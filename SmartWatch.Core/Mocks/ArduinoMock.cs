using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WobbrockLib;

namespace SmartWatch.Core.Mocks
{
    public class ArduinoMock : IArduino, IDisposable
    {
        public ArduinoMock()
        {
            Task.Run(() => GenerateTestData());
        }

        public void GenerateTestData()
        {
            var list = new List<TimePointF>();

            var l = 30;

            for (var i = 0; i < l; i++)
            {
                if (i == 5 || i == 15)
                    OnTapped(true);

                var tpf = new TimePointF(i, 1, 10);
                list.Add(tpf);
            }

            foreach (var item in list)
            {
                OnDataRecieved(item);
                Thread.Sleep(100);
            }
        }

        #region IArduino Members

        public event EventHandler<TimePointF> DataRecieved;

        public event EventHandler<bool> TapRecieved;

        public bool IsEnabled { get; set; }

        public void Connect()
        {
        }

        #endregion

        #region Event Invokers

        protected virtual void OnDataRecieved(TimePointF e)
        {
            var handler = DataRecieved;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnTapped(bool e)
        {
            var handler = TapRecieved;
            if (handler != null) handler(this, e);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}