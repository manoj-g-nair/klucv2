using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;

using KluSharp;
using ffp.TrainingDataSetTableAdapters;

namespace ffp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// This member is a wrapper object provides access to the klu C DLL.
        /// </summary>
        private Klu klu;
        private DispatcherTimer captureTimer;
        private System.Drawing.Bitmap tmpBitmap;
        TableAdapterManager tam;
        TrainingDataSet dataSet;

        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            klu = new Klu();
            captureTimer = null; // is this needed?
            tmpBitmap = new System.Drawing.Bitmap(320, 240);

            try
            {
                tam = new TableAdapterManager();  
                dataSet = new TrainingDataSet();

                // Load data from SQL database and fill our DataSet
                tam.ExpressionTableAdapter = new ExpressionTableAdapter();
                tam.EmoticonTableAdapter = new EmoticonTableAdapter();
                tam.TrainingTableAdapter = new TrainingTableAdapter();
                tam.ImageTableAdapter = new ImageTableAdapter();

                LoadData();
            }            
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadData()
        {
            // Clear the complete dataset
            dataSet.Clear();

            // Load data from database and fill dataset
            tam.ExpressionTableAdapter.Fill(dataSet.Expression);
            tam.EmoticonTableAdapter.Fill(dataSet.Emoticon);
            tam.TrainingTableAdapter.Fill(dataSet.Training);
            tam.ImageTableAdapter.Fill(dataSet.Image);

            // Bind data to control
            lviewExpressions.ItemsSource = dataSet.Expression;
            lviewTrainingData.ItemsSource = dataSet.Training;
            
            dataGrid.DataContext = dataSet.Expression;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createCaptureBt_Click(object sender, RoutedEventArgs e)
        {
            if ( klu.CreateCapture() )
            {
                statusText.Text = "Capture Created!";
            }
            else
            {
                statusText.Text = "Failed to create Capture!";
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void freeCaptureBt_Click(object sender, RoutedEventArgs e)
        {
            klu.FreeCapture();
            statusText.Text = "Capture freed!";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void querySingleFrameBt_Click(object sender, RoutedEventArgs e)
        {
            if ( !klu.CreateCapture() )
            {
                return;
            }

            tmpBitmap = klu.QueryCaptureImage();
            klu.SetWpfImageFromBitmap(ref image1, ref tmpBitmap);                        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startLiveBt_Click(object sender, RoutedEventArgs e)
        {
            if ( !klu.CreateCapture() )
            {
                return;
            }

            if (captureTimer == null)
            {
                // Create a Timer with a Normal Priority
                captureTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle, this.Dispatcher);

                // Set the callback to just show the time ticking away
                // NOTE: We are using a control so this has to run on 
                // the UI thread
                captureTimer.Tick += new EventHandler(
                    delegate(object s, EventArgs a)
                    {
                        klu.QueryCaptureImage2(ref tmpBitmap);
                        //tmpBitmap = klu.QueryCaptureImage();
                        klu.SetWpfImageFromBitmap(ref image1, ref tmpBitmap);
                    }
                );
            }

            // Set the Interval to what is typed into the corresponding text box, but don't allow values below 100ms.
            captureTimer.Interval = TimeSpan.FromMilliseconds(Convert.ToInt32(captureTimeTextBox.Text));

            // Start the timer
            captureTimer.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopLiveBt_Click(object sender, RoutedEventArgs e)
        {
            if (captureTimer != null)
            {
                captureTimer.Stop();
                klu.FreeCapture();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void captureTimeTextBox_TextInput(object sender, TextCompositionEventArgs e)
        {
            int value = Math.Min(1, Convert.ToInt32(captureTimeTextBox.Text));
            captureTimeTextBox.Text = Convert.ToString(value);

            // Also adjust the timer.
            if (captureTimer != null)
            {
                captureTimer.Interval = TimeSpan.FromMilliseconds(value);
            }
        }

        private void processVideo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void processStill_Click(object sender, RoutedEventArgs e)
        {

        }

        private void processCamera_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }

        private void closeApplication(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
