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
        private readonly IArduino _arduino;
        private int _counter;
        private bool _detectingScrolls = true; // if false, detecting zooms

        private List<TimePointF> _list;
        private int _pause;
        private bool _result = true;
        private bool _shouldProcessTaps = true;

        public GestureRecognition()
        {
            _list = new List<TimePointF>();
            
            _arduino = new Arduino.Arduino("COM3");
            _arduino.TapRecieved += TapRecieved;
            _arduino.DataRecieved += DetectScrolls;
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
                _list = new List<TimePointF>();
                SetTapsProcessingTimer(10000);
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

            if (_pause == 0)
                _list.Add(largest);
            else
                _pause--;

            if (_list.Count > max)
            {
                _list.RemoveAt(0);
            }

            var xList = new List<int>();
            var yList = new List<int>();
            var tempList = new List<int>();

            foreach (var item in _list)
            {
                tempList = xList;
                tempList.Add((int) item.X);

                if (tempList.Max() - tempList.Min() > 6)
                {
                    xList.Add((int) item.X);
                }

                if ((int) item.Y != 50 && _result)
                {
                    _result = false;
                    yList.Add((int) item.Y);
                }
                else if (!_result)
                {
                    yList.Add((int) item.Y);
                }
            }

            var filteredList = Process(xList);
            var filteredListY = Process(yList);

            // every time theres a new data, it tries to do a detection
            var temp = 0;

            if (xList.Count >= 3)
            {
                temp = xList.Max() - xList.Min();
            }

            if (temp < 15)
            {
                if (filteredListY.Count > 10 && _pause == 0)
                {
                    if (yList.Count != 0)
                        _result = DetectY(filteredListY);

                    if (_result)
                    {
                        _pause = 30;
                        _list = new List<TimePointF>();
                    }
                }
            }
            else
            {
                if (filteredList.Count > 10 && _pause == 0)
                {
                    if (xList.Count != 0)
                        _result = DetectX(filteredList);

                    if (_result)
                    {
                        _pause = 30;

                        _list = new List<TimePointF>();
                    }
                }
            }
        }

        private bool DetectY(IEnumerable<int> gradient)
        {
            var result = gradient.Aggregate(0, (current, item) => current + item);

            if (result > 0)
                OnScrollDown();
            else if (result < 0)
                OnScrollUp();
            else
                return false;

            return true;
        }

        private bool DetectX(IEnumerable<int> gradient)
        {
            var pos = 0;
            var neg = 0;
            var count = 0;
            var sum = 0;

            foreach (var item in gradient)
            {
                if (item > 40)
                {
                    pos = 0;
                    neg = 0;
                    sum = 0;
                    count = 0;
                }
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

            if (pos > neg && count > 10)
                OnScrollLeft();

            else if (neg > pos && count > 10)
                OnScrollRight();

            else
                return false;

            return true;
        }

        // apply filter to the list
        // gradient filter is applied at the moment
        private static List<int> Process(List<int> list)
        {
            var filteredList = new List<int>();

            int[] mask = {-1, 1};

            for (var i = 0; i < list.Count - 1; i++)
            {
                var val = mask[0]*list[i] + mask[1]*list[i + 1];

                filteredList.Add(val);
            }

            return filteredList;
        }

        private enum Zoom
        {
            In = 1,
            Out = -1
        }
    }
}