using System;
using System.Data;
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
using ffp.TrainingDataSetTableAdapters;
using KluSharp;

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

        /// <summary>
        /// This is the timer used to query camera images.
        /// </summary>
        private DispatcherTimer captureTimer;

        /// <summary>
        /// This is a temporary bitmap which acts as a container for camera images.
        /// </summary>
        private System.Drawing.Bitmap tmpBitmap;

        /// <summary>
        /// This stores all table adapters.
        /// </summary>
        TableAdapterManager tam;

        /// <summary>
        /// This is the dataset which contains the connected database in memory. 
        /// </summary>
        TrainingDataSet dataSet;

        /// <summary>
        /// This is a small wrapper around the structure NOT the logic of an ANN.
        /// </summary>
        ANN ann;

        /// <summary>
        /// This contains all the information about the hidden layers of the ANN.
        /// We use a DataSet for this purpose because it can be easily sychronized with
        /// a DataGrid.
        /// </summary>
        DataSet dataSetAnn;

        /// <summary>
        /// The main entry point for this window.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                #region Initialize ANN stuff
                ann = new ANN();
                ann.NumLayers = 2;
                ann.SetNumNeurons(0, 4);
                ann.SetNumNeurons(1, 1);

                // Bind DataGrid to DataSet
                dataSetAnn = new DataSet("HiddenLayer");
                dataSetAnn.Tables.Add("HiddenLayerTable");
                uint tmp = 0;
                dataSetAnn.Tables[0].Columns.Add("NeuronsColumn", tmp.GetType());
                tmp = 3;
                dataSetAnn.Tables[0].Rows.Add(tmp);
                dgridHiddenLayer.DataContext = dataSetAnn.Tables[0];
                #endregion

                #region Intialize encapsulated OpenCV subsystem
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
                #endregion

                #region "Connect" to database
                tam = new TableAdapterManager();  
                dataSet = new TrainingDataSet();

                // Load data from SQL database and fill our DataSet
                tam.ExpressionTableAdapter = new ExpressionTableAdapter();
                tam.EmoticonTableAdapter = new EmoticonTableAdapter();
                tam.TrainingTableAdapter = new TrainingTableAdapter();
                tam.ImageTableAdapter = new ImageTableAdapter();

                LoadData();
                #endregion
            }            
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Bind some DataGrids to the Database
        /// </summary>
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
            dgridExpressions.DataContext = dataSet.Expression;
            dgridTraining.DataContext = dataSet.Training;      
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
        /// Callback which start the camera live view.
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
        /// Callback which sets the capture time interval in ms.
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

        /// <summary>
        /// Starts the processing of video images.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void processVideo_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Starts the processing of still images.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void processStill_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Starts the processing of camera images.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    try
                    {
                        tam.UpdateAll(dataSet);
                        dataSet.AcceptChanges();
                    }
                    catch (Exception e)
                    {
                        res = MessageBox.Show(e.Message, "An unhandled exception occured!", MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.No);

                        if (res == MessageBoxResult.Yes)
                        {
                            throw e;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Callback which is called when the window is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            lastChanceSaving();
        }

        /// <summary>
        /// Callback which is called when the application closes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeApplication(object sender, RoutedEventArgs e)
        {
            lastChanceSaving();
            Close();
        }

        /// <summary>
        /// Callback which stops the training process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopTrainingMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Callback which starts the training process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startTrainingMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Draws the ANN on the appropriate Canvas.
        /// </summary>
        private void DrawANN()
        {
            #region Draw the Neurons and weights in one go

            // Clear Canvas
            annCanvas.Children.Clear();

            // Save positions of neurons in this array
            Point[] neuronPos = new Point[ann.GetTotalNumberOfNeurons()];

            double layerDist = (double)myWindow.Width / (double)ann.NumLayers;
            double neuronDiameter = 20.0;


            RadialGradientBrush neuronBrush = new RadialGradientBrush();
            neuronBrush.RadiusX = 1.0;
            neuronBrush.RadiusY = 1.0;
            neuronBrush.GradientOrigin = new Point(0.7, 0.3);
            neuronBrush.GradientStops.Add(new GradientStop(Colors.White, 0.0));
            neuronBrush.GradientStops.Add(new GradientStop(Colors.Black, 1.0));

            // Iterate over every layer
            for (int l = 0; l < ann.NumLayers; l++)
            {
                double neuronDist = (double)myWindow.Height / (double)ann.GetNumNeurons(l);

                // Iterare over every neuron on the current layer
                for (int n = 0; n < ann.GetNumNeurons(l); n++)
                {
                    Ellipse e = new Ellipse();
                    e.Stroke = Brushes.Blue;
                    e.Width = neuronDiameter;
                    e.Height = neuronDiameter;
                    e.StrokeThickness = 2.0;
                    e.Fill = Brushes.Blue;
                    Canvas.SetLeft(e, layerDist * l);
                    Canvas.SetTop(e, neuronDist * n);
                    annCanvas.Children.Add(e);

                    neuronPos[ann.GetNumberOfNeuronsBefore(l) + n] = new Point(layerDist * l, neuronDist * n);

                    if (l > 0)
                    {
                        // Draw all connections from previous layer to current neuron.                     
                        for (int i = 0; l > 0 && i < ann.GetNumNeurons(l - 1); i++)
                        {
                            Line line = new Line();
                            line.X1 = neuronPos[ann.GetNumberOfNeuronsBefore(l - 1) + i].X + neuronDiameter / 2.0;
                            line.Y1 = neuronPos[ann.GetNumberOfNeuronsBefore(l - 1) + i].Y + neuronDiameter / 2.0;
                            line.X2 = neuronPos[ann.GetNumberOfNeuronsBefore(l) + n].X + neuronDiameter / 2.0;
                            line.Y2 = neuronPos[ann.GetNumberOfNeuronsBefore(l) + n].Y + neuronDiameter / 2.0;
                            line.Stroke = Brushes.Blue;
                            line.StrokeThickness = 1;
                            annCanvas.Children.Add(line);
                        }
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// Calback which redraws the ANN onto the canvas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void annCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawANN();
        }

        /// <summary>
        /// Callback which takes the ANN DataSet for hidden layers and draws the ANN
        /// using the using the new hidden layers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyAnnSettings_Click(object sender, RoutedEventArgs e)
        {
            ann.NumLayers = 2 + dataSetAnn.Tables[0].Rows.Count;
            ann.SetNumNeurons(0, 4);
            ann.SetNumNeurons(dataSetAnn.Tables[0].Rows.Count + 1, 1);

            for (int i = 0; i < dataSetAnn.Tables[0].Rows.Count; i++)
            {
                ann.SetNumNeurons(i + 1, Convert.ToInt32(dataSetAnn.Tables[0].Rows[i].ItemArray[0]));
            }

            DrawANN();
        }
    }
}
