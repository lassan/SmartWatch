using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WobbrockLib;

namespace SmartWatch.Core.Mocks
{
    public class ArduinoMock : IArduino
    {
        public ArduinoMock()
        {
            Task.Run(() => GenerateTestData());
        }

        public void GenerateTestData()
        {
            var left02 = new List<TimePointF>
            {
                new TimePointF(45, 2, 63521666942371),
                new TimePointF(45, 2, 63521666942699),
                new TimePointF(47, 2, 63521666943043),
                new TimePointF(52, 2, 63521666943371),
                new TimePointF(97, 2, 63521666943715)
            };

            var left04 = new List<TimePointF>
            {
                new TimePointF(45, 2, 63521667003788),
                new TimePointF(46, 2, 63521667004101),
                new TimePointF(48, 2, 63521667004444),
                new TimePointF(67, 2, 63521667004773),
                new TimePointF(150, 2, 63521667005116)

            };

            //var l = 30;

            //for (var i = 0; i < l; i++)
            //{
            //    if (i == 5 || i == 15)
            //        OnTapped(true);

            //    var tpf = new TimePointF(i, 1, 10);
            //    list.Add(tpf);
            //}

            foreach (var item in left04)
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