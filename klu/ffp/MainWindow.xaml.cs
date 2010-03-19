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
using AForge.Neuro.Learning;
using AForge.Neuro;

namespace ffp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        /// <summary>
        /// This member is a wrapper object provides access to the _KLU C DLL.
        /// </summary>
        Klu _KLU;

        /// <summary>
        /// This is the timer used to query camera images.
        /// </summary>
        DispatcherTimer _CaptureTimer;

        /// <summary>
        /// This is a temporary bitmap which acts as a container for camera images.
        /// </summary>
        System.Drawing.Bitmap _TempBitmap;

        /// <summary>
        /// This stores all table adapters.
        /// </summary>
        TableAdapterManager _TAM;

        /// <summary>
        /// This is the dataset which contains the connected database in memory. 
        /// </summary>
        TrainingDataSet _DataSet;

        /// <summary>
        /// Feature points from the last processed image
        /// </summary>
        FaceFeaturePoints _FFP;

        /// <summary>
        /// Define how to process images (either still or moving images)
        /// </summary>
        ProcessOptions _ProcessOptions;

        ArrayList _ImagePathArray;
        int _CurrentImagePathIndex;

        /// <summary>
        /// Used as a dummy for thumbnail creation.
        /// </summary>
        /// <returns></returns>
        public static bool ThumbnailCallback()
        {
            return false;
        }

        /// <summary>
        /// The main entry point for this window.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            Console.WriteLine("Loc: " + System.Reflection.Assembly.GetExecutingAssembly().Location);

            try
            {
                _CurrentImagePathIndex = 0;
                _ImagePathArray = new ArrayList();
                _ProcessOptions = new ProcessOptions();
                _FFP = new FaceFeaturePoints();

                _ProcessOptions.DoEyeProcessing = 1;
                _ProcessOptions.DoMouthProcessing = 1;
                _ProcessOptions.DrawAnthropometricPoints = 0;
                _ProcessOptions.DrawSearchRectangles = 0;
                _ProcessOptions.DrawFaceRectangle = 1;
                _ProcessOptions.DrawDetectionTime = 1;
                _ProcessOptions.DrawFeaturePoints = 1;
                _ProcessOptions.DoVisualDebug = 0;

                #region Intialize encapsulated OpenCV subsystem
                _KLU = new Klu();
                _TempBitmap = new System.Drawing.Bitmap(10, 10);

                // Create a Timer with a Normal Priority
                _CaptureTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle, this.Dispatcher);
                
                // Set the callback to just show the time ticking away
                // NOTE: We are using a control so this has to run on 
                // the UI thread
                _CaptureTimer.Tick += new EventHandler(
                    delegate(object s, EventArgs a)
                    {
                        _KLU.ProcessCaptureImage(ref _ProcessOptions, ref _FFP);

                        // Ensure the image (bitmap) we are writing to has the correct dimensions
                        int width = 0, height = 0;
                        _KLU.GetLastProcessedImageDims(ref width, ref height);

                        if (_TempBitmap.Width != width || _TempBitmap.Height != height)
                        {
                            Console.WriteLine("Need to resize the _TempBitmap to " + width + "x" + height);
                            _TempBitmap.Dispose();
                            GC.Collect();
                            _TempBitmap = new System.Drawing.Bitmap(width, height);
                        }

                        _KLU.GetLastProcessedImage(ref _TempBitmap);
                        _KLU.SetWpfImageFromBitmap(ref image1, ref _TempBitmap);
                        //_KLU.SetImageBrushFromBitmap(ref imageBrush, ref _TempBitmap);
                    }
                );
                #endregion

                #region "Connect" to database
                _TAM = new TableAdapterManager();  
                _DataSet = new TrainingDataSet();

                // Load data from SQL database and fill our DataSet
                _TAM.ExpressionTableAdapter = new ExpressionTableAdapter();
                _TAM.TrainingTableAdapter = new TrainingTableAdapter();

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
            _DataSet.Clear();

            // Load data from database and fill dataset
            _TAM.ExpressionTableAdapter.Fill(_DataSet.Expression);
            _TAM.TrainingTableAdapter.Fill(_DataSet.Training);

            // Bind data to controls 
            expressionSelectorComboBox.DataContext = _DataSet.Expression;
        }

        /// <summary>
        /// Starts the processing of video images.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void processVideo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void processNextFile()
        {
            _CurrentImagePathIndex++;

            // Start infront if we reach the end
            if (_CurrentImagePathIndex >= _ImagePathArray.Count)
            {
                _CurrentImagePathIndex = 0;
            }            

            if (_CurrentImagePathIndex < _ImagePathArray.Count)
            {
                
                _KLU.ProcessStillImage((string)_ImagePathArray[_CurrentImagePathIndex], ref _ProcessOptions, ref _FFP);

                int width = 0, height = 0;
                _KLU.GetLastProcessedImageDims(ref width, ref height);

                if (_TempBitmap.Width != width || _TempBitmap.Height != height)
                {
                    Console.WriteLine("Need to resize the tmpBitmap to " + width + "x" + height);
                    _TempBitmap.Dispose();
                    GC.Collect();
                    _TempBitmap = new System.Drawing.Bitmap(width, height);
                }

                _KLU.GetLastProcessedImage(ref _TempBitmap);
                //_KLU.SetImageBrushFromBitmap(ref imageBrush, ref _TempBitmap);
                _KLU.SetWpfImageFromBitmap(ref image1, ref _TempBitmap);
            }
        }

        private void processPreviousFile()
        {
            _CurrentImagePathIndex--;

            // Start infront if we reach the end
            if (_CurrentImagePathIndex < 0)
            {
                _CurrentImagePathIndex = _ImagePathArray.Count - 1;
            }

            if (_CurrentImagePathIndex >= 0 && _CurrentImagePathIndex < _ImagePathArray.Count)
            {
                _KLU.ProcessStillImage((string)_ImagePathArray[_CurrentImagePathIndex], ref _ProcessOptions, ref _FFP);

                int width = 0, height = 0;
                _KLU.GetLastProcessedImageDims(ref width, ref height);

                if (_TempBitmap.Width != width || _TempBitmap.Height != height)
                {
                    Console.WriteLine("Need to resize the tmpBitmap to " + width + "x" + height);
                    _TempBitmap.Dispose();
                    GC.Collect();
                    _TempBitmap = new System.Drawing.Bitmap(width, height);
                }                
                
                _KLU.GetLastProcessedImage(ref _TempBitmap);
                //_KLU.SetImageBrushFromBitmap(ref imageBrush, ref _TempBitmap);
                _KLU.SetWpfImageFromBitmap(ref image1, ref _TempBitmap);
            }
        }

        private void processFirstButton_Click(object sender, RoutedEventArgs e)
        {
            _CurrentImagePathIndex = -1;
            processNextFile();
        }

        private void processLastButton_Click(object sender, RoutedEventArgs e)
        {
            _CurrentImagePathIndex = _ImagePathArray.Count;
            processPreviousFile();
        }

        /// <summary>
        /// Starts the processing of still images.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureStart_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Configure save file dialog box            
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();            
            dlg.DefaultExt = "."; // Default file extension
            dlg.Filter = "Imagefiles (*.bmp, *.jpg, *.png, *.tif, *.tiff, *.tga, *.pgm)|*.bmp;*.jpg;*.png;*.tif;*.tiff;*.tga;*.pgm"; // Filter files by extension
            dlg.Title = "Load images";
            dlg.Multiselect = true;

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {                
                //SidebarImage.Source = BitmapFrame.Create(new Uri(dlg.FileNames[0]));                

                stopAllProcessing();

                processFirstButton.IsEnabled = true;
                processLastButton.IsEnabled = true;
                processPreviousButton.IsEnabled = true;
                processNextButton.IsEnabled = true;
                processPlayPauseButton.IsEnabled = false;
                ClassifyResultButton.IsEnabled = true;
                expressionSelectorComboBox.IsEnabled= true;
                StopCameraMenuItem.IsEnabled = false;
                CameraParametersMenuItem.IsEnabled = false;

                _ImagePathArray.Clear();

                _ImagePathArray.AddRange(dlg.FileNames);

                _CurrentImagePathIndex = -1;
                processNextFile();
            }    
        }

        /// <summary>
        /// Starts the processing of camera images.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraStart_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            processFirstButton.IsEnabled = false;
            processLastButton.IsEnabled = false;
            processPreviousButton.IsEnabled = false;
            processNextButton.IsEnabled = false;
            processPlayPauseButton.IsEnabled = true;
            ClassifyResultButton.IsEnabled = false;
            expressionSelectorComboBox.IsEnabled = false;
            StopCameraMenuItem.IsEnabled = true;
            CameraParametersMenuItem.IsEnabled = true;

            _ImagePathArray.Clear();

            if (!_KLU.CreateCapture())
            {
                return;
            }

            // Set the Interval to what is typed into the corresponding text box, but don't allow values below 100ms.
            _CaptureTimer.Interval = TimeSpan.FromMilliseconds(Convert.ToInt32(30));//captureTimeTextBox.Text));

            // Start the timer
            _CaptureTimer.Start();

            statusText.Text = "Started camera processing";
        }

        /// <summary>
        /// Stops all camera or video processing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraStop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            stopAllProcessing();
        }

        private void stopAllProcessing()
        {
            // Stops the internal timer which is responsible for updating the (live) image
            // and doing the processing.
            _CaptureTimer.Stop();

            processFirstButton.IsEnabled = false;
            processLastButton.IsEnabled = false;
            processPreviousButton.IsEnabled = false;
            processNextButton.IsEnabled = false;
            processPlayPauseButton.IsEnabled = false;
            ClassifyResultButton.IsEnabled = false;
            expressionSelectorComboBox.IsEnabled = false;
            StopCameraMenuItem.IsEnabled = false;
            CameraParametersMenuItem.IsEnabled = false;

            _KLU.FreeCapture();

            statusText.Text = "All processing stopped";
        }

        /// <summary>
        /// Call this function to ensure the user has the ability to save
        /// unsaved changes to the dataset. It will open a message box and
        /// asks if the user wants to save those changes.
        /// NOTE: Currently this function simple saves all the updates made!
        /// </summary>
        private void LastChanceSaving()
        {            
            if (_DataSet.HasChanges())
            {
                MessageBoxResult res;
                //res = MessageBox.Show("You've made some changes that are not yet saved! Do you wish to save them?",
                //    "Prevent data loss?",
                //    MessageBoxButton.YesNoCancel,
                //    MessageBoxImage.Question,
                //    MessageBoxResult.Yes
                //);

                //if (res == MessageBoxResult.Yes)
                //{
                    try
                    {
                        _TAM.UpdateAll(_DataSet);
                        _DataSet.AcceptChanges();
                    }
                    catch (Exception e)
                    {
                        res = MessageBox.Show(e.Message, "An unhandled exception occured!", MessageBoxButton.OK, MessageBoxImage.Error);                        
                    }
                //}
            }
        }

        /// <summary>
        /// Callback which is called when the window is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LastChanceSaving();
        }

        /// <summary>
        /// Callback which is called when the application closes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseApplication_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LastChanceSaving();
            Close();
        }

        /// <summary>
        /// Callback which stops the training process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrainingStop_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        /// <summary>
        /// Callback which starts the training process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrainingStart_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            statusText.Text = "Now training...";

            // Enable infinite progess indicator
            statusProgess.IsEnabled = true;
            statusProgess.Visibility = Visibility.Visible;

            #region Prepare data to be trained. Involves copying.

            int numTrainingSets = _DataSet.Training.Rows.Count;
            const int numInputNeurons = 16;
            const int numOutputNeurons = 1;
            double[][] inputs = new double[numTrainingSets][];
            double[][] outputs = new double[numTrainingSets][];

            for (int i = 0; i < numTrainingSets; i++)
            {
                inputs[i] = new double[numInputNeurons];
                inputs[i][0] = _DataSet.Training[i].LipCornerLeftX;
                inputs[i][1] = _DataSet.Training[i].LipCornerLeftY;
                inputs[i][2] = _DataSet.Training[i].LipCornerRightX;
                inputs[i][3] = _DataSet.Training[i].LipCornerRightY;
                inputs[i][4] = _DataSet.Training[i].LipUpLeftX;
                inputs[i][5] = _DataSet.Training[i].LipUpLeftY;
                inputs[i][6] = _DataSet.Training[i].LipUpCenterX;
                inputs[i][7] = _DataSet.Training[i].LipUpCenterY;
                inputs[i][8] = _DataSet.Training[i].LipUpRightX;
                inputs[i][9] = _DataSet.Training[i].LipUpRightY;
                inputs[i][10] = _DataSet.Training[i].LipBottomLeftX;
                inputs[i][11] = _DataSet.Training[i].LipBottomLeftY;
                inputs[i][12] = _DataSet.Training[i].LipBottomCenterX;
                inputs[i][13] = _DataSet.Training[i].LipBottomCenterY;
                inputs[i][14] = _DataSet.Training[i].LipBottomRightX;
                inputs[i][15] = _DataSet.Training[i].LipBottomRightY;

                outputs[i] = new double[numOutputNeurons];
                outputs[i][0] = _DataSet.Training[i].ExpressionOID;
            }
            #endregion

            ActivationNetwork network = new ActivationNetwork(
                new SigmoidFunction(1.0),
                numInputNeurons,
                6,
                5,
                numOutputNeurons
            );

            BackPropagationLearning teacher = new BackPropagationLearning(network);
            teacher.LearningRate = 0.1;

            for (int i = 0; i < 1000; i++)
            {
                double error = teacher.RunEpoch(inputs, outputs);
                if (i == 0)
                {
                    Console.WriteLine("Error at i=" + i + ": " + error);
                }
                if (i == 999)
                {
                    Console.WriteLine("Error at i=" + i + ": " + error);
                }
            }

            // Disable infinite progess indicator
            statusProgess.IsEnabled = false;
            statusProgess.Visibility = Visibility.Hidden;

                
            //Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            //dlg.DefaultExt = "."; // Default file extension
            //dlg.Filter = "Neural network (*.xml)|*.xml|All files (*.*)|*.*"; // Filter files by extension
            //dlg.Title = "Select the neural network you want to train?";
            //dlg.Multiselect = false;

            //// Show save file dialog box
            //Nullable<bool> result = dlg.ShowDialog();

            //if (result == true)
            //{
            //    bool res = _KLU.LoadANN(dlg.FileName);

            //    Console.WriteLine("Load ANN success? " + res );

            //    if ( !res )
            //    {
            //        return;
            //    }

            //    statusText.Text = "ANN loaded: " + dlg.FileName;

            //    TerminationCriteria terminationCriteria = new TerminationCriteria();
            //    terminationCriteria.TerminationType = Convert.ToInt32(TrainingTermination.MaxIterationTermination);
            //    terminationCriteria.MaxIteration = 2000; // TODO: (Ko) after checking the saved ANN file I think, there is an error with this number

            //    TrainOptions options = new TrainOptions();
            //    options.Algorithm = TrainingAlgorithm.BackpropAlgorithm;
            //    options.Termination = terminationCriteria;

            //    statusText.Text = "Now training...";

            //    // Enable infinite progess indicator
            //    statusProgess.IsEnabled = true;
            //    statusProgess.Visibility = Visibility.Visible;

            //    #region Prepare data to be trained. Involves copying.

            //    int numTrainingSets = _DataSet.Training.Rows.Count;
            //    int numInputNeurons = 16;
            //    int numOutputNeurons = 1;
            //    float[] inputs = new float[numTrainingSets * numInputNeurons];
            //    float[] outputs = new float[numTrainingSets * numOutputNeurons];

            //    for (int i = 0; i < numTrainingSets; i++)
            //    {
            //        inputs[i * numInputNeurons + 0] = _DataSet.Training[i].LipCornerLeftX;
            //        inputs[i * numInputNeurons + 1] = _DataSet.Training[i].LipCornerLeftY;
            //        inputs[i * numInputNeurons + 2] = _DataSet.Training[i].LipCornerRightX;
            //        inputs[i * numInputNeurons + 3] = _DataSet.Training[i].LipCornerRightY;
            //        inputs[i * numInputNeurons + 4] = _DataSet.Training[i].LipUpLeftX;
            //        inputs[i * numInputNeurons + 5] = _DataSet.Training[i].LipUpLeftY;
            //        inputs[i * numInputNeurons + 6] = _DataSet.Training[i].LipUpCenterX;
            //        inputs[i * numInputNeurons + 7] = _DataSet.Training[i].LipUpCenterY;
            //        inputs[i * numInputNeurons + 8] = _DataSet.Training[i].LipUpRightX;
            //        inputs[i * numInputNeurons + 9] = _DataSet.Training[i].LipUpRightY;
            //        inputs[i * numInputNeurons + 10] = _DataSet.Training[i].LipBottomLeftX;
            //        inputs[i * numInputNeurons + 11] = _DataSet.Training[i].LipBottomLeftY;
            //        inputs[i * numInputNeurons + 12] = _DataSet.Training[i].LipBottomCenterX;
            //        inputs[i * numInputNeurons + 13] = _DataSet.Training[i].LipBottomCenterY;
            //        inputs[i * numInputNeurons + 14] = _DataSet.Training[i].LipBottomRightX;
            //        inputs[i * numInputNeurons + 15] = _DataSet.Training[i].LipBottomRightY;

            //        outputs[i] = _DataSet.Training[i].ExpressionOID;
            //    }
            //    #endregion

            //    _KLU.SaveANN(dlg.FileName + ".untrained.xml");

            //    int iters = -10;
            //    res = _KLU.TrainAnn(options, numTrainingSets, inputs, outputs, ref iters);

            //    Console.WriteLine("Training success? " + res);

            //    // Disable infinite progess indicator
            //    statusProgess.IsEnabled = false;
            //    statusProgess.Visibility = Visibility.Hidden;

            //    statusText.Text = "Training result: " + res;

            //    _KLU.SaveANN(dlg.FileName + ".trained.xml");

            //    // Let's see if the ANN is trained. (simple for now)
            //    // TODO: (Ko) Do 7 out of 10 validation etc.

            //    // Test the first dataset and compare the predicted output with the expected
            //    float[] results = new float[1];
            //    _KLU.PredictANN(inputs, 16, results, 1);

            //    Console.WriteLine("Expected output: " + _DataSet.Training[0].ExpressionOID + " Predicted output: " + results[0]);
            //}
        }

        /// <summary>
        /// Handles all processing options
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OptionToggle_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.RoutedEventArgs e1 = e as System.Windows.RoutedEventArgs;

            if (e1 == null)
            {
                return;
            }

            // TODO: (Ko) Be aware that the RibbonCommand is not always invoked by a RibbonButton!

            RibbonToggleButton b = (RibbonToggleButton)e1.OriginalSource;

            if (b == null)
            {
                return;
            }

            switch (b.Name)
            {
                case "drawAnthropometricPointsMenuItem":
                    _ProcessOptions.DrawAnthropometricPoints = Convert.ToInt32(b.IsChecked);
                    break;
                case "drawSearchRectanglesMenuItem":
                    _ProcessOptions.DrawSearchRectangles = Convert.ToInt32(b.IsChecked);
                    break;
                case "drawFaceRectangleMenuItem":
                    _ProcessOptions.DrawFaceRectangle = Convert.ToInt32(b.IsChecked);
                    break;
                case "drawDetectionTimeMenuItem":
                    _ProcessOptions.DrawDetectionTime = Convert.ToInt32(b.IsChecked);
                    break;
                case "drawFeaturePointsMenuItem":
                    _ProcessOptions.DrawFeaturePoints = Convert.ToInt32(b.IsChecked);
                    break;
                case "doEyeProcessingMenuItem":
                    _ProcessOptions.DoEyeProcessing = Convert.ToInt32(b.IsChecked);
                    break;
                case "doMouthProcessingMenuItem":
                    _ProcessOptions.DoMouthProcessing = Convert.ToInt32(b.IsChecked);
                    break;
                case "doVisualDebugMenuItem":
                    _ProcessOptions.DoVisualDebug = Convert.ToInt32(b.IsChecked);
                    break;
            };

            if (_ImagePathArray.Count > 0)
            {
                _CurrentImagePathIndex--;
                processNextFile();
            }
        }

        

        private void processNextButton_Click(object sender, RoutedEventArgs e)
        {
            processNextFile();
        }

        private void processPreviousButton_Click(object sender, RoutedEventArgs e)
        {
            processPreviousFile();
        }

        private void processPlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_CaptureTimer.IsEnabled)
            {
                _CaptureTimer.Stop();
                ClassifyResultButton.IsEnabled = true;
                expressionSelectorComboBox.IsEnabled = true;
            }
            else
            {
                _CaptureTimer.Start();
                ClassifyResultButton.IsEnabled = false;
                expressionSelectorComboBox.IsEnabled = false;
            }
        }

        private void expressionSelectorComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            ComboBox cbox = sender as ComboBox;
            if (e.Key == Key.Enter)
            {
                try
                {
                    _DataSet.Expression.AddExpressionRow(cbox.Text, null);
                }
                catch (Exception ex)
                {
                    cbox.BorderBrush = Brushes.Red;
                    cbox.BorderThickness = new Thickness(2.0);
                    cbox.ToolTip = ex.Message;
                }                
            }
        }

        /// <summary>
        /// Calculates from fixed (integer) to relative coordinates
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="begin"></param>
        /// <param name="dim"></param>
        /// <returns>(double) (coord - begin) / (double) dim</returns>
        private static float i2r(int coord, int begin, int dim)
        {
            float res = (float)(coord - begin) / (float)dim;

            // This is a quick hack to set invalid FFPs to 0
            return (res < 0 || res > 1) ? 0 : res;
        }

        public static void AddTrainingData(ref TrainingDataSet dataSet, ref FaceFeaturePoints ffp, int expressionOID, ref System.Drawing.Bitmap picture)
        {
            // Convert facial coordinates to relative face rectangle coordinates.
            // We do this because we are not interested in absolute coordinates but in quality of the face.

            int x = ffp.FaceRectangle.X;
            int y = ffp.FaceRectangle.Y;
            int w = ffp.FaceRectangle.Width;
            int h = ffp.FaceRectangle.Height;

            // Calculate the eye distance (in relative coordinates
            float dx = i2r(ffp.RightEye.EyeCenter.X, x, w) - i2r(ffp.LeftEye.EyeCenter.X, x, w);
            float dy = i2r(ffp.RightEye.EyeCenter.Y, y, h) - i2r(ffp.LeftEye.EyeCenter.Y, y, h);
            //float dx = i2r(ffp.RightEye.EyeCenter.X - ffp.LeftEye.EyeCenter.X, x, w);
            //float dy = i2r(ffp.RightEye.EyeCenter.Y - ffp.LeftEye.EyeCenter.Y, y, h);
            float eyeDist = (float)Math.Sqrt(dx * dx + dy * dy);

            // Construct thumbnail
            const int thumbnailWidth = 50;
            const int thumbnailHeight = 50;

            System.Drawing.Rectangle faceRect = new System.Drawing.Rectangle(x, y, w, h);
            System.Drawing.Bitmap faceImg = picture.Clone(faceRect, picture.PixelFormat);

            System.Drawing.Image.GetThumbnailImageAbort tc = new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback);
            System.Drawing.Image thumbnail = faceImg.GetThumbnailImage(thumbnailWidth, thumbnailHeight, tc, IntPtr.Zero);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            thumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            TrainingDataSet.TrainingRow row = dataSet.Training.NewTrainingRow();

            row.ExpressionOID = expressionOID;
            row.LipCornerLeftX = i2r(ffp.Mouth.LipCornerLeft.X, x, w);
            row.LipCornerLeftY = i2r(ffp.Mouth.LipCornerLeft.Y, y, h);
            row.LipCornerRightX = i2r(ffp.Mouth.LipCornerRight.X, x, w);
            row.LipCornerRightY = i2r(ffp.Mouth.LipCornerRight.Y, y, h);
            row.LipUpLeftX = i2r(ffp.Mouth.LipUpLeft.X, x, w);
            row.LipUpLeftY = i2r(ffp.Mouth.LipUpLeft.Y, y, h);
            row.LipUpCenterX = i2r(ffp.Mouth.LipUpCenter.X, x, w);
            row.LipUpCenterY = i2r(ffp.Mouth.LipUpCenter.Y, y, h);
            row.LipUpRightX = i2r(ffp.Mouth.LipUpRight.X, x, w);
            row.LipUpRightY = i2r(ffp.Mouth.LipUpRight.Y, y, h);
            row.LipUpRightX = i2r(ffp.Mouth.LipBottomLeft.X, x, w);
            row.LipUpRightY = i2r(ffp.Mouth.LipBottomLeft.Y, y, h);
            row.LipBottomCenterX = i2r(ffp.Mouth.LipBottomCenter.X, x, w);
            row.LipBottomCenterY = i2r(ffp.Mouth.LipBottomCenter.Y, y, h);
            row.LipBottomRightX = i2r(ffp.Mouth.LipBottomRight.X, x, w);
            row.LipBottomRightY = i2r(ffp.Mouth.LipBottomRight.Y, y, h);
            row.LipBottomLeftX = i2r(ffp.Mouth.LipBottomLeft.X, x, w);
            row.LipBottomLeftY = i2r(ffp.Mouth.LipBottomLeft.Y, y, h);
            row.EyeDistance = eyeDist;
            row.LeftEyeCenterX = i2r(ffp.LeftEye.EyeCenter.X, x, w);
            row.LeftEyeCenterY = i2r(ffp.LeftEye.EyeCenter.Y, y, h);
            row.LeftLidBottomX = i2r(ffp.LeftEye.LidBottomCenter.X, x, w);
            row.LeftLidBottomY = i2r(ffp.LeftEye.LidBottomCenter.Y, y, h);
            row.LeftLidCornerLeftX = i2r(ffp.LeftEye.LidCornerLeft.X, x, w);
            row.LeftLidCornerLeftY = i2r(ffp.LeftEye.LidCornerLeft.Y, y, h);
            row.LeftLidCornerRightX = i2r(ffp.LeftEye.LidCornerRight.X, x, w);
            row.LeftLidCornerRightY = i2r(ffp.LeftEye.LidCornerRight.Y, y, h);
            row.LeftLidUpX = i2r(ffp.LeftEye.LidUpCenter.X, x, w);
            row.LeftLidUpY = i2r(ffp.LeftEye.LidUpCenter.Y, y, h);
            row.RightEyeCenterX = i2r(ffp.RightEye.EyeCenter.X, x, w);
            row.RightEyeCenterY = i2r(ffp.RightEye.EyeCenter.Y, y, h);
            row.RightLidBottomX = i2r(ffp.RightEye.LidBottomCenter.X, x, w);
            row.RightLidBottomY = i2r(ffp.RightEye.LidBottomCenter.Y, y, h);
            row.RightLidCornerLeftX = i2r(ffp.RightEye.LidCornerLeft.X, x, w);
            row.RightLidCornerLeftY = i2r(ffp.RightEye.LidCornerLeft.Y, y, h);
            row.RightLidCornerRightX = i2r(ffp.RightEye.LidCornerRight.X, x, w);
            row.RightLidCornerRightY = i2r(ffp.RightEye.LidCornerRight.Y, y, h);
            row.RightLidUpX = i2r(ffp.RightEye.LidUpCenter.X, x, w);
            row.RightLidUpY = i2r(ffp.RightEye.LidUpCenter.Y, y, h);
            row.Thumbnail = ms.ToArray();

            dataSet.Training.AddTrainingRow(row);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int expressionOID = Convert.ToInt32(expressionSelectorComboBox.SelectedValue);

            AddTrainingData(ref _DataSet, ref _FFP, expressionOID, ref _TempBitmap);           
        }

        /// <summary>
        /// Pops up a dialog to configure the capture device's parameters.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraParameters_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _KLU.ConfigureCaptureDialog();
        }

        /// <summary>
        /// Pops up a dialog to configure the capture device's resolution.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void captureResolutionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _KLU.ConfigureCaptureResolutionDialog();
        }

        private void About_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AboutDialog dlg = new AboutDialog();
            dlg.Owner = this;
            dlg.ShowDialog();
        }

        private void StretchNone_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            image1.Stretch = Stretch.None;
        }

        private void StretchUniform_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            image1.Stretch = Stretch.Uniform;
        }

        private void ExpressionsDialog_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ExpressionsDialog dlg = new ExpressionsDialog(ref _DataSet);
            dlg.Owner = this;
            dlg.ShowDialog();
        }

        private void TrainingDataSetsDialog_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TrainingDataSetsDialog dlg = new TrainingDataSetsDialog(ref _DataSet);
            dlg.Owner = this;
            dlg.ShowDialog();
        }

        private void AnnDialog_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AnnDialog dlg = new AnnDialog(ref _KLU);
            dlg.Owner = this;
            dlg.ShowDialog();
        }

        private void ExportToExcel_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void ExportToCSV_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void ExportToXML_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = "."; // Default file extension
            dlg.Filter = "Training (*.xml)|*.xml|All files (*.*)|*.*"; // Filter files by extension
            dlg.Title = "Where to save your training data?";

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                _DataSet.Training.WriteXml(dlg.FileName);
            }
        }

        private void ImportFromXML_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = "."; // Default file extension
            dlg.Filter = "Training (*.xml)|*.xml|All files (*.*)|*.*"; // Filter files by extension
            dlg.Title = "Pick a XML file from which to import datasets!";
            dlg.Multiselect = false;

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                try
                {
                    _DataSet.Training.ReadXml(dlg.FileName);
                }
                catch (Exception exception)
                {
                    MessageBox.Show("There was an error while importing your data. This can have multiple reasons."
                        + " The most comment reason ist that the imported datasets are already loaded. If that is the case you can simply ignore this error."
                        + " Here are the exception details: \n\n\"" + exception.Message + "\"", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BatchClassification_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            BatchClassificationDialog dlg = new BatchClassificationDialog(ref _KLU, ref _DataSet, _ProcessOptions);
            dlg.Owner = this;
            dlg.ShowDialog();
        }
    }
}
