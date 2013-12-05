using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Recognizer.Dollar;
using SmartWatch.Core.Gestures;
using WobbrockLib;

namespace SmartWatch.Core.ProximitySensors
{
    public class GestureRecognition : IGestures
    {
        private const int QueueCapacity = 15;

        private const bool UsingQueue = true;
        private readonly IArduino _arduino;
        private readonly Recognizer.Dollar.Recognizer _recogniser;
        private int pause = 0;

        private List<TimePointF> _list;
        private Queue<TimePointF> _queue;

        public GestureRecognition()
        {
            _list = new List<TimePointF>();
            _queue = new Queue<TimePointF>(QueueCapacity);

            _recogniser = new Recognizer.Dollar.Recognizer();

            LoadGestures();

            _arduino = new Arduino("COM3");
            //_arduino = new ArduinoMock();
            _arduino.DataRecieved += arduino_DataRecievedIntoQueue;

            //_arduino.TapRecieved += arduino_TapRecieved;
        }

        /// <summary>
        ///     Loads gesture templates from Xml files stored in "C:\InteractiveDevices\GestureXmls\"
        ///     These are not stored in a relative directory because this is project produces a .dll, so you would either have to
        ///     embed the xml files in the dll or copy them to projects that use this dll, neither of which I can currently be
        ///     bothered to do
        /// </summary>
        private void LoadGestures()
        {
            string[] filePaths = Directory.GetFiles(@"C:\InteractiveDevices\GestureXmls\", "*.xml",
                SearchOption.AllDirectories);

            foreach (string path in filePaths)
            {
                bool success = _recogniser.LoadGesture(path);
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
            if (e && UsingQueue)
                _arduino.DataRecieved += arduino_DataRecievedIntoQueue;
        }

        private void arduino_DataRecievedIntoList(object sender, List<TimePointF> e)
        {
            _list.Add(e[1]);
            

            if (_list.Count > 6)
            {
                NBestList result = _recogniser.Recognize(_list, false);

                if (result.IsEmpty)
                {
                    Debug.WriteLine("Nothing recognised");
                }
                else
                {
                    string output = String.Format("{0}, {1}, {2}, {3}{4}", result.Name,
                        Math.Round(result.Score, 2),
                        Math.Round(result.Distance, 2),
                        Math.Round(result.Angle, 2), (char) 176);

                    Debug.WriteLine(output);

                    _arduino.DataRecieved -= arduino_DataRecievedIntoList;
                    _list = new List<TimePointF>();
                    _arduino.IsEnabled = false;
                    Debug.WriteLine("Detection stopped.");
                }
            }
        }

        private void arduino_DataRecievedIntoQueue(object sender, List<TimePointF> e)
        {
            const int max = 15;

            if (_list.Any(item => (int) item.X == (int)e[0].X))
            {
                return;
            }

            if (pause == 0)
                _list.Add(e[0]);
            else
            {
                pause--;
            }

            if (_list.Count > max)
            {
                _list.RemoveAt(0);
            }

            var xList = new List<int>();
            foreach(var item in _list)
                xList.Add((int)item.X);

            var filtered_list = process(xList);



            // every time theres a new data, it tries to do a detection
            var result = 0;

            if (filtered_list.Count > 0 && pause == 0)
            {
                var index = 0;
                //if (filtered_list.Max() > 20)
                //{
                //    index = filtered_list.IndexOf(filtered_list.Max()) + 1;

                //    _list = _list.GetRange(index, _list.Count - index);
                //    xList = xList.GetRange(index, xList.Count - index);
                //    filtered_list = filtered_list.GetRange(index, filtered_list.Count - index);
                //}
                //else if (filtered_list.Min() < -20)
                //{
                //    index = filtered_list.IndexOf(filtered_list.Min());

                //    _list = _list.GetRange(0, index);
                //    xList = xList.GetRange(0, index);
                //    filtered_list = filtered_list.GetRange(0, index);
                //}
                if(xList.Count != 0)
                    result = Detect(xList, filtered_list);

                if (result == 1)
                {

                    pause = 8;

                    _list = new List<TimePointF>();

                    //foreach (var item in filtered_list)
                    //{
                    //    Debug.Write(item);
                    //    Debug.Write("   ");
                    //}


                    //Debug.WriteLine("---------------");
                }
            }
        }

        private int Detect(List<int> list, List<int> gradient)
        {
            int min = 0;
            int max = 100;

            // gesture is only valid if the hand is moving half of the maximun distance
            int valid_dist = (max - min)/2;

            if (list.Max() - list.Min() < valid_dist)
            {
               // Debug.WriteLine("quiting");
                return 0;
            }

            int pos = 0;
            int neg = 0;
            int count = 0;

            foreach (var item in gradient)
            {
               if (item > 20)
                {
                    pos = 0;
                    neg = 0;
                    count = 0;
                }
                //else if (item < -10)
                //{
                //    break;
                //}
                else if (item > 0)
                {
                    pos++;
                    count++;
                }
                else if (item < 0)
                {
                    neg++;
                    count++;
                }
            }

            if (pos > neg && count >  8)
            {
                Debug.WriteLine("LEFT");
                OnScrollHorizontal(new ScrollParameters(new Point(0, 0), new Point(10, 0)));

            }
            else if (neg > pos && count > 8)
            {
                Debug.WriteLine(("RIGHT"));
                OnScrollHorizontal(new ScrollParameters(new Point(10, 0), new Point(0, 0)));

            }
            else
            {
                return 0;
            }

            return 1;
        }

        // apply filter to the list
        // gradient filter is applied at the moment
        private List<int> process(List<int> list)
        {
            var filtered_list = new List<int>();

            int[] mask = {-1, 1};

            int val;

            for (int i = 0; i < list.Count - 1; i++)
            {
                val = mask[0]*list[i] + mask[1]*list[i + 1];

                filtered_list.Add(val);
            }

            return filtered_list;
        }

        /// <summary>
        ///     Creates the gesture events from the detected gesture for applications to use
        /// </summary>
        /// <param name="e"></param>
        /// <param name="result"></param>
        private void CreateGestureEvents(TimePointF e, NBestList result)
        {
            switch (char.ToLower(result.Name[0]))
            {
                case 'l':
                    OnScrollHorizontal(new ScrollParameters(new Point(0, (int) e.Y), new Point(10, (int) e.Y)));
                    break;
                case 'r':
                    OnScrollHorizontal(new ScrollParameters(new Point(10, (int) e.Y), new Point(0, (int) e.Y)));
                    break;
            }
        }

        #region Events

        public event EventHandler<PinchParameters> PinchIn;

        public event EventHandler<PinchParameters> PinchOut;

        public event EventHandler<ScrollParameters> ScrollHorizontal;

        public event EventHandler<ScrollParameters> ScrollVertical;

        public event EventHandler<ScrollParameters> ScrollDiagonal;

        protected virtual void OnPinchIn(PinchParameters e)
        {
            EventHandler<PinchParameters> handler = PinchIn;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnPinchOut(PinchParameters e)
        {
            EventHandler<PinchParameters> handler = PinchOut;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnScrollHorizontal(ScrollParameters e)
        {
            EventHandler<ScrollParameters> handler = ScrollHorizontal;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnScrollVertical(ScrollParameters e)
        {
            EventHandler<ScrollParameters> handler = ScrollVertical;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnScrollDiagonal(ScrollParameters e)
        {
            EventHandler<ScrollParameters> handler = ScrollDiagonal;
            if (handler != null) handler(this, e);
        }

        #endregion
    }
}