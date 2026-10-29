
using System.Windows.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace ManagementSystem.WPF.UI.Converters
{
    public class FileToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = value as string;
            BitmapImage bmp;
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                bmp = new BitmapImage();
                bmp.BeginInit();
                // Load immediately to free the file
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.UriSource = new Uri(path);
                bmp.EndInit();
            }
            else
            {
                GlobalFunctions g = App.GetRequiredService<GlobalFunctions>();
                bmp = g.ImageError;
            }
            // Return a default image embedded as a Resource if file doesn't exist
            return bmp;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
