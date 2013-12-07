using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.Maps.MapControl.WPF;
using SmartWatch.Core.Gestures;
using SmartWatch.Core.Mocks;
using SmartWatch.Maps.Annotations;

namespace SmartWatch.Maps
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Data Model

        private string _lastGesture;
        private const int HorizontalScrollDistance = 100;
        private const int VerticalScrollDistance = 50;

        public string LastGesture
        {
            get { return _lastGesture; }
            set
            {
                if (value == _lastGesture) return;
                _lastGesture = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            //var gestures = new RandomGesturesProvider();
            //var gestures = new ScrollGestureProviderMocks();
            var gestures = new GestureRecognition();
            gestures.ZoomIn += GesturesZoomIn;
            gestures.ZoomOut += GesturesZoomOut;
            gestures.ScrollLeft += GesturesScrollLeft;
            gestures.ScrollRight += GesturesScrollRight;
            gestures.ScrollUp += GesturesScrollUp;
            gestures.ScrollDown += GesturesScrollDown;
        }

        
        #endregion

        #region Gesture Handlers

        private void GesturesScrollRight(object sender, EventArgs eventArgs)
        {
            LastGesture = "Scroll Right";
            MapControl.Dispatcher.Invoke(() => MoveHorizontally(HorizontalScrollDistance));
        }

        private void GesturesScrollLeft(object sender, EventArgs eventArgs)
        {
            LastGesture = "Scroll Left";
            MapControl.Dispatcher.Invoke(() => MoveHorizontally(-HorizontalScrollDistance));
        }

        private void GesturesScrollDown(object sender, EventArgs e)
        {
            LastGesture = "Scroll Down";
            MapControl.Dispatcher.Invoke(() => MoveVertically(-VerticalScrollDistance));
        }

        private void GesturesScrollUp(object sender, EventArgs e)
        {
            LastGesture = "Scroll Up";
            MapControl.Dispatcher.Invoke(() => MoveVertically(VerticalScrollDistance));
        }

        private void GesturesZoomIn(object sender, EventArgs eventArgs)
        {
            LastGesture = "Zoom In";

            //Invoke the action on the UI thread that owns MapControl
            MapControl.Dispatcher.Invoke(() => ChangeZoomLevel(1));
        }

        private void GesturesZoomOut(object sender, EventArgs eventArgs)
        {
            LastGesture = "Zoom Out";

            //Invoke the action on the UI thread that owns MapControl
            MapControl.Dispatcher.Invoke(() => ChangeZoomLevel(-1));
        }

        #endregion

        #region Supporting Methods

        /// <summary>
        ///     Changes the zoom level on the map.
        /// </summary>
        /// <param name="change">Positive value  = zoom in, negative value = zoom out</param>
        private void ChangeZoomLevel(int change)
        {
            var zoomLevel = MapControl.ZoomLevel + change;
            MapControl.SetView(MapControl.Center, zoomLevel);
        }

        private void MoveHorizontally(int change)
        {
            var newChange = change/MapControl.ZoomLevel;

            var location = new Location(MapControl.Center.Latitude, MapControl.Center.Longitude + newChange);
            MapControl.SetView(location, MapControl.ZoomLevel);
        }

        private void MoveVertically(int change)
        {
            var newChange = change/MapControl.ZoomLevel;

            var location = new Location(MapControl.Center.Latitude + newChange, MapControl.Center.Longitude);
            MapControl.SetView(location, MapControl.ZoomLevel);
        }

        #endregion

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