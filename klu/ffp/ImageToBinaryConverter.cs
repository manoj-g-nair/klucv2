using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;


namespace ffp
{
    public class ImageToBinaryConverter : IValueConverter
    {
        /// <summary>
        /// Converts a binary array (byte[]) to a BitmapImage (XAML).
        /// </summary>
        /// <returns>BitmapImage</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && !DBNull.Value.Equals(value))
            {
                
                byte[] data = (byte[]) value;   
                MemoryStream ms = new MemoryStream(data);
                    
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = ms;
                image.EndInit();

                return image;
            }
            return null;
        }

        /// <summary>
        /// Converts a BitmapImage (XAML) a binary array (byte[])
        /// </summary>
        /// <returns>Binary array (byte[])</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte[] data = (byte[]) value;
            MemoryStream ms = new MemoryStream(data);
            
            return ms.ToArray();
        }
    }
}