using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace UMDH.Visualizer
{
    public class ModuleToColorConverter : IValueConverter
    {
        private static Dictionary<string, Color> mColors = new Dictionary<string, Color>();
        private static Random mRand = new Random();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            var moduleName = value.ToString();
            if (!mColors.ContainsKey(moduleName))
            {
                var r = (byte)(mRand.Next() % 256);
                var g = (byte)(mRand.Next() % 256);
                var b = (byte)(mRand.Next() % 256);

                while (r + g + b > 500 || r + g > 2 * b)
                {
                    r = (byte)(mRand.Next() % 256);
                    g = (byte)(mRand.Next() % 256);
                    b = (byte)(mRand.Next() % 256);
                }

                mColors[moduleName] = Color.FromArgb(255, r, g, b);
            }
            return new SolidColorBrush(mColors[moduleName]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
