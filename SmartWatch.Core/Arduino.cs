using System;
using System.IO.Ports;

namespace SmartWatch.Core
{
    public class Arduino
    {
        #region Data Model

        private readonly SerialPort _serialPort;

        public event EventHandler DataRecieved;

        #endregion

        #region Constructor

        public Arduino()
        {
            //TODO - Get correct arduino port, etc
            _serialPort = new SerialPort("COM3", 9600);
            _serialPort.DataReceived += _serialPort_DataReceived;
            _serialPort.Open();
        }

        /// <summary>
        ///     Event that is raised when there is data on the serial port from the Arduino.
        ///     This should probably just invoke the OnDataRecieved method of this class with the serial data to stop this class
        ///     from depending on any of the consumers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        ///     Event that is invoked when there is data that should be used by any one else is ready
        /// </summary>
        protected virtual void OnDataRecieved()
        {
            var handler = DataRecieved;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}