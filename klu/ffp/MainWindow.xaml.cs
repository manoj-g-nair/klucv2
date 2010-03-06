using System;
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

                // Bind certain labels to ANN stuff
                annNumLayers.DataContext = ann;

                // Bind DataGrid to DataSet
                dataSetAnn = new DataSet("HiddenLayer");
                dataSetAnn.Tables.Add("HiddenLayerTable");
                uint tmp = 0;
                dataSetAnn.Tables[0].Columns.Add("NeuronsColumn", tmp.GetType());
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
        private void drawANN()
        {
            // Clear Canvas
            annCanvas.Children.Clear();

            // Save positions of neurons in this array
            Point[] neuronPos = new Point[ann.GetTotalNumberOfNeurons()];

            double neuronDiameter = 20.0;
            double layerDist = (double)annCanvas.ActualWidth / (double)(ann.NumLayers-1) - neuronDiameter;

            RadialGradientBrush neuronBrush = new RadialGradientBrush();
            neuronBrush.RadiusX = 1.0;
            neuronBrush.RadiusY = 1.0;
            neuronBrush.GradientOrigin = new Point(0.7, 0.3);
            neuronBrush.GradientStops.Add(new GradientStop(Colors.White, 0.0));
            neuronBrush.GradientStops.Add(new GradientStop(Colors.Black, 1.0));

            #region Iterate over every layer
            for (int l = 0; l < ann.NumLayers; l++)
            {

                double neuronDist;

                if (ann.GetNumNeurons(l) > 1)
                {
                    neuronDist = (double)annBorder.ActualHeight / ((double)ann.GetNumNeurons(l)-1) - neuronDiameter;
                }
                else
                {
                    neuronDist = (double)annBorder.ActualHeight / ((double)ann.GetNumNeurons(l));
                }

                #region Iterare over every neuron on the current layer
                for (int n = 0; n < ann.GetNumNeurons(l); n++)
                {
                    Ellipse e = new Ellipse();                    
                    e.Stroke = Brushes.White;
                    e.Width = neuronDiameter;
                    e.Height = neuronDiameter;
                    e.StrokeThickness = 2.0;
                    e.Fill = Brushes.White;

                    Canvas.SetLeft(e, layerDist * l);
                    Canvas.SetTop(e, neuronDist * n);

                    annCanvas.Children.Add(e);

                    // Safe the point of the current neuron for line drawing
                    neuronPos[ann.GetNumberOfNeuronsBefore(l) + n] = new Point(layerDist * l, neuronDist * n);

                    #region Draw all connections from previous layer to current neuron.
                    for (int i = 0; l > 0 && i < ann.GetNumNeurons(l - 1); i++)
                    {
                        Line line = new Line();
                        line.X1 = neuronPos[ann.GetNumberOfNeuronsBefore(l - 1) + i].X + neuronDiameter / 2.0;
                        line.Y1 = neuronPos[ann.GetNumberOfNeuronsBefore(l - 1) + i].Y + neuronDiameter / 2.0;
                        line.X2 = neuronPos[ann.GetNumberOfNeuronsBefore(l) + n].X + neuronDiameter / 2.0;
                        line.Y2 = neuronPos[ann.GetNumberOfNeuronsBefore(l) + n].Y + neuronDiameter / 2.0;
                        line.Stroke = Brushes.White;
                        line.StrokeThickness = 1;
                        line.Opacity = 0.5;

                        annCanvas.Children.Add(line);
                    }
                    #endregion
                }
                #endregion
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
            drawANN();
        }

        /// <summary>
        /// Callback which takes the ANN DataSet for hidden layers and draws the ANN
        /// using the using the new hidden layers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applyAnnSettings_Click(object sender, RoutedEventArgs e)
        {
            ann.NumLayers = 2 + dataSetAnn.Tables[0].Rows.Count;
            ann.SetNumNeurons(0, 4);
            ann.SetNumNeurons(dataSetAnn.Tables[0].Rows.Count + 1, 1);

            for (int i = 0; i < dataSetAnn.Tables[0].Rows.Count; i++)
            {
                ann.SetNumNeurons(i + 1, Convert.ToInt32(dataSetAnn.Tables[0].Rows[i].ItemArray[0]));
            }

            annNumLayers.DataContext = ann;

            drawANN();
        }

        /// <summary>
        /// When the "Show export options is toggled from the "View" menu, we hide
        /// certain tab items that the user might not be interested in.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showExpertOptionsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if ( showExpertOptionsMenuItem.IsChecked )
            {
                annConfigurationTabItem.Visibility = Visibility.Visible;
                expressionsTabItem.Visibility = Visibility.Visible;
                trainingDatasetsTabItem.Visibility = Visibility.Visible;
                exportTabItem.Visibility = Visibility.Visible;
            }
            else
            {
                annConfigurationTabItem.Visibility = Visibility.Hidden;
                expressionsTabItem.Visibility = Visibility.Hidden;
                trainingDatasetsTabItem.Visibility = Visibility.Hidden;
                exportTabItem.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Saves the current ANN settings to an OpenCV ANN file which can be loaded
        /// later for initialization, training and use later.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAnnButton_Click(object sender, RoutedEventArgs e)
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "NeuralNetwork"; // Default file name
            dlg.DefaultExt = ".xml"; // Default file extension
            dlg.Filter = "XML documents (.xml)|*.xml"; // Filter files by extension
            dlg.Title = "Where to save your new Neural Network?";

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
            }

            // Call OpenCV wrapper from KluSharp library here.
            // ...
            
        }
    }
}
