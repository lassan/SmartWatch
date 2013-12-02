using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using WobbrockLib;
using WobbrockLib.Extensions;

namespace SmartWatch.Core
{
    public class Arduino : IDisposable, IArduino
    {
        #region Data Model

        private readonly SerialPort _serialPort;

        #endregion

        #region Constructor

        public Arduino(string portName)
        {
            _serialPort = new SerialPort {PortName = portName, BaudRate = 9600, Handshake = Handshake.None};
            _serialPort.DataReceived += _serialPort_DataReceived;

        }

        #endregion

        /// <summary>
        ///     Event that is raised when there is data on the serial port from the Arduino.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var serialPort = (SerialPort) sender;
            var data = serialPort.ReadLine();
            

            var array = data.Split('|');
            var tapped = Int32.Parse(array[0]);
            var ambient1 = Int32.Parse(array[1]);
            var proximity1 = Int32.Parse(array[2]);

            //if (tapped == 1)
            //    OnTapped(true);


            //proximity1 = MapProximity(proximity1);

            //if (IsEnabled == false && proximity1 > 25 && proximity1 <= 150)
            //{
            //    OnTapped(true);
            //    IsEnabled = true;
            //    _time = 1;
            //}

            var tpf1 = new TimePointF(proximity1, 2, TimeEx.NowMs);

            OnDataRecieved(tpf1);
        }

        private int MapProximity(int val)
        {
            if (val < 2500)
                val = 2500;
            else if (val > 5000)
                val = 5000;

            return val/20 - 100;
        }

        #region IArudino Members

        public event EventHandler<TimePointF> DataRecieved;

        public event EventHandler<bool> TapRecieved;

        public bool IsEnabled { get; set; }

        public void Connect()
        {
            _serialPort.Open();
        }

        #endregion

        #region Event Invokers

        /// <summary>
        ///     Event that is invoked when there is data that should be used by any one else is ready
        /// </summary>
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

        #region IDisposable members

        public void Dispose()
        {
            _serialPort.Close();
        }

        #endregion
    }
}