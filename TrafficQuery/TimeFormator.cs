using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TrafficQuery
{
    class TimeFormator : IValueConverter
    {
        public static string Hour = "小时";
        public static string Minute = "分钟";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sb = new StringBuilder();
            int minutes = (int)value;
            int hour = minutes / 60;
            int minute = minutes % 60;
            if (hour > 0)
            {
                sb.Append(hour.ToString());
                sb.Append(" ");
                sb.Append(Hour);
                sb.Append(" ");
            }
            sb.Append(minute.ToString());
            sb.Append(" ");
            sb.Append(Minute);
            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
