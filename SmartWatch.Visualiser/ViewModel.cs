using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SmartWatch.Core;
using SmartWatch.Visualiser.Annotations;
using WobbrockLib;

namespace SmartWatch.Visualiser
{
    public class ViewModel : INotifyPropertyChanged
    {
        #region View Properties

        public int Sensor1Proximity
        {
            get { return _sensor1Proximity; }
            set
            {
                if (value == _sensor1Proximity) return;
                _sensor1Proximity = value;
                OnPropertyChanged();
            }
        }

        public int Sensor2Proximity
        {
            get { return _sensor2Proximity; }
            set
            {
                if (value == _sensor2Proximity) return;
                _sensor2Proximity = value;
                OnPropertyChanged();
            }
        }

        public int Sensor3Proximity
        {
            get { return _sensor3Proximity; }
            set
            {
                if (value == _sensor3Proximity) return;
                _sensor3Proximity = value;
                OnPropertyChanged();
            }
        }

        #endregion

        private int _sensor1Proximity;
        private int _sensor2Proximity;
        private int _sensor3Proximity;

        public ViewModel(IArduino arduino)
        {
            arduino.DataRecieved += ArduinoDataRecievedHandler;
        }

        private void ArduinoDataRecievedHandler(object sender, List<TimePointF> e)
        {
            Sensor1Proximity = (int) e[0].X;
            Sensor2Proximity = (int) e[1].X;
            Sensor3Proximity = (int) e[2].X;
        }

        #region INotifyPropertyChanged Members

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