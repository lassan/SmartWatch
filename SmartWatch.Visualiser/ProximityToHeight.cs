using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SmartWatch.Visualiser
{
    public class ProximityToHeight : IMultiValueConverter
    {
        public int MaxProximity { get; set; }

        public int Margin { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var proximity = (int) values[0];
            var windowHeight = (double) values[1];

            var height = Math.Round(((double) proximity/MaxProximity)* (windowHeight - Margin),0);

            return height;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
