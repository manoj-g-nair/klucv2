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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        /// <summary>
        /// This member is a wrapper object provides access to the klu C DLL.
        /// </summary>
        Klu klu;

        /// <summary>
        /// This is the timer used to query camera images.
        /// </summary>
        DispatcherTimer captureTimer;

        /// <summary>
        /// This is a temporary bitmap which acts as a container for camera images.
        /// </summary>
        System.Drawing.Bitmap tmpBitmap;

        /// <summary>
        /// This stores all table adapters.
        /// </summary>
        TableAdapterManager tam;

        /// <summary>
        /// This is the dataset which contains the connected database in memory. 
        /// </summary>
        TrainingDataSet dataSet;

        /// <summary>
        /// Feature points from the last processed image
        /// </summary>
        FaceFeaturePoints ffp;

        /// <summary>
        /// Define how to process images (either still or moving images)
        /// </summary>
        ProcessOptions processOptions;

        System.Collections.ArrayList filesToProcess;
        int fileProcessIdx;

        /// <summary>
        /// Used as a dummy for thumbnail creation.
        /// </summary>
        /// <returns></returns>
        public bool ThumbnailCallback()
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
                fileProcessIdx = 0;
                filesToProcess = new ArrayList();
                processOptions = new ProcessOptions();
                ffp = new FaceFeaturePoints();

                processOptions.DoEyeProcessing = 0;
                processOptions.DoMouthProcessing = 1;
                processOptions.DrawAnthropometricPoints = 0;
                processOptions.DrawSearchRectangles = 0;
                processOptions.DrawFaceRectangle = 1;
                processOptions.DrawDetectionTime = 1;
                processOptions.DrawFeaturePoints = 1;
                processOptions.DoVisualDebug = 0;



                #region Intialize encapsulated OpenCV subsystem
                klu = new Klu();
                tmpBitmap = new System.Drawing.Bitmap(10, 10);

                // Create a Timer with a Normal Priority
                captureTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle, this.Dispatcher);

                // Set the callback to just show the time ticking away
                // NOTE: We are using a control so this has to run on 
                // the UI thread
                captureTimer.Tick += new EventHandler(
                    delegate(object s, EventArgs a)
                    {
                        klu.ProcessCaptureImage(ref processOptions, ref ffp);

                        // Ensure the image (bitmap) we are writing to has the correct dimensions
                        int width = 0, height = 0;
                        klu.GetLastProcessedImageDims(ref width, ref height);

                        if (tmpBitmap.Width != width || tmpBitmap.Height != height)
                        {
                            Console.WriteLine("Need to resize the tmpBitmap to " + width + "x" + height);
                            tmpBitmap.Dispose();
                            GC.Collect();
                            tmpBitmap = new System.Drawing.Bitmap(width, height);
                        }

                        klu.GetLastProcessedImage(ref tmpBitmap);
                        klu.SetWpfImageFromBitmap(ref image1, ref tmpBitmap);
                        //klu.SetImageBrushFromBitmap(ref imageBrush, ref tmpBitmap);
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
            expressionSelectorComboBox.DataContext = dataSet.Expression;
        }

        ///// <summary>
        ///// Callback which sets the capture time interval in ms.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void captureTimeTextBox_TextInput(object sender, TextCompositionEventArgs e)
        //{
        //    int value = Math.Min(1, Convert.ToInt32(captureTimeTextBox.Text));
        //    captureTimeTextBox.Text = Convert.ToString(value);

        //    // Also adjust the timer.
        //    captureTimer.Interval = TimeSpan.FromMilliseconds(value);            
        //}

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
            fileProcessIdx++;

            // Start infront if we reach the end
            if (fileProcessIdx >= filesToProcess.Count)
            {
                fileProcessIdx = 0;
            }            

            if (fileProcessIdx < filesToProcess.Count)
            {
                klu.ProcessStillImage((string)filesToProcess[fileProcessIdx], ref processOptions, ref ffp);

                int width = 0, height = 0;
                klu.GetLastProcessedImageDims(ref width, ref height);

                if (tmpBitmap.Width != width || tmpBitmap.Height != height)
                {
                    Console.WriteLine("Need to resize the tmpBitmap to " + width + "x" + height);
                    tmpBitmap.Dispose();
                    GC.Collect();
                    tmpBitmap = new System.Drawing.Bitmap(width, height);
                }

                klu.GetLastProcessedImage(ref tmpBitmap);
                //klu.SetImageBrushFromBitmap(ref imageBrush, ref tmpBitmap);
                klu.SetWpfImageFromBitmap(ref image1, ref tmpBitmap);
            }
        }

        private void processPreviousFile()
        {
            fileProcessIdx--;

            // Start infront if we reach the end
            if (fileProcessIdx < 0)
            {
                fileProcessIdx = filesToProcess.Count - 1;
            }

            if (fileProcessIdx >= 0 && fileProcessIdx < filesToProcess.Count)
            {
                klu.ProcessStillImage((string)filesToProcess[fileProcessIdx], ref processOptions, ref ffp);

                int width = 0, height = 0;
                klu.GetLastProcessedImageDims(ref width, ref height);

                if (tmpBitmap.Width != width || tmpBitmap.Height != height)
                {
                    Console.WriteLine("Need to resize the tmpBitmap to " + width + "x" + height);
                    tmpBitmap.Dispose();
                    GC.Collect();
                    tmpBitmap = new System.Drawing.Bitmap(width, height);
                }                
                
                klu.GetLastProcessedImage(ref tmpBitmap);
                //klu.SetImageBrushFromBitmap(ref imageBrush, ref tmpBitmap);
                klu.SetWpfImageFromBitmap(ref image1, ref tmpBitmap);
            }
        }

        private void processFirstButton_Click(object sender, RoutedEventArgs e)
        {
            fileProcessIdx = -1;
            processNextFile();
        }

        private void processLastButton_Click(object sender, RoutedEventArgs e)
        {
            fileProcessIdx = filesToProcess.Count - 1;
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
            dlg.Filter = "Imagefiles (*.bmp, *.jpg, *.png, *.tif, *.tga)|*.bmp;*.jpg;*.png;*.tif;*.tga|All files (*.*)|*.*"; // Filter files by extension
            dlg.Title = "Load images";
            dlg.Multiselect = true;

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                stopAllProcessing();

                processPreviousButton.IsEnabled = true;
                processNextButton.IsEnabled = true;
                processPlayPauseButton.IsEnabled = false;

                filesToProcess.Clear();

                filesToProcess.AddRange(dlg.FileNames);

                fileProcessIdx = -1;
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
            processPreviousButton.IsEnabled = false;
            processNextButton.IsEnabled = false;
            processPlayPauseButton.IsEnabled = true;

            filesToProcess.Clear();

            if (!klu.CreateCapture())
            {
                return;
            }

            // Set the Interval to what is typed into the corresponding text box, but don't allow values below 100ms.
            captureTimer.Interval = TimeSpan.FromMilliseconds(Convert.ToInt32(50));//captureTimeTextBox.Text));

            // Start the timer
            captureTimer.Start();

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
            captureTimer.Stop();

            klu.FreeCapture();

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
            if (dataSet.HasChanges())
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
                        tam.UpdateAll(dataSet);
                        dataSet.AcceptChanges();
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
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = "."; // Default file extension
            dlg.Filter = "Neural network (*.xml)|*.xml|All files (*.*)|*.*"; // Filter files by extension
            dlg.Title = "Select the neural network you want to train?";
            dlg.Multiselect = false;

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                bool res = klu.LoadANN(dlg.FileName);

                Console.WriteLine("Load ANN success? " + res );

                if ( !res )
                {
                    return;
                }

                statusText.Text = "ANN loaded: " + dlg.FileName;

                TerminationCriteria terminationCriteria = new TerminationCriteria();
                terminationCriteria.TerminationType = Convert.ToInt32(TrainingTermination.MaxIterationTermination);
                terminationCriteria.MaxIteration = 2000; // TODO: (Ko) after checking the saved ANN file I think, there is an error with this number

                TrainOptions options = new TrainOptions();
                options.Algorithm = TrainingAlgorithm.BackpropAlgorithm;
                options.Termination = terminationCriteria;

                statusText.Text = "Now training...";

                // Enable infinite progess indicator
                statusProgess.IsEnabled = true;
                statusProgess.Visibility = Visibility.Visible;

                #region Prepare data to be trained. Involves copying.

                int numTrainingSets = dataSet.Training.Rows.Count;
                int numInputNeurons = 16;
                int numOutputNeurons = 1;
                float[] inputs = new float[numTrainingSets * numInputNeurons];
                float[] outputs = new float[numTrainingSets * numOutputNeurons];

                for (int i = 0; i < numTrainingSets; i++)
                {
                    inputs[i + 0] = dataSet.Training[i].LipCornerLeftX;
                    inputs[i + 1] = dataSet.Training[i].LipCornerLeftY;
                    inputs[i + 2] = dataSet.Training[i].LipCornerRightX;
                    inputs[i + 3] = dataSet.Training[i].LipCornerRightY;
                    inputs[i + 4] = dataSet.Training[i].LipUpLeftX;
                    inputs[i + 5] = dataSet.Training[i].LipUpLeftY;
                    inputs[i + 6] = dataSet.Training[i].LipUpCenterX;
                    inputs[i + 7] = dataSet.Training[i].LipUpCenterY;
                    inputs[i + 8] = dataSet.Training[i].LipUpRightX;
                    inputs[i + 9] = dataSet.Training[i].LipUpRightY;
                    inputs[i + 10] = dataSet.Training[i].LipBottomLeftX;
                    inputs[i + 11] = dataSet.Training[i].LipBottomLeftY;
                    inputs[i + 12] = dataSet.Training[i].LipBottomCenterX;
                    inputs[i + 13] = dataSet.Training[i].LipBottomCenterY;
                    inputs[i + 14] = dataSet.Training[i].LipBottomRightX;
                    inputs[i + 15] = dataSet.Training[i].LipBottomRightY;

                    outputs[i] = dataSet.Training[i].ExpressionOID;
                }
                #endregion

                klu.SaveANN(dlg.FileName + ".untrained.xml");

                int iters = -10;
                res = klu.TrainAnn(options, numTrainingSets, inputs, outputs, ref iters);

                Console.WriteLine("Training success? " + res);

                // Disable infinite progess indicator
                statusProgess.IsEnabled = false;
                statusProgess.Visibility = Visibility.Hidden;

                statusText.Text = "Training result: " + res;

                klu.SaveANN(dlg.FileName + ".trained.xml");

                // Let's see if the ANN is trained. (simple for now)
                // TODO: (Ko) Do 7 out of 10 validation etc.

                // Test the first dataset and compare the predicted output with the expected
                float[] results = new float[1];
                klu.PredictANN(inputs, 16, results, 1);

                Console.WriteLine("Expected output: " + dataSet.Training[0].ExpressionOID + " Predicted output: " + results[0]);
            }
        }

        /// <summary>
        /// Handles all processing options
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OptionToggle_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RibbonToggleButton s = sender as RibbonToggleButton;

            switch (s.Name)
            {
                //case "showExpertOptionsMenuItem":
                //    if (menuItem.IsChecked)
                //    {
                //        annConfigurationTabItem.Visibility = Visibility.Visible;
                //        expressionsTabItem.Visibility = Visibility.Visible;
                //        trainingDatasetsTabItem.Visibility = Visibility.Visible;
                //        // TODO: (Ko) Re-enable the export TabItem when functionality is implemented
                //        //exportTabItem.Visibility = Visibility.Visible;
                //    }
                //    else
                //    {
                //        annConfigurationTabItem.Visibility = Visibility.Hidden;
                //        expressionsTabItem.Visibility = Visibility.Hidden;
                //        trainingDatasetsTabItem.Visibility = Visibility.Hidden;
                //        // TODO: (Ko) Re-enable the export TabItem when functionality is implemented
                //        //exportTabItem.Visibility = Visibility.Hidden;
                //    }
                //    break;
                case "drawAnthropometricPointsMenuItem":
                    processOptions.DrawAnthropometricPoints = Convert.ToInt32(s.IsChecked);
                    break;
                case "drawSearchRectanglesMenuItem":
                    processOptions.DrawSearchRectangles = Convert.ToInt32(s.IsChecked);
                    break;
                case "drawFaceRectangleMenuItem":
                    processOptions.DrawFaceRectangle = Convert.ToInt32(s.IsChecked);
                    break;
                case "drawDetectionTimeMenuItem":
                    processOptions.DrawDetectionTime = Convert.ToInt32(s.IsChecked);
                    break;
                case "drawFeaturePointsMenuItem":
                    processOptions.DrawFeaturePoints = Convert.ToInt32(s.IsChecked);
                    break;
                case "doEyeProcessingMenuItem":
                    processOptions.DoEyeProcessing = Convert.ToInt32(s.IsChecked);
                    break;
                case "doMouthProcessingMenuItem":
                    processOptions.DoMouthProcessing = Convert.ToInt32(s.IsChecked);
                    break;
                case "doVisualDebugMenuItem":
                    processOptions.DoVisualDebug = Convert.ToInt32(s.IsChecked);
                    break;
            };

            if (filesToProcess.Count > 0)
            {
                fileProcessIdx--;
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
            if (captureTimer.IsEnabled)
            {
                captureTimer.Stop();
            }
            else
            {
                captureTimer.Start();
            }
        }

        private void expressionSelectorComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            ComboBox cbox = sender as ComboBox;
            if (e.Key == Key.Enter)
            {
                try
                {
                    dataSet.Expression.AddExpressionRow(cbox.Text, null);
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
        private float i2r(int coord, int begin, int dim)
        {
            return (float)(coord - begin) / (float)dim;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int expressionOID = Convert.ToInt32(expressionSelectorComboBox.SelectedValue);

            // Convert facial coordinates (for mouth only atm.) to relative face coordinates.

            int x = ffp.FaceRectangle.X;
            int y = ffp.FaceRectangle.Y;
            int w = ffp.FaceRectangle.Width;
            int h = ffp.FaceRectangle.Height;
            
            // Calculate the eye distance
            float dx = i2r(ffp.RightEye.EyeCenter.X - ffp.LeftEye.EyeCenter.X, x, w);
            float dy = i2r(ffp.RightEye.EyeCenter.Y - ffp.LeftEye.EyeCenter.Y, y, h);
            float eyeDist = (float) Math.Sqrt(dx * dx + dy * dy);

            dataSet.Training.AddTrainingRow(
                dataSet.Expression.FindByExpressionOID(expressionOID),
                null,
                i2r(ffp.Mouth.LipCornerLeft.X, x, w),   i2r(ffp.Mouth.LipCornerLeft.Y, y, h), 
                i2r(ffp.Mouth.LipCornerRight.X, x, w),  i2r(ffp.Mouth.LipCornerRight.Y, y, h), 
                i2r(ffp.Mouth.LipUpLeft.X, x, w),       i2r(ffp.Mouth.LipUpLeft.Y, y, h),
                i2r(ffp.Mouth.LipUpCenter.X, x, w),     i2r(ffp.Mouth.LipUpCenter.Y, y, h), 
                i2r(ffp.Mouth.LipUpRight.X, x, w),      i2r(ffp.Mouth.LipUpRight.Y, y, h), 
                i2r(ffp.Mouth.LipBottomLeft.X, x, w),   i2r(ffp.Mouth.LipBottomLeft.Y, y, h), 
                i2r(ffp.Mouth.LipBottomCenter.X, x, w), i2r(ffp.Mouth.LipBottomCenter.Y, y, h),
                i2r(ffp.Mouth.LipBottomRight.X, x, w),  i2r(ffp.Mouth.LipBottomRight.Y, y, h),
                eyeDist
            );

            // Add a thumbnail to the image table
            const int thumbnailWidth = 50;
            const int thumbnailHeight = 50;

            System.Drawing.Image.GetThumbnailImageAbort tc = new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback);
            System.Drawing.Image thumbnail = tmpBitmap.GetThumbnailImage(thumbnailWidth, thumbnailHeight, tc, IntPtr.Zero);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            thumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            dataSet.Image.AddImageRow(thumbnailWidth, thumbnailHeight, 3, 0, ms.ToArray());
        }

        /// <summary>
        /// Pops up a dialog to configure the capture device's parameters.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraParameters_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            klu.ConfigureCaptureDialog();
        }

        /// <summary>
        /// Pops up a dialog to configure the capture device's resolution.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void captureResolutionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            klu.ConfigureCaptureResolutionDialog();
        }

        private void About_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("This is KLU a Facial Feature Point (FFP) detector and Facial Expression "
            +"Analyzation tool. The project is maintained at the South Westphalia University of Applied Science "
            +" by Konrad Kleine and Jens Lukowski.", "About", MessageBoxButton.OK, MessageBoxImage.Information);
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
            // Start Expressions Dialog here.
            ExpressionsDialog dlg = new ExpressionsDialog(ref dataSet);

            // Configure the dialog box
            dlg.Owner = this;

            // Open the dialog box modally 
            dlg.ShowDialog();
        }

        private void TrainingDataSetsDialog_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Start Expressions Dialog here.
            TrainingDataSetsDialog dlg = new TrainingDataSetsDialog(ref dataSet);

            // Configure the dialog box
            dlg.Owner = this;

            // Open the dialog box modally 
            dlg.ShowDialog();
        }

        private void AnnDialog_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Start Expressions Dialog here.
            AnnDialog dlg = new AnnDialog(ref klu);

            // Configure the dialog box
            dlg.Owner = this;

            // Open the dialog box modally 
            dlg.ShowDialog();
        }
    }
}
