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
            };

            //_serialPort.DataReceived += DataRecievedEventHandler;
            var thread = new Thread(ReadDataFromSerialPort) { Priority = ThreadPriority.Normal, Name = "SerialDataRecieverThread" };
            _serialPort.Open();
            thread.Start();

        }

        private void DataRecievedEventHandler(object sender, SerialDataReceivedEventArgs e)
        {
            var serialPort = sender as SerialPort;
            ProcessIncomingData(serialPort);
        }


        private void ReadDataFromSerialPort()
        {
            while (true)
            {
                ProcessIncomingData(_serialPort);
            }
        }

        private void ProcessIncomingData(SerialPort serialPort)
        {
            var data = String.Empty;
            int tapped;
            int proximity1 = 0;
            int proximity2 = 0;
            int proximity3 = 0;
            int zoom = 0;
            bool exception = false;
            try
            {
                data = serialPort.ReadLine();
                var array = data.Split('|');
                
                if (array.Length != 5)
                    return;

                tapped = Int32.Parse(array[0]);
                proximity1 = Int32.Parse(array[1]);
                proximity2 = Int32.Parse(array[2]);
                proximity3 = Int32.Parse(array[3]);
                zoom = Int32.Parse(array[4]);
            }
            catch (TimeoutException ex)
            {
                Debug.WriteLine("Timeout on the serial port");
            }
            catch (Exception)
            {
                exception = true;
                Debug.Write("Invalid data:\t");
                Debug.WriteLine(data);
            }
            if (!exception)
            {
                var tpf1 = new TimePointF(proximity1, 0, TimeEx.NowMs);
                var tpf2 = new TimePointF(proximity2, 50, TimeEx.NowMs);
                var tpf3 = new TimePointF(proximity3, 100, TimeEx.NowMs);
                var zoomtpf = new TimePointF(zoom, 0, TimeEx.NowMs);

                OnDataRecieved(new List<TimePointF> {tpf1, tpf2, tpf3, zoomtpf});
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