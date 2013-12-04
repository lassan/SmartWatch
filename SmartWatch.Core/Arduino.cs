using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using WobbrockLib;
using WobbrockLib.Extensions;

namespace SmartWatch.Core
{
    public class Arduino : IArduino
    {
        #region Data Model

        private readonly SerialPort _serialPort;

        #endregion

        #region Constructor

        public Arduino(string portName)
        {
            _serialPort = new SerialPort
            {
                PortName = portName,
                BaudRate = 19200,
                Handshake = Handshake.None,
                ReadTimeout = 500
                //ReadBufferSize = 12
            };

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

            if (array.Length != 4)
                return;

            var tapped = Int32.Parse(array[0]);
            var proximity1 = Int32.Parse(array[1]);
            var proximity2 = Int32.Parse(array[2]);
            var proximity3 = Int32.Parse(array[3]);


            //if (tapped == 1 && IsEnabled == false)
            //{
            //    IsEnabled = true;
            //    OnTapped(true);
            //}

            var tpf1 = new TimePointF(proximity1, 1, TimeEx.NowMs);
            var tpf2 = new TimePointF(proximity2, 2, TimeEx.NowMs);
            var tpf3 = new TimePointF(proximity3, 2, TimeEx.NowMs);

            OnDataRecieved(new List<TimePointF> {tpf1, tpf2, tpf3});
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

        public event EventHandler<List<TimePointF>> DataRecieved;

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

        #region IDisposable members

        public void Dispose()
        {
            _serialPort.Close();
        }

        #endregion
    }
}