using System;
using System.Diagnostics;
using System.IO.Ports;
using WobbrockLib;

namespace SmartWatch.Core
{
    public class Arduino : IDisposable, IArduino
    {
        #region Data Model

        private readonly SerialPort _serialPort;
        private int _time;
        private bool _isEnabled;

        #endregion

        #region Constructor

        public Arduino()
        {
            _serialPort = new SerialPort("COM4");
            _serialPort.BaudRate = 9600;
            _serialPort.Handshake = Handshake.None;
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
            var serialPort = (SerialPort)sender;
            var data = serialPort.ReadLine();
            var array = data.Split('|');
            var tapped = Int32.Parse(array[0]);
            var ambient1 = Int32.Parse(array[1]);
            var proximity1 = Int32.Parse(array[2]);
            var ambient2 = Int32.Parse(array[3]);
            var proximity2 = Int32.Parse(array[4]);
            var ambient3 = Int32.Parse(array[5]);
            var proximity3 = Int32.Parse(array[6]);
            var tappedVal = Int32.Parse(array[7]);

            //if (tapped == 1)
            //{
            //    OnTapped(true);
            //    _time = 1;
            //}

            proximity1 = MapProximity(proximity1);
            proximity2 = MapProximity(proximity2);
            proximity3 = MapProximity(proximity3);


            if (IsEnabled == false && proximity1 > 20 && proximity1 < 80)
            {
                OnTapped(true);
                IsEnabled = true;
                _time = 1;
            }

            var tpf1 = new TimePointF(proximity1, 5 * 2, _time * 10);

            var tpf2 = new TimePointF(proximity2, 5 * 3, _time * 10);

            var tpf3 = new TimePointF(proximity3, 5 * 4, _time * 10);

            _time = _time + 1;

            OnDataRecieved(tpf1);
        }

        private int MapProximity(int val)
        {
            if (val < 2500)
                val = 2500;
            else if (val > 5000)
                val = 5000;

            return val / 20 - 100;
        }

        #region IArudino Members

        public event EventHandler<TimePointF> DataRecieved;

        public event EventHandler<bool> TapRecieved;

        public bool IsEnabled {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

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