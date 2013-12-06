using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using WobbrockLib;

namespace SmartWatch.Core.Gestures
{
    public class GestureRecognition : GesturesEventInvocator
    {
        private const int QueueCapacity = 25;

        private readonly IArduino _arduino;
        private readonly Recognizer.Dollar.Recognizer _recogniser;
        private int _counter;
        private bool _detectingScrolls = true; // if false, detecting zooms

        private List<TimePointF> _list;
        private int _pause;
        private Zoom _previousZoom = Zoom.None;
        private Queue<TimePointF> _queue;
        private int _result = 1;
        private bool _shouldProcessTaps = true;

        public GestureRecognition()
        {
            _list = new List<TimePointF>();
            _queue = new Queue<TimePointF>(QueueCapacity);

            _recogniser = new Recognizer.Dollar.Recognizer();

            LoadGestures();

            _arduino = new Arduino.Arduino("COM3");


            _arduino.TapRecieved += TapRecieved;
            _arduino.DataRecieved += DetectScrolls;
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
        private void TapRecieved(object sender, bool e)
        {
            if (_shouldProcessTaps)
            {
                _shouldProcessTaps = false;
                Debug.WriteLine("Tapped");
                _queue = new Queue<TimePointF>();
                SetTapsProcessingTimer(5000);
                _detectingScrolls = !_detectingScrolls;


                if (_detectingScrolls)
                {
                    _arduino.DataRecieved -= DetectZoom;
                    _arduino.DataRecieved += DetectScrolls;
                }
                else
                {
                    _arduino.DataRecieved -= DetectScrolls;
                    _arduino.DataRecieved += DetectZoom;
                }
            }
        }

        /// <summary>
        ///     Sets a timer which enables tap processing
        /// </summary>
        /// <param name="milliseconds"></param>
        private void SetTapsProcessingTimer(int milliseconds)
        {
            var timer = new Timer(milliseconds);
            timer.Elapsed += (sender, args) => _shouldProcessTaps = true;
            timer.Enabled = true;
        }

        private void DetectZoom(object sender, List<TimePointF> e)
        {
            // Test for zooming in and out
            if (_counter == 0)
            {
                if ((int) e[3].X == (int) Zoom.In)
                {
                    _list = new List<TimePointF>();
                    OnZoomIn();
                    _counter = 20;
                    return;
                }

                if ((int) e[3].X == (int) Zoom.Out)
                {
                    _list = new List<TimePointF>();
                    OnZoomOut();
                    _counter = 20;
                }
            }
            else
            {
                _counter--;
            }
        }

        /// <summary>
        ///     Detects whether the gesture is left or right or up or down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">e[0-2] has information from the proximity sensors, and e[3] has zoom information</param>
        private void DetectScrolls(object sender, List<TimePointF> e)
        {
            const int max = 15;

            var largest = e[0];
            for (var i = 1; i < e.Count; i++)
            {
                if (e[i].X > largest.X)
                    largest = e[i];
            }
            //Debug.Write("");

            //if (_list.Any(item => (int) item.X == (int) largest.X))
            //{
            //    return;
            //}

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
                //yList.Add((int)item.Y);
                //Debug.Write(item.X);
                //Debug.Write(" ");

                if (item.Y != 50 && _result == 1)
                {
                    _result = 0;
                    yList.Add((int) item.Y);
                    //Debug.Write(item.Y);
                    //Debug.Write(" ");
                }
                else if (_result == 0)
                {
                    yList.Add((int) item.Y);
                    //Debug.Write(item.Y);
                    //Debug.Write(" ");
                }
            }
            //Debug.WriteLine("------------------------------");

            var filtered_list = process(xList);
            var filtered_list_y = process(yList);


            int temp = 0;
            //foreach (var item in yList)
            //{
            //    if (item == 0)
            //    {
            //        temp++;
            //    }
            //}

            //Debug.WriteLine("sensor 0: " + temp);

            //temp = 0;
            //foreach (var item in yList)
            //{
            //    if (item == 50)
            //    {
            //        temp++;
            //    }
            //}

            //Debug.WriteLine("sensor 1: " + temp);

            //temp = 0;
            //foreach (var item in yList)
            //{
            //    if (item == 100)
            //    {
            //        temp++;
            //    }
            //}

            //Debug.WriteLine(temp);

            //Debug.WriteLine("sensor 2: " + temp);


            // every time theres a new data, it tries to do a detection
            temp = 2;

            if (xList.Count > 2)
            {
                temp = xList.Max() - xList.Min();
            }

            //Debug.WriteLine(temp);

            int _pause2 = 0;

            if (temp < 12)
            {
                if (filtered_list_y.Count > 10 && _pause == 0)
                {
                    var index = 0;

                    if (yList.Count != 0)
                        _result = Detect_y(yList, filtered_list_y);

                    if (_result == 1)
                    {
                        _pause = 20;

                        //_pause2 = 10;

                        _list = new List<TimePointF>();
                    }
                }
            }
            else
            {
                if (filtered_list.Count > 10 && _pause == 0)
                {
                    var index = 0;

                    if (xList.Count != 0)
                        _result = Detect(xList, filtered_list);

                    if (_result == 1)
                    {
                        _pause = 10;

                        _list = new List<TimePointF>();
                    }
                }
            }

            //Debug.WriteLine(result);


            //if (_pause2 != 0 && result == 1)
            //{
            //    _pause2 --;
            //}
            //else if (_pause2 == 0 && result == 0)
            //{
        }


        private int Detect_y(List<int> list, List<int> gradient)
        {
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
                OnScrollDown();
            }
            else if (result < 0)
            {
                OnScrollUp();
            }
            else
            {
                return 0;
            }

            return 1;
        }


        private int Detect(List<int> list, List<int> gradient)
        {
            int pos = 0;
            int neg = 0;
            int count = 0;
            int sum = 0;

            foreach (var item in gradient)
            {
                if (item > 20)
                {
                    pos = 0;
                    neg = 0;
                    sum = 0;
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
                    sum = sum + item;
                }
                else if (item < 0)
                {
                    neg++;
                    count++;
                    sum = sum + item;
                }
            }

            //if (sum > 0 && count > 6)
            if (pos > neg && count > 6)
            {
                Debug.WriteLine("LEFT");
                OnScrollLeft();
            }
                //else if (sum < 0 && count > 6)
            else if (neg > pos && count > 6)
            {
                Debug.WriteLine(("RIGHT"));
                OnScrollRight();
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

        private enum Zoom
        {
            In = 1,
            Out = -1,
            None = 0
        }
    }
}