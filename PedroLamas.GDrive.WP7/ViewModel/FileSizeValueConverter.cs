using System;
using System.Windows.Data;

namespace PedroLamas.GDrive.ViewModel
{
    public class FileSizeValueConverter : IValueConverter
    {
        private static readonly string[] _availableSuffixes = new string[] { "B", "KB", "MB", "GB", "TB" };

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var size = (int)value;
            var suffixIndex = 0;

            if (size <= 1024)
                return "1 KB";

            while (size > 1024)
            {
                size = size / 1024;

                suffixIndex++;
            }

            return string.Format("{0} {1}", size, _availableSuffixes[suffixIndex]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}