using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SmartWatch.Core.Gestures;
using SmartWatch.Core.Mocks;
using WobbrockLib;

namespace SmartWatch.Core.ProximitySensors
{
    public class GestureRecognition : IGestures
    {
        private readonly IArduino _arduino;
        private readonly Recognizer.Dollar.Recognizer _recogniser;

        private List<TimePointF> _list;
        private Queue<TimePointF> _queue;
        private const int QueueCapacity = 15;

        private const bool UsingQueue = true;

        public GestureRecognition()
        {
            _list = new List<TimePointF>();
            _queue = new Queue<TimePointF>(QueueCapacity);

            _recogniser = new Recognizer.Dollar.Recognizer();

            LoadGestures();

            _arduino = new Arduino("COM3");
            //_arduino = new ArduinoMock();
            _arduino.DataRecieved += arduino_DataRecievedIntoQueue;

            _arduino.TapRecieved += arduino_TapRecieved;
            _arduino.Connect();
        }

        /// <summary>
        ///     Loads gesture templates from Xml files stored in "C:\InteractiveDevices\GestureXmls\"
        ///     These are not stored in a relative directory because this is project produces a .dll, so you would either have to
        ///     embed the xml files in the dll or copy them to projects that use this dll, neither of which I can currently be
        ///     bothered to do
        /// </summary>
        private void LoadGestures()
        {
            var filePaths = Directory.GetFiles(@"C:\InteractiveDevices\GestureXmls\", "*.xml",
                SearchOption.AllDirectories);

            foreach (var path in filePaths)
            {
                var success = _recogniser.LoadGesture(path);
                if (success)
                    Debug.WriteLine(path + " loaded successfully.");
                else
                    Debug.WriteLine(path + " failed to load.");
            }
        }

        /// <summary>
        ///     Starts listening to the DataRecieved event from the arduino to start collecting data for gesture recognition
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void arduino_TapRecieved(object sender, bool e)
        {
            Debug.WriteLine("Starting to detect.");
            if (e && !UsingQueue)
                _arduino.DataRecieved += arduino_DataRecievedIntoList;
            else if (e && UsingQueue)
                _arduino.DataRecieved += arduino_DataRecievedIntoQueue;

        }

        private void arduino_DataRecievedIntoList(object sender, TimePointF e)
        {
            _list.Add(e);

            if (_list.Count > 6)
            {
                var result = _recogniser.Recognize(_list, false);

                if (result.IsEmpty)
                {
                    Debug.WriteLine("Nothing recognised");
                }
                else
                {
                    var output = String.Format("{0}, {1}, {2}, {3}{4}", result.Name,
                        Math.Round(result.Score, 2),
                        Math.Round(result.Distance, 2),
                        Math.Round(result.Angle, 2), (char)176);

                    Debug.WriteLine(output);

                    _arduino.DataRecieved -= arduino_DataRecievedIntoList;
                    _list = new List<TimePointF>();
                    _arduino.IsEnabled = false;
                    Debug.WriteLine("Detection stopped.");
                }
            }
        }

        private void arduino_DataRecievedIntoQueue(object sender, TimePointF e)
        {
            //Debug.WriteLine(e.X);

            if (_queue.Count == QueueCapacity)
                _queue.Dequeue();

            if (_queue.Count != 0)
            {
                
                var difference = Math.Abs(_queue.Peek().X - e.X);
                //Debug.Write();
                if (difference > 0)
                    _queue.Enqueue(e);
            }
            else
            {
                _queue.Enqueue(e);
            }

            if (_queue.Count < QueueCapacity)
                return;

            var result = _recogniser.Recognize(_queue.ToList(), false);

            if (result.IsEmpty)
                return;

            if (!(result.Score > 0.8))
            {
                _queue.Clear();
                return;
            }


            foreach (var item in _queue.ToList())
                Debug.Write(item.X + "\t");
            Debug.WriteLine("");


            var output = String.Format("{0}, {1}, {2}, {3}{4}", result.Name,
                Math.Round(result.Score, 2),
                Math.Round(result.Distance, 2),
                Math.Round(result.Angle, 2), (char) 176);
            
            Debug.WriteLine(output);

            switch (char.ToLower(result.Name[0]))
            {
                case 'l':
                    OnScrollHorizontal(new ScrollParameters(new Point(0,(int)e.Y), new Point(10,(int)e.Y)));
                    break;
                case 'r':
                    OnScrollHorizontal(new ScrollParameters(new Point(10, (int)e.Y), new Point(0, (int)e.Y)));
                    break;
            }

            _arduino.DataRecieved -= arduino_DataRecievedIntoQueue;
            _arduino.IsEnabled = false;
            _queue.Clear();
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