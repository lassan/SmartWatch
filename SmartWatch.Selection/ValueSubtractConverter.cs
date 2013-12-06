using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SmartWatch.Selection
{
 public    class ValueSubtractConverter :IValueConverter
    {
     public int SubtractValue { get; set; }
     public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
     {
         return (double) value - SubtractValue;
     }

     public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
     {
         throw new NotImplementedException();
     }
    }
}
