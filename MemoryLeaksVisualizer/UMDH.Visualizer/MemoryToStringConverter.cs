using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UMDH.Visualizer
{
    public class MemoryToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var memory = System.Convert.ToInt64(value);
            if (memory < 1024)
            {
                return memory + " Bytes";
            }
            else if (memory < 1024 * 1024)
            {
                return (memory / 1024) + " KBytes (" + memory + " Bytes)";
            }
            else if (memory < 1024 * 1024 * 1024)
            {
                return (memory / 1024 * 1024) + " MB (" + memory + " Bytes)";
            }
            else
            {
                return (memory / 1024 * 1024 * 1024) + " GB (" + memory + " Bytes)";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
