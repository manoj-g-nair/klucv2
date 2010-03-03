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

            try
            {
                klu = new Klu();
                tmpBitmap = new System.Drawing.Bitmap(320, 240);

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

            // Bind data to controls            
            //dataGrid.AutoGenerateColumns = false;            
            dgridExpressions.DataContext = dataSet.Expression;

            //dgridExpressions.Columns.ElementAt(0).Visibility = Visibility.Hidden;
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

            // Set the Interval to what is typed into the corresponding text box, but don't allow values below 100ms.
            captureTimer.Interval = TimeSpan.FromMilliseconds(Convert.ToInt32(captureTimeTextBox.Text));

            // Start the timer
            captureTimer.Start();
        }

        /// <summary>
        /// Stops the internal timer which is responsible for updating the (live) image
        /// and doing the processing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopLiveBt_Click(object sender, RoutedEventArgs e)
        {
            captureTimer.Stop();            
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
            captureTimer.Interval = TimeSpan.FromMilliseconds(value);            
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

        /// <summary>
        /// Call this function to ensure the user has the ability to save
        /// unsaved changes to the dataset. It will open a message box and
        /// asks if the user wants to save those changes.
        /// </summary>
        private void lastChanceSaving()
        {
            if (dataSet.HasChanges())
            {
                MessageBoxResult res = MessageBox.Show("You've made some changes that are not yet saved! Do you wish to save them?",
                    "Prevent data loss?",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question,
                    MessageBoxResult.Yes
                );

                if (res == MessageBoxResult.Yes)
                {
                    tam.UpdateAll(dataSet);
                    dataSet.AcceptChanges();
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            lastChanceSaving();
        }

        private void closeApplication(object sender, RoutedEventArgs e)
        {
            lastChanceSaving();
            Close();
        }

        private void dgridExpressions_CellEditEnding(object sender, Microsoft.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            tam.UpdateAll(dataSet);
        }

        private void dgridTraining_CellEditEnding(object sender, Microsoft.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            tam.UpdateAll(dataSet);
        }

        private void deleteExpressions_Click(object sender, RoutedEventArgs e)
        {
            //dgridExpressions.sel
        }
    }
}
