using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Cryptography;
using SmartWatch.Core.Gestures;
using SmartWatch.Core.Mocks;

namespace SmartWatch.Core.ProximitySensors
{
    public class Gestures : IGestures
    {
        List<int> list = new List<int>();

        public Gestures()
        {
            //var arduino = new Arduino();
            var arduino = new ArduinoMock();
            arduino.DataRecieved += arduino_DataRecieved;
            arduino.OpenConnection();
        }

        List<int> detect(List<int> list, List<int> gradient)
        {
            var min = 0;
            var max = 100;

            // gesture is only valid if the hand is moving half of the maximun distance

            var valid_dist = (max - min)/2;

            var number = 5;

            var b = 0;

            int ii = 0;
            int jj = 0;

            for (int i = 0; i < gradient.Count - number; i++)
            {
                for (int j = 0; j < number; j++)
                {
                    if (gradient[i + j] < 0)
                    {
                        b = 1;
                        jj = j;
                        break;
                    }
                }

                if (b == 1)
                {
                    ii = i;
                    break;
                }
            }

            if (b == 0 && gradient.Count >= number)
            {
                Debug.WriteLine("scroll to the right");

                foreach (int item in list)
                {
                    Debug.WriteLine(item);
                }

                Debug.WriteLine("11111111111111111");
                OnScrollHorizontal(new ScrollParameters(new Point(0,50), new Point(100, 50) ));

                list.RemoveRange(0, ii + jj);

                foreach (int item in list)
                {
                    Debug.WriteLine(item);
                }
            }

            return list;
        }

        // apply filter to the list
        // gradient filter is applied at the moment
        List<int> process(List<int> list)
        {
            List<int> filtered_list = new List<int>();

            int[] mask = new int[] {-1, 1};

            int val;

            for (int i = 0; i < list.Count - 1; i++)
            {
                val = mask[0] * list[i] + mask[1] * list[i + 1];
                
                filtered_list.Add(val);
            }

            return filtered_list;
        }

        void arduino_DataRecieved(object sender, int e)
        {

            list.Add(e);

            List<int> filtered_list = process(list);

            // every time theres a new data, it tries to do a detection
            list = detect(list, filtered_list);

            Debug.WriteLine("start");


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