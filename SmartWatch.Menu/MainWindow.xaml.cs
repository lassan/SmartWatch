using System;
using System.Threading;
using System.Windows;
using SmartWatch.Core.Mocks;

namespace SmartWatch.Menu
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int HorizontalScrollStep = 10;
        private const int VerticalScrollStep = 10;

        public MainWindow()
        {
            InitializeComponent();
            var gestures = new RandomGesturesProvider();
            //var gestures = new GestureProviderRecognition();

            gestures.ScrollRight += GesturesScrollRight;
            gestures.ScrollLeft += GesturesScrollLeft;
            gestures.ScrollUp += GesturesScrollUp;
            gestures.ScrollDown += GesturesScrollDown;
        }

        private void GesturesScrollDown(object sender, EventArgs e)
        {
            while ((ScrollViewerControl.VerticalOffset + ScrollViewerControl.ViewportHeight) <
                   ScrollViewerControl.ExtentHeight)
            {
                ScrollVertically(VerticalScrollStep);
            }
        }

        private void GesturesScrollUp(object sender, EventArgs e)
        {
            while (ScrollViewerControl.VerticalOffset > 0)
            {
                ScrollVertically(-VerticalScrollStep);
            }
        }

        private void GesturesScrollLeft(object sender, EventArgs eventArgs)
        {
            while (ScrollViewerControl.HorizontalOffset > 0)
            {
                ScrollHorizontally(-HorizontalScrollStep);
            }
        }

        private void GesturesScrollRight(object sender, EventArgs eventArgs)
        {
            while ((ScrollViewerControl.HorizontalOffset + ScrollViewerControl.ViewportWidth) <
                   ScrollViewerControl.ExtentWidth)
            {
                ScrollHorizontally(HorizontalScrollStep);
            }
        }

        private void ScrollVertically(int change)
        {
            var offset = ScrollViewerControl.VerticalOffset + change;
            ScrollViewerControl.Dispatcher.Invoke(() => ScrollViewerControl.ScrollToVerticalOffset(offset));
            Thread.Sleep(10);
        }

        private void ScrollHorizontally(int change)
        {
            var offset = ScrollViewerControl.HorizontalOffset + change;
            ScrollViewerControl.Dispatcher.Invoke(() => ScrollViewerControl.ScrollToHorizontalOffset(offset));
            Thread.Sleep(10);
        }
    }
}