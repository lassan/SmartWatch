using System;

namespace SmartWatch.Core.Gestures
{
    /// <summary>
    ///     TODO Perhaps it makes more sense to have a discrete set of parameters
    /// </summary>
    public class GestureParameters
    {
        #region Backing Fields

        private int _x0;
        private int _x1;
        private int _y0;
        private int _y1;

        #endregion

        #region Constructor

        public GestureParameters(int x0, int x1, int y0, int y1)
        {
            //TODO - Do some validation 

            Y1 = y1;
            Y0 = y0;
            X1 = x1;
            X0 = x0;
        }

        #endregion

        #region Data Model

        public int X0
        {
            get { return _x0; }
            private set
            {
                ValidateXCoordinate(value);
                _x0 = value;
            }
        }

        public int X1
        {
            get { return _x1; }
            private set
            {
                ValidateXCoordinate(value);
                _x1 = value;
            }
        }

        public int Y0
        {
            get { return _y0; }
            private set
            {
                ValidateYCoordinate(value);
                _y0 = value;
            }
        }

        public int Y1
        {
            get { return _y1; }
            private set
            {
                ValidateYCoordinate(value);
                _y1 = value;
            }
        }

        #endregion

        #region Validation Methods

        private void ValidateXCoordinate(int value)
        {
            if (value < 0 || value > 10)
                throw new ArgumentOutOfRangeException();
        }

        public void ValidateYCoordinate(int value)
        {
            if (value < 0 || value > 10)
                throw new ArgumentOutOfRangeException();
        }

        #endregion
    }
}