using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.Maps.MapControl.WPF;
using SmartWatch.Core.Mocks;
using SmartWatch.Maps.Annotations;

namespace SmartWatch.Maps
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string _lastGesture;

        public string LastGesture
        {
            get {
                return String.IsNullOrEmpty(_lastGesture) 
                    ? "No gestures detected yet." 
                    : _lastGesture;
            }
            set
            {
                _lastGesture = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            var gestures = new MockGestures();
            gestures.PinchIn += GesturesPinchIn;
            gestures.PinchOut += gestures_PinchOut;
            gestures.ScrollHorizontal += gestures_ScrollHorizontal;
            gestures.ScrollVertical += gestures_ScrollVertical;
            gestures.ScrollDiagonal += gestures_ScrollDiagonal;
        }

        void gestures_ScrollDiagonal(object sender, Core.Gestures.GestureParameters e)
        {
            LastGesture = "Scroll Diagonal";
        }

        void gestures_ScrollVertical(object sender, Core.Gestures.GestureParameters e)
        {
            LastGesture = "Scroll Vertical";
        }

        void gestures_ScrollHorizontal(object sender, Core.Gestures.GestureParameters e)
        {
            LastGesture = "Scroll Horizontal";
        }

        void GesturesPinchIn(object sender, Core.Gestures.GestureParameters e)
        {
            LastGesture = "Pinch In";

            //Invoke the action on the UI thread that owns MapControl
            MapControl.Dispatcher.Invoke(() => ChangeZoomLevel(1));
        }

        void gestures_PinchOut(object sender, Core.Gestures.GestureParameters e)
        {
            LastGesture = "Pinch Out";

            //Invoke the action on the UI thread that owns MapControl
            MapControl.Dispatcher.Invoke(() => ChangeZoomLevel(-1));
        }

        /// <summary>
        /// Changes the zoom level on the map. 
        /// </summary>
        /// <param name="change">Positive value  = zoom in, negative value = zoom out</param>
        private void ChangeZoomLevel(int change)
        {
            var zoomLevel = MapControl.ZoomLevel + change;
            MapControl.SetView(MapControl.Center, zoomLevel);
        }


        #region INotifyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
