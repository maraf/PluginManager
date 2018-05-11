using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PackageManager.Views.Converters
{
    public class DropNewLineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string target = value?.ToString();
            if (String.IsNullOrEmpty(target))
                return target;

            target = target.Replace(Environment.NewLine, String.Empty);
            target = target.Replace("\n", String.Empty);
            return target;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
