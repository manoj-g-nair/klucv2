using System;
using System.Collections;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ffp.TrainingDataSetTableAdapters;
using KluSharp;
using Microsoft.Windows.Controls.Ribbon;

namespace ffp
{
    /// <summary>
    /// Interaction logic for ExpressionsDialog.xaml
    /// </summary>
    public partial class ExpressionsDialog : Window
    {
        TrainingDataSet _DataSet;

        /// <summary>
        /// Used as a dummy for thumbnail creation.
        /// </summary>
        /// <returns></returns>
        public bool ThumbnailCallback()
        {
            return false;
        }

        public ExpressionsDialog(ref TrainingDataSet DataSet)
        {
            InitializeComponent();

            _DataSet = DataSet;
            ExpressionsDataGrid.ItemsSource = _DataSet.Expression;
        }

        private void BrowseImage_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            // Configure save file dialog box            
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = "."; // Default file extension
            dlg.Filter = "Imagefiles (*.bmp, *.jpg, *.png, *.tif, *.tga)|*.bmp;*.jpg;*.png;*.tif;*.tga"; // Filter files by extension
            dlg.Title = "Load image";

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                System.Drawing.Bitmap emoticon = new System.Drawing.Bitmap(dlg.FileName);                

                const int thumbnailWidth = 50;
                const int thumbnailHeight = 50;

                System.Drawing.Image.GetThumbnailImageAbort tc = new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback);
                System.Drawing.Image thumbnail = emoticon.GetThumbnailImage(thumbnailWidth, thumbnailHeight, tc, IntPtr.Zero);
                
                System.IO.MemoryStream ms = new System.IO.MemoryStream();                
                thumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                Console.WriteLine("Length: " + ms.ToArray().Length);

                int oid = Convert.ToInt32(button.DataContext);

                TrainingDataSet.ExpressionRow row = _DataSet.Expression.FindByExpressionOID(oid);

                if (row != null && !DBNull.Value.Equals(row))
                {
                    row.Thumbnail = ms.ToArray();
                }

            }  
        }

        private void RemoveImage_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            int oid = Convert.ToInt32(button.DataContext);

            TrainingDataSet.ExpressionRow row = _DataSet.Expression.FindByExpressionOID(oid);

            if (row != null)
            {
                row.Thumbnail = null;
            }
        }
    }
}
