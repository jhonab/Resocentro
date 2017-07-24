using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Resocentro_Desktop.Complemento
{
   public class BrushColorConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var converter = new System.Windows.Media.BrushConverter();
            if ((bool)value)
            {
                //return new SolidColorBrush(Colors.Black);
                return (Brush)converter.ConvertFromString("#FFAE4C4C");
            }
            //return new SolidColorBrush(Colors.White);
            return (SolidColorBrush)App.Current.Resources["Windows.Background"];
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

   public class ValueBoolConverter : IValueConverter
   {
       public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
       {
           return !(bool)value;
       }


       public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
       {
           throw new NotImplementedException();
       }
   }
}
