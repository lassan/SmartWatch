using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
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

            var gestures = new RandomGestures();
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
            LastGesture = "Scroll Vertical";
        }

        private void gestures_ScrollHorizontal(object sender, ScrollParameters e)
        {
            LastGesture = "Scroll Horizontal";
        }

        private void GesturesPinchIn(object sender, PinchParameters e)
        {
            LastGesture = "Pinch In";

            //Invoke the action on the UI thread that owns MapControl
            MapControl.Dispatcher.Invoke(() => ChangeZoomLevel(1));
        }

        private void gestures_PinchOut(object sender, PinchParameters e)
        {
            LastGesture = "Pinch Out";

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