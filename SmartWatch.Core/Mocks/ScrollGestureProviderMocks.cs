using System.Threading;
using System.Threading.Tasks;
using SmartWatch.Core.Gestures;

namespace SmartWatch.Core.Mocks
{
    public class ScrollGestureProviderMocks : GesturesEventInvocator
    {
        public ScrollGestureProviderMocks()
        {
            Task.Run(() => GenerateGestureEvents());
        }

        private void GenerateGestureEvents()
        {
            Thread.Sleep(1000);
            while (true)
            {
                OnScrollLeft();
                Thread.Sleep(500);
                OnScrollUp();
                Thread.Sleep(500);
                OnScrollRight();
                Thread.Sleep(500);
                OnScrollDown();
                Thread.Sleep(500);
            }
        }
    }
}