﻿using System.Threading;
using System.Windows;
using SmartWatch.Core.Gestures;
using SmartWatch.Core.Mocks;
using SmartWatch.Core.ProximitySensors;

namespace SmartWatch.Selection
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //var gestures = new GestureRecognition();
            var gestures = new RandomGestures();
            gestures.ScrollHorizontal += ScrollHorizontalGesture;
            gestures.ScrollVertical += ScrollVerticalGestures;
        }

        private void ScrollVerticalGestures(object sender, ScrollParameters e)
        {
            var shouldScrollUp = (ScrollViewerControl.ScrollableHeight > 0) && (ScrollViewerControl.VerticalOffset > 0);

            var shouldScrollDown = (ScrollViewerControl.ScrollableHeight > 0) &&
                                   ((ScrollViewerControl.VerticalOffset + ScrollViewerControl.ViewportHeight) <
                                    ScrollViewerControl.ExtentHeight);

            if (shouldScrollUp)
            {
                while (ScrollViewerControl.VerticalOffset > 0)
                {
                    ScrollVertically(-10);
                }
            }
            else if (shouldScrollDown)
            {
                while ((ScrollViewerControl.VerticalOffset + ScrollViewerControl.ViewportHeight) <
                       ScrollViewerControl.ExtentHeight)
                {
                    ScrollVertically(10);
                }
            }
        }

        private void ScrollVertically(int change)
        {
            var offset = ScrollViewerControl.VerticalOffset + change;
            ScrollViewerControl.Dispatcher.Invoke(() => ScrollViewerControl.ScrollToVerticalOffset(offset));
            Thread.Sleep(10);
        }


        private void ScrollHorizontalGesture(object sender, ScrollParameters e)
        {
            var shouldScrollLeft = (ScrollViewerControl.ScrollableWidth > 0) &&
                                  (ScrollViewerControl.HorizontalOffset > 0) &&
                                  e.StartPoint.x < e.EndPoint.x;

            var shouldScrollRight = (ScrollViewerControl.ScrollableWidth > 0) &&
                                    ((ScrollViewerControl.HorizontalOffset + ScrollViewerControl.ViewportWidth) <
                                     ScrollViewerControl.ExtentWidth) &&
                                     e.StartPoint.x > e.EndPoint.x;

            if (shouldScrollLeft)
            {
                while (ScrollViewerControl.HorizontalOffset > 0)
                {
                    ScrollHorizontally(-10);
                }
            }
            else if (shouldScrollRight)
            {
                while ((ScrollViewerControl.HorizontalOffset + ScrollViewerControl.ViewportWidth) <
                       ScrollViewerControl.ExtentWidth)
                {
                    ScrollHorizontally(10);
                }
            }
        }

        private void ScrollHorizontally(int change)
        {
            var offset = ScrollViewerControl.HorizontalOffset + change;
            ScrollViewerControl.Dispatcher.Invoke(() => ScrollViewerControl.ScrollToHorizontalOffset(offset));
            Thread.Sleep(10);
        }
    }
}