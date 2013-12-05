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

            var incrementingList = new List<TimePointF>();
            for (var i = 0; i < 150; i++)
                incrementingList.Add(new TimePointF(i, 1, 1));
            
            while (true)
            {
                foreach (var item in incrementingList)
                {
                    OnDataRecieved(new List<TimePointF> { item, item, item });
                    Thread.Sleep(10);
                }
                incrementingList.Reverse();
            }
        }

        #region IArduino Members

        public event EventHandler<List<TimePointF>> DataRecieved;

        public event EventHandler<bool> TapRecieved;

        public bool IsEnabled { get; set; }

        public void Connect()
        {
        }

        #endregion

        #region Event Invokers

        protected virtual void OnDataRecieved(List<TimePointF> e)
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