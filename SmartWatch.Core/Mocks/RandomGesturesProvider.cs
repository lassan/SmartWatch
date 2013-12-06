using System;
using System.Timers;

namespace SmartWatch.Core.Mocks
{
    /// <summary>
    ///     Mock object that invokes the event for a random gesture every second or so
    /// </summary>
    public class RandomGesturesProvider : GesturesEventInvocator
    {
        private readonly Random _randomNumGenerator;

        public RandomGesturesProvider()
        {
            _randomNumGenerator = new Random(5000);
            var timer = new Timer(1500);
            timer.Elapsed += timer_Elapsed;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var num = _randomNumGenerator.Next(0, 6);

            switch (num)
            {
                case 0:
                    OnZoomIn();
                    break;
                case 1:
                    OnZoomOut();
                    break;
                case 2:
                    OnScrollLeft();
                    break;
                case 3:
                    OnScrollRight();
                    break;
                case 4:
                    OnScrollUp();
                    break;
                case 5:
                    OnScrollDown();
                    break;
                default:
                    throw new ArgumentException("Random number should be restricted to a maximum of 3.");
            }
        }
    }
}