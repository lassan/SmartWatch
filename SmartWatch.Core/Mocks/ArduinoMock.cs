using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace SmartWatch.Core.Mocks
{
    public class ArduinoMock
    {
        private readonly Random _randomNumGenerator;

        public ArduinoMock()
        {
            _randomNumGenerator = new Random(500);

            Task.Run(()=>GenerateIncrementingNumbers());

            //var timer = new Timer(100);
            //timer.Elapsed += timer_Elapsed;
            //timer.AutoReset = true;
            //timer.Enabled = true;
        }

        public void GenerateIncrementingNumbers()
        {
            int[] num = new int[] {5, 1, 2, 5, 6, 3, 1 , 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11};

            foreach (int item in num)
            //while (true)
            {
                OnDataRecieved(item);
                //num = 2* num;
                Thread.Sleep(100);
            }
        }

        public event EventHandler<int> DataRecieved;

        protected virtual void OnDataRecieved(int e)
        {
            var handler = DataRecieved;
            if (handler != null) handler(this, e);
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var num = _randomNumGenerator.Next(0, 500);
            OnDataRecieved(num);

        }

        public void OpenConnection()
        {
        }
    }
}
