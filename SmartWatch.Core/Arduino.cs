﻿using System;
using System.Diagnostics;
using System.IO.Ports;

namespace SmartWatch.Core
{
    public class Arduino : IDisposable
    {
        #region Data Model

        private readonly SerialPort _serialPort;

        public event EventHandler<int> DataRecieved;

       

        #endregion

        #region Constructor

        public Arduino()
        {
            //TODO - Get correct arduino port, etc
            _serialPort = new SerialPort("COM4");
            _serialPort.BaudRate = 9600;
            _serialPort.Handshake = Handshake.None;
            _serialPort.DataReceived += _serialPort_DataReceived;
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
            var serialPort = (SerialPort) sender;
            var data = serialPort.ReadLine();
            var array = data.Split('|');
            int proximity;
            if (Int32.TryParse(array[1], out proximity))
            {
                if (proximity < 2000)
                    proximity = 2000;
                else if (proximity > 4000)
                    proximity = 4000;
                
                proximity = proximity/20 - 100;
                OnDataRecieved(proximity);
                Debug.WriteLine(proximity);
            } else
                Debug.WriteLine("Proximity conversion to int failed.");
        }

        public void OpenConnection()
        {
            _serialPort.Open();
        }

        public void CloseConnection()
        {
            _serialPort.Close();
        }


        #endregion

        /// <summary>
        ///     Event that is invoked when there is data that should be used by any one else is ready
        /// </summary>
        protected virtual void OnDataRecieved(int e)
        {
            var handler = DataRecieved;
            if (handler != null) handler(this, e);
        }
     

        #region IDisposable members

        public void Dispose()
        {
            _serialPort.Close();
        }

        #endregion
    }
}