using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Cryptography;
using SmartWatch.Core.Gestures;
using SmartWatch.Core.Mocks;
using Recognizer.Dollar;
using System.IO;

namespace SmartWatch.Core.ProximitySensors
{
    public class GestureRecognition : IGestures
    {
        private Recognizer.Dollar.Recognizer _recogniser;

        List<int> list = new List<int>();

        public GestureRecognition()
        {
            //var arduino = new Arduino();
            var arduino = new ArduinoMock();
            arduino.DataRecieved += arduino_DataRecieved;
            arduino.OpenConnection();

            _recogniser = new Recognizer.Dollar.Recognizer();

            LoadGestures();
        }

        private void LoadGestures()
        {
            var filePaths = Directory.GetFiles(@"C:\InteractiveDevices\GestureXmls\", "*.xml", SearchOption.AllDirectories);

            foreach (var path in filePaths)
            {
                var success = _recogniser.LoadGesture(path);
                if (success)
                    Debug.WriteLine(path + "loaded successfully.");
                else
                    Debug.WriteLine(path + "failed to load.");
            }
        }


        void arduino_DataRecieved(object sender, int e)
        {

            throw new NotImplementedException();
        }


        #region Events

        public event EventHandler<PinchParameters> PinchIn;

        public event EventHandler<PinchParameters> PinchOut;

        public event EventHandler<ScrollParameters> ScrollHorizontal;

        public event EventHandler<ScrollParameters> ScrollVertical;

        public event EventHandler<ScrollParameters> ScrollDiagonal;

        protected virtual void OnPinchIn(PinchParameters e)
        {
            var handler = PinchIn;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnPinchOut(PinchParameters e)
        {
            var handler = PinchOut;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnScrollHorizontal(ScrollParameters e)
        {
            var handler = ScrollHorizontal;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnScrollVertical(ScrollParameters e)
        {
            var handler = ScrollVertical;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnScrollDiagonal(ScrollParameters e)
        {
            var handler = ScrollDiagonal;
            if (handler != null) handler(this, e);
        }

        #endregion
    }
}