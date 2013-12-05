using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using WobbrockLib;
using WobbrockLib.Extensions;

namespace SmartWatch.Core
{
    public class Arduino : IArduino
    {
        #region Data Model
        SerialPort _serialPort;

        #endregion

        #region Constructor

        public Arduino(string portName)
        {
            _serialPort = new SerialPort
            {
                PortName = portName,
                BaudRate = 19200,
                ReadTimeout = 500
            };

            var thread = new Thread(ReadDataFromSerialPort) { IsBackground = true, Name = "SerialDataRecieverThread" };
            _serialPort.Open();
            thread.Start();

        }

        private void ReadDataFromSerialPort()
        {
            while (true)
            {
                var data = String.Empty;
                int tapped;
                int proximity1 = 0;
                int proximity2 = 0;
                int proximity3 = 0;
                bool exception = false;
                try
                {
                    var serialPort = _serialPort;
                    data = serialPort.ReadLine();

                    var array = data.Split('|');

                    if (array.Length != 4)
                        return;

                    tapped = Int32.Parse(array[0]);
                    proximity1 = Int32.Parse(array[1]);
                    proximity2 = Int32.Parse(array[2]);
                    proximity3 = Int32.Parse(array[3]);
                }
                catch (Exception)
                {
                    exception = true;
                    Debug.Write("Invalid data:\t");
                    Debug.WriteLine(data);
                }
                //if (tapped == 1 && IsEnabled == false)
                //{
                //    IsEnabled = true;
                //    OnTapped(true);
                //}
                if (!exception)
                {
                    var tpf1 = new TimePointF(proximity1, 1, TimeEx.NowMs);
                    var tpf2 = new TimePointF(proximity2, 2, TimeEx.NowMs);
                    var tpf3 = new TimePointF(proximity3, 2, TimeEx.NowMs);

                    OnDataRecieved(new List<TimePointF> { tpf1, tpf2, tpf3 });
                }
            }
        }

        #endregion

        #region IArudino Members

        public event EventHandler<List<TimePointF>> DataRecieved;

        public event EventHandler<bool> TapRecieved;

        public bool IsEnabled { get; set; }

        public void Connect()
        {
        }

        #endregion

        #region Event Invokers

        /// <summary>
        ///     Event that is invoked when there is data that should be used by any one else is ready
        /// </summary>
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
    }
}