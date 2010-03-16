using System;
using System.IO;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using ffp.TrainingDataSetTableAdapters;

namespace ffp
{
    public class ExpressionIDToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                TableAdapterManager tam = new TableAdapterManager();
                TrainingDataSet dataSet = new TrainingDataSet();
                tam.ExpressionTableAdapter = new ExpressionTableAdapter();

                tam.ExpressionTableAdapter.Fill(dataSet.Expression);

                int eoid = System.Convert.ToInt32(value);

                if (!DBNull.Value.Equals(eoid))
                {
                    TrainingDataSet.ExpressionRow row = dataSet.Expression.FindByExpressionOID(eoid);

                    if (!DBNull.Value.Equals(row) && !DBNull.Value.Equals(row.Thumbnail))
                    {
                        byte[] imageData = row.Thumbnail;

                        if (imageData != null && !DBNull.Value.Equals(imageData))
                        {
                            byte[] data = (byte[])value;
                            MemoryStream ms = new MemoryStream(data);

                            BitmapImage image = new BitmapImage();
                            image.BeginInit();
                            image.StreamSource = ms;
                            image.EndInit();

                            return image;
                        }
                    }
                }

                return null;
            }
            catch (NullReferenceException)
            {
                return null;
            }
            catch (System.Data.StrongTypingException)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return 666;// System.Convert.ToDateTime(value);
            }
            return value;
        }
    }
}