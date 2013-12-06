using System;
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
        private const int QueueCapacity = 25;

        private readonly IArduino _arduino;
        private readonly Recognizer.Dollar.Recognizer _recogniser;
        private int _counter;

        private List<TimePointF> _list;
        private int _pause;
        private Queue<TimePointF> _queue;

        public GestureRecognition()
        {
            _list = new List<TimePointF>();
            _queue = new Queue<TimePointF>(QueueCapacity);

            _recogniser = new Recognizer.Dollar.Recognizer();

            LoadGestures();

            _arduino = new Arduino("COM3");
            _arduino.DataRecieved += ArduinoDataRecievedGradientDetection;
            //_arduino.DataRecieved += ArduinoDataRecievedDollarRecognizer;
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

            foreach (var path in filePaths)
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

            if (e)
                _arduino.DataRecieved += ArduinoDataRecievedGradientDetection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">e[0] has tapped information, e[1-3] has information from the proximity sensors, and e[4] has zoom information</param>
        private void ArduinoDataRecievedGradientDetection(object sender, List<TimePointF> e)
        {
            
            

            const int max = 15;

            var largest = e[0];
            for (var i = 1; i < e.Count; i++)
            {
                if (e[i].X > largest.X)
                    largest = e[i];
            }
            //Debug.Write("");

            if (_list.Any(item => (int) item.X == (int) largest.X))
            {
                return;
            }

            if (_pause == 0)
                _list.Add(largest);
            else
            {
                _pause--;
            }

            if (_list.Count > max)
            {
                _list.RemoveAt(0);
            }

            var xList = new List<int>();
            var yList = new List<int>();

            foreach (var item in _list)
            {
                xList.Add((int) item.X);
                yList.Add((int) item.Y);
            }

            var filtered_list = process(xList);
            var filtered_list_y = process(yList);


            // every time theres a new data, it tries to do a detection
            var result = 0;

            if (filtered_list_y.Count > 0 && _pause == 0)
            {
                var index = 0;

                if (yList.Count != 0)
                    result = Detect_y(yList, filtered_list_y);

                if (result == 1)
                {
                    _pause = 8;

                    _list = new List<TimePointF>();
                }
            }


            //var result = 0;

            //if (filtered_list.Count > 0 && pause == 0)
            //{
            //    var index = 0;

            //    if (xList.Count != 0)
            //        result = Detect(xList, filtered_list);

            //    if (result == 1)
            //    {

            //        pause = 8;

            //        _list = new List<TimePointF>();
            //    }
            //}
        }


        private int Detect_y(List<int> list, List<int> gradient)
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
            int result = 0;

            foreach (var item in gradient)
            {
                //if (item > 20)
                //{
                //    pos = 0;
                //    neg = 0;
                //    count = 0;
                //}
                //else if (item < -10)
                //{
                //    break;
                //}
                if (item > 0)
                {
                    pos++;
                }
                else if (item < 0)
                {
                    neg++;
                }

                result = result + item;
            }

            if (result > 0)
            {
                Debug.WriteLine("DOWN");
                OnScrollHorizontal(new ScrollParameters(new Point(0, 0), new Point(10, 0)));
            }
            else if (result < 0)
            {
                Debug.WriteLine(("UP"));
                OnScrollHorizontal(new ScrollParameters(new Point(10, 0), new Point(0, 0)));
            }
            else
            {
                return 0;
            }

            return 1;
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

            if (pos > neg && count > 8)
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


        private void ArduinoDataRecievedDollarRecognizer(object sender, List<TimePointF> e)
        {
            var largest = e[0];
            //for (var i = 1; i < e.Count; i++)
            //{
            //    if (e[i].X > largest.X)
            //        largest = e[i];
            //}
            //Debug.Write("");


            if (_queue.Count == QueueCapacity)
                _queue.Dequeue();


            if (_counter == 0)
            {
                _queue.Enqueue(largest);
            }
            else
            {
                _counter--;

                return;
            }
            //if (_queue.Count != 0)
            //{
            //    //var difference = Math.Abs(_queue.LastOrDefault().X - e.X);
            //    //Debug.WriteLine(difference);
            //    //Debug.WriteLine(_queue.LastOrDefault().X);
            //    //Debug.WriteLine(e.X);
            //    //Debug.WriteLine("--------------------------");


            //    //if (difference > 0)
            //    _queue.Enqueue(largest);
            //    //foreach (var item in _queue.ToList())
            //    //    Debug.Write(item.X + "\t");
            //    //Debug.WriteLine("");
            //    //Debug.WriteLine("+++++++++++++++++++++++++");
            //}
            //else
            //{
            //    _queue.Enqueue(largest);
            //}


            if (_queue.Count < QueueCapacity)
                return;


            var diff = _queue.Max(x => x.X) - _queue.Min(x => x.X);


            NBestList result;
            //if (diff > 15)
            //{
            //Debug.WriteLine(diff);
            result = _recogniser.Recognize(_queue.ToList(), false);
            //}
            //else return;


            if (result.IsEmpty)
                return;


            if (!(result.Score > 0.6))
            {
                _queue = new Queue<TimePointF>();
                return;
            }


            foreach (var item in _queue.ToList())
                Debug.Write(item.X + "\t");
            Debug.WriteLine("");

            _counter = 50;

            var output = String.Format("{0}, {1}, {2}, {3}{4}", result.Name,
                Math.Round(result.Score, 2),
                Math.Round(result.Distance, 2),
                Math.Round(result.Angle, 2), (char) 176);

            _queue = new Queue<TimePointF>();

            Debug.WriteLine(output);

            _arduino.IsEnabled = false;

            //_arduino.DataRecieved -= ArduinoDataRecievedDollarRecognizer;


            CreateGestureEvents(e[0], result);
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

        private enum Zoom
        {
            In = 1,
            Out = -1,
            None = 0
        }
    }
}