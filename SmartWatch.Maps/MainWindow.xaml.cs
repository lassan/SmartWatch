using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.Maps.MapControl.WPF;
using SmartWatch.Core.Gestures;
using SmartWatch.Core.Mocks;
using SmartWatch.Core.ProximitySensors;
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

            //var gestures = new RandomGestures();
            var gestures = new GestureRecognition();
            gestures.PinchIn += GesturesPinchIn;
            gestures.PinchOut += gestures_PinchOut;
            gestures.ScrollHorizontal += gestures_ScrollHorizontal;
            gestures.ScrollVertical += gestures_ScrollVertical;
            gestures.ScrollDiagonal += gestures_ScrollDiagonal;
        }

        #endregion

        #region Gesture Handlers

        private void gestures_ScrollDiagonal(object sender, ScrollParameters e)
        {
            LastGesture = "Scroll Diagonal";
        }

        private void gestures_ScrollVertical(object sender, ScrollParameters e)
        {
            var shouldScrollUp = e.StartPoint.y > e.EndPoint.y;
            var shouldScrolldown = e.StartPoint.y < e.EndPoint.y;


            const int scrollDistance = 100;

            if (shouldScrollUp)
            {
                LastGesture = "Scroll Up";

                MapControl.Dispatcher.Invoke(() => MoveVertically(scrollDistance));
            }
            else if (shouldScrolldown)
            {
                LastGesture = "Scroll Down";

                MapControl.Dispatcher.Invoke(() => MoveVertically(-scrollDistance));
            }
        }

        private void gestures_ScrollHorizontal(object sender, ScrollParameters e)
        {
            var shouldScrollLeft = e.StartPoint.x < e.EndPoint.x;
            var shouldScrollRight = e.StartPoint.x > e.EndPoint.x;

            const int scrollDistance = 100;

            if (shouldScrollLeft)
            {
                LastGesture = "Scroll Left";

                MapControl.Dispatcher.Invoke(() => MoveHorizontally(scrollDistance));
            }
            else if (shouldScrollRight)
            {
                LastGesture = "Scroll Right";

                MapControl.Dispatcher.Invoke(() => MoveHorizontally(-scrollDistance));
            }
        }

        private void GesturesPinchIn(object sender, PinchParameters e)
        {
            LastGesture = "Zoom In";

            //Invoke the action on the UI thread that owns MapControl
            MapControl.Dispatcher.Invoke(() => ChangeZoomLevel(1));
        }

        private void gestures_PinchOut(object sender, PinchParameters e)
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
            var newChange = change / MapControl.ZoomLevel;

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