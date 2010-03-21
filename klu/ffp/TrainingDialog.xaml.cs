using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Threading;
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
using AForge;
using AForge.Controls;
using AForge.Math;
using AForge.Neuro;
using AForge.Neuro.Learning;
using ffp.TrainingDataSetTableAdapters;
using KluSharp;
using Microsoft.Windows.Controls.Ribbon;


namespace ffp
{
    /// <summary>
    /// Interaction logic for TrainingDialog.xaml
    /// </summary>
    public partial class TrainingDialog : Window
    {
        TrainingDataSet _DataSet;
        DataSet _DataSetANN;
        
        /// <summary>
        /// This is a small wrapper around the structure NOT the logic of an ANN.
        /// </summary>
        ANN _ANN;

        ActivationNetwork _Network;

        Chart _Chart;
        BackgroundWorker _BackgroundWorker;
        static int _Series = 0;
        double _LearningRate;
        double _SigmoidAlpha;
        int _MaxIterations;
        double _Epsilon;
        double _Momentum;

        const int _NumInputNeurons = 38;
        const int _NumOutputNeurons = 7;

        class ProgressState
        {
            public double Error { get; set; }
            public int Iteration { get; set; }
        };

        public TrainingDialog(ref TrainingDataSet dataSet)
        {
            InitializeComponent();

            _BackgroundWorker = new BackgroundWorker();
            _BackgroundWorker.WorkerReportsProgress = true;
            _BackgroundWorker.WorkerSupportsCancellation = true;
            _BackgroundWorker.DoWork += new DoWorkEventHandler(DoWork);
            _BackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);
            _BackgroundWorker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);        

            _DataSet = dataSet;

             _Chart = new Chart();
            windowsFormsHost1.Child = _Chart;

            _Series = 0;

            StopButton.IsEnabled = false;
        
            _Network = new ActivationNetwork(
                new SigmoidFunction(),
                _NumInputNeurons,
                9,
                _NumOutputNeurons
            );

            #region Initialize ANN stuff
            _ANN = new ANN();
            _ANN.NumLayers = 3;
            _ANN.SetNumNeurons(0, _NumInputNeurons);
            _ANN.SetNumNeurons(1, 9);
            _ANN.SetNumNeurons(2, _NumOutputNeurons);

            // Bind DataGrid to ANN-DataSet now
            _DataSetANN = new DataSet("HiddenLayer");
            _DataSetANN.Tables.Add("HiddenLayerTable");
            uint tmp = 9;
            _DataSetANN.Tables[0].Columns.Add("Neurons", tmp.GetType());
            _DataSetANN.Tables[0].Rows.Add(tmp);
            HiddenLayerDataGrid.DataContext = _DataSetANN.Tables[0];
            #endregion
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            e.Result = new double[0];

            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            TrainingDialog obj = (TrainingDialog)e.Argument;

            _Series++;

            #region Prepare data to be trained. Involves copying.

            int numTrainingSets = obj._DataSet.Training.Rows.Count;

            ArrayList inputs = new ArrayList();
            ArrayList outputs = new ArrayList();

            for (int i = 0; i < numTrainingSets; i++)
            {
                // Input data
                double[] inData = new double[_NumInputNeurons];

                inData[0] = obj._DataSet.Training[i].LipCornerLeftX;
                inData[1] = obj._DataSet.Training[i].LipCornerLeftY;
                inData[2] = obj._DataSet.Training[i].LipCornerRightX;
                inData[3] = obj._DataSet.Training[i].LipCornerRightY;
                inData[4] = obj._DataSet.Training[i].LipUpLeftX;
                inData[5] = obj._DataSet.Training[i].LipUpLeftY;
                inData[6] = obj._DataSet.Training[i].LipUpCenterX;
                inData[7] = obj._DataSet.Training[i].LipUpCenterY;
                inData[8] = obj._DataSet.Training[i].LipUpRightX;
                inData[9] = obj._DataSet.Training[i].LipUpRightY;
                inData[10] = obj._DataSet.Training[i].LipBottomLeftX;
                inData[11] = obj._DataSet.Training[i].LipBottomLeftY;
                inData[12] = obj._DataSet.Training[i].LipBottomCenterX;
                inData[13] = obj._DataSet.Training[i].LipBottomCenterY;
                inData[14] = obj._DataSet.Training[i].LipBottomRightX;
                inData[15] = obj._DataSet.Training[i].LipBottomRightY;
                inData[16] = obj._DataSet.Training[i].LeftEyeCenterX;
                inData[17] = obj._DataSet.Training[i].LeftEyeCenterY;
                inData[18] = obj._DataSet.Training[i].LeftLidBottomX;
                inData[19] = obj._DataSet.Training[i].LeftLidBottomY;
                inData[20] = obj._DataSet.Training[i].LeftLidCornerLeftX;
                inData[21] = obj._DataSet.Training[i].LeftLidCornerLeftY;
                inData[22] = obj._DataSet.Training[i].LeftLidCornerRightX;
                inData[23] = obj._DataSet.Training[i].LeftLidCornerRightY;
                inData[24] = obj._DataSet.Training[i].LeftLidUpX;
                inData[25] = obj._DataSet.Training[i].LeftLidUpY;
                inData[26] = obj._DataSet.Training[i].MouthCenterX;
                inData[27] = obj._DataSet.Training[i].MouthCenterY;
                inData[28] = obj._DataSet.Training[i].RightEyeCenterX;
                inData[29] = obj._DataSet.Training[i].RightEyeCenterY;
                inData[30] = obj._DataSet.Training[i].RightLidBottomX;
                inData[31] = obj._DataSet.Training[i].RightLidBottomY;
                inData[32] = obj._DataSet.Training[i].RightLidCornerLeftX;
                inData[33] = obj._DataSet.Training[i].RightLidCornerLeftY;
                inData[34] = obj._DataSet.Training[i].RightLidCornerRightX;
                inData[35] = obj._DataSet.Training[i].RightLidCornerRightY;
                inData[36] = obj._DataSet.Training[i].RightLidUpX;
                inData[37] = obj._DataSet.Training[i].RightLidUpY;

                inputs.Add(inData);

                // Output data
                double[] outData = new double[_NumOutputNeurons];

                int eid = obj._DataSet.Training[i].ExpressionOID;
                string expression = obj._DataSet.Expression.FindByExpressionOID(eid).Expression.ToLower();

                outData[0] = expression.Contains("anger") ? 1 : 0;
                outData[1] = expression.Contains("disg") ? 1 : 0;
                outData[2] = expression.Contains("fear") ? 1 : 0;
                outData[3] = expression.Contains("happy") ? 1 : 0;
                outData[4] = expression.Contains("neutr") ? 1 : 0;
                outData[5] = expression.Contains("sad") ? 1 : 0;
                outData[6] = expression.Contains("surp") ? 1 : 0;

                outputs.Add(outData);
            }
            #endregion

            #region Norm datasets per input neuron
            for (int j = 0; j < _NumInputNeurons; j++)
            {
                double min = 100000000.0;
                double max = -100000000.0;

                for (int i = 0; i < numTrainingSets; i++)
                {
                    double cur = ((double[])inputs[i])[j];

                    if (min > cur)
                    {
                        min = cur;
                    }
                    if (max < cur)
                    {
                        max = cur;
                    }
                }

                for (int i = 0; (max - min) != 0 && i < numTrainingSets; i++)
                {
                    ((double[])inputs[i])[j] = (((double[])inputs[i])[j] - min) / (max - min);
                }
            }
            #endregion

            #region Pick random train-, validate and test datasets

            // Like Mr. Schneider ;)
            int numTestDataSets = (int)Math.Floor((double)numTrainingSets * 0.1);
            int numValidationDataSets = (int)Math.Floor((double)(numTrainingSets - numTestDataSets) * 0.2);
            int numTrainDataSets = numTrainingSets - numTestDataSets - numValidationDataSets;

            Random rand = new Random();

            // Get random training data
            double[][] trainingInputs = new double[numTrainDataSets][];
            double[][] trainingOutputs = new double[numTrainDataSets][];
            for (int i = 0; i < trainingInputs.GetLength(0); i++)
            {
                int idx = rand.Next(inputs.Count);
                trainingInputs[i] = (double[])inputs[idx];
                trainingOutputs[i] = (double[])outputs[idx];
                inputs.Remove(idx);
                outputs.Remove(idx);
            }

            // Get random validation data
            double[][] validateInputs = new double[numValidationDataSets][];
            double[][] validateOutputs = new double[numValidationDataSets][];
            for (int i = 0; i < validateInputs.GetLength(0); i++)
            {
                int idx = rand.Next(numTrainDataSets);
                validateInputs[i] = trainingInputs[idx];
                validateOutputs[i] = trainingOutputs[idx];
            }

            // Get random test data
            double[][] testInputs = new double[numTestDataSets][];
            double[][] testOutputs = new double[numTestDataSets][];
            for (int i = 0; i < testInputs.GetLength(0); i++)
            {
                int idx = rand.Next(inputs.Count);
                testInputs[i] = (double[])inputs[idx];
                testOutputs[i] = (double[])outputs[idx];
                inputs.Remove(idx);
                outputs.Remove(idx);
            }

            #endregion

            #region

            _Network.SetActivationFunction(new SigmoidFunction(_SigmoidAlpha));

            BackPropagationLearning teacher = new BackPropagationLearning(_Network);
            teacher.LearningRate = obj._LearningRate;
            teacher.Momentum = obj._Momentum;

            double error = 1;
            double maxError = -10000.0;
            int maxIterations = obj._MaxIterations;
            double epsilon = obj._Epsilon;

            // Prepare the error Chart
            double[,] errorValues = new double[maxIterations, 2];

            ProgressState state = new ProgressState();

            double[] errors = new double[maxIterations];

            for (int i = 0; i < maxIterations || error <= epsilon; i++)
            {
                if (maxIterations > 0)
                {
                    if (i >= maxIterations)
                    {
                        break;
                    }
                }
                if (epsilon > 0.0)
                {
                    if (error <= epsilon)
                    {
                        break;
                    }
                }
                // Abort if use requested it
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                // Train
                error = teacher.RunEpoch(trainingInputs, trainingOutputs);

                // Store error for result
                errors[i] = error;
                state.Error = error;

                // Plot size
                if (maxError < error)
                {
                    maxError = error;
                }

                // Plot values
                errorValues[i, 0] = i;
                errorValues[i, 1] = error;

                // Report progress
                if (i % 10 == 0 || i == (maxIterations - 1))
                {
                    state.Iteration = i + 1;
                    state.Error = error;

                    worker.ReportProgress((int)((float)(100 * (i + 1)) / (float)maxIterations), state);
                }
            }        

            e.Result = errors;

            
            obj._Chart.RangeX = new DoubleRange(0.0, (double)maxIterations);
            obj._Chart.RangeY = new DoubleRange(0.0, maxError);

            // add new data series to the chart
            obj._Chart.AddDataSeries("Error " + _Series, System.Drawing.Color.DarkGreen, Chart.SeriesType.ConnectedDots, 2);

            // update the chart
            obj._Chart.UpdateDataSeries("Error " + _Series, errorValues);

            #endregion
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;

            BackgroundWorker worker = sender as BackgroundWorker;

            if (worker.CancellationPending || e.Cancelled)
            {
                return;
            }

            double[] errors = (double[])e.Result;

            double meanError = 0.0;
            double meanAbsoluteError = 0.0;
            double maxError = -100000.0;
            double meanSquaredError = 0.0;
            double median = 0.0;
            double standardDeviation = 0.0;

            // Do mean, mean absolute and max error
            for (int i = 0; i < errors.GetLength(0); i++)
            {
                meanError += errors[i];
                meanAbsoluteError += Math.Abs(errors[i]);
                if (maxError < errors[i])
                {
                    maxError = errors[i];
                }
            }

            // Do mean error
            meanError = meanError / (double)errors.GetLength(0);
            meanAbsoluteError = meanAbsoluteError / (double)errors.GetLength(0);

            // Do standard deviation
            for (int i = 0; i < errors.GetLength(0); i++)
            {
                meanSquaredError += (errors[i] - meanError) * (errors[i] - meanError);                
            }
            meanSquaredError /= (double)errors.GetLength(0);

            // Do standard deviation
            standardDeviation = Math.Sqrt(meanSquaredError);

            // Do median
            ArrayList tmp = new ArrayList(errors);
            tmp.Sort();
            if (tmp.Count % 2 == 0)
            {
                median = (double)tmp[tmp.Count / 2] + (double)tmp[tmp.Count / 2 - 1];
                median /= 2.0;
            }
            else
            {
                median = (double)tmp[tmp.Count / 2];
            }           

            // Show the results
            MeanError.Text = Convert.ToString(meanError);
            MeanAbsoluteError.Text = Convert.ToString(meanAbsoluteError);
            MaxError.Text = Convert.ToString(maxError);
            MeanSquaredError.Text = Convert.ToString(meanSquaredError);
            StandardDeviation.Text = Convert.ToString(standardDeviation);
            Median.Text = Convert.ToString(median);
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressState state = (ProgressState)e.UserState;

            CurrentIteration.Text = Convert.ToString(state.Iteration);
            CurrentError.Text = Convert.ToString(state.Error);
            TrainingProgress.Value = e.ProgressPercentage;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {            
            if (_DataSet.Training.Count < 1)
            {
                MessageBox.Show("Please, ensure you have at least one training dataset.");
                return;
            }                        

            //_LearningRate = Convert.ToDouble(LearningRate.Text);
            //_SigmoidAlpha = Convert.ToDouble(SigmoidAlpha.Text);
            //_MaxIterations = Convert.ToInt32(MaxIterations.Text);
            //_Epsilon = Convert.ToDouble(Epsilon.Text,);
            //_Momentum = Convert.ToDouble(Momentum.Text);


            _LearningRate = Convert.ToDouble(LearningRate.Text.Replace('.',','));
            _SigmoidAlpha = Convert.ToDouble(SigmoidAlpha.Text.Replace('.', ','));
            _MaxIterations = Convert.ToInt32(MaxIterations.Text.Replace('.', ','));
            _Epsilon = Convert.ToDouble(Epsilon.Text.Replace('.', ','));
            _Momentum = Convert.ToDouble(Momentum.Text.Replace('.', ','));

            
            // Clear labels
            MeanError.Text = "";
            MeanAbsoluteError.Text = "";
            MaxError.Text = "";
            MeanSquaredError.Text = "";
            StandardDeviation.Text = "";
            Median.Text = "";

            _BackgroundWorker.RunWorkerAsync(this);

            TrainingProgress.Value = 0;

            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
        }

        private void ClearGraph_Click(object sender, RoutedEventArgs e)
        {
            _Chart.RemoveAllDataSeries();
            _Series = 0;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _BackgroundWorker.CancelAsync();
        
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
        }

        /// <summary>
        /// Draws the ANN on the appropriate Canvas.
        /// </summary>
        private void DrawANN()
        {
            // Clear Canvas
            AnnCanvas.Children.Clear();

            // Save positions of neurons in this array
            Point[] neuronPos = new Point[_ANN.GetTotalNumberOfNeurons()];

            double neuronDiameter = 10.0;
            double layerDist = (double)AnnCanvas.ActualWidth / (double)(_ANN.NumLayers - 1) - neuronDiameter;

            RadialGradientBrush neuronBrush = new RadialGradientBrush();
            neuronBrush.RadiusX = 1.0;
            neuronBrush.RadiusY = 1.0;
            neuronBrush.GradientOrigin = new Point(0.7, 0.3);
            neuronBrush.GradientStops.Add(new GradientStop(Colors.White, 0.0));
            neuronBrush.GradientStops.Add(new GradientStop(Colors.Black, 1.0));

            #region Iterate over every layer
            for (int l = 0; l < _ANN.NumLayers; l++)
            {

                double neuronDist = 0;

                if (_ANN.GetNumNeurons(l) > 1)
                {
                    neuronDist = (double)AnnBorder.ActualHeight / ((double)_ANN.GetNumNeurons(l));
                }
                else
                {
                    neuronDist = (double)AnnBorder.ActualHeight / ((double)_ANN.GetNumNeurons(l));
                }

                #region Iterare over every neuron on the current layer
                for (int n = 0; n < _ANN.GetNumNeurons(l); n++)
                {
                    Ellipse e = new Ellipse();
                    e.Stroke = Brushes.White;
                    e.Width = neuronDiameter;
                    e.Height = neuronDiameter;
                    e.StrokeThickness = 2.0;
                    e.Fill = Brushes.White;

                    Canvas.SetLeft(e, layerDist * l);
                    Canvas.SetTop(e, neuronDist * n);

                    AnnCanvas.Children.Add(e);

                    // Safe the point of the current neuron for line drawing
                    neuronPos[_ANN.GetNumberOfNeuronsBefore(l) + n] = new Point(layerDist * l, neuronDist * n);

                    #region Draw all connections from previous layer to current neuron.
                    for (int i = 0; l > 0 && i < _ANN.GetNumNeurons(l - 1); i++)
                    {
                        Line line = new Line();
                        line.X1 = neuronPos[_ANN.GetNumberOfNeuronsBefore(l - 1) + i].X + neuronDiameter / 2.0;
                        line.Y1 = neuronPos[_ANN.GetNumberOfNeuronsBefore(l - 1) + i].Y + neuronDiameter / 2.0;
                        line.X2 = neuronPos[_ANN.GetNumberOfNeuronsBefore(l) + n].X + neuronDiameter / 2.0;
                        line.Y2 = neuronPos[_ANN.GetNumberOfNeuronsBefore(l) + n].Y + neuronDiameter / 2.0;
                        line.Stroke = Brushes.White;
                        line.StrokeThickness = 1;
                        line.Opacity = 0.5;

                        AnnCanvas.Children.Add(line);
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
        private void AnnCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
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
            _ANN.NumLayers = 2 + _DataSetANN.Tables[0].Rows.Count;
            _ANN.SetNumNeurons(0, 38);
            _ANN.SetNumNeurons(_DataSetANN.Tables[0].Rows.Count + 1, _NumOutputNeurons);

            int[] neuronsCount = new int[_DataSetANN.Tables[0].Rows.Count + 1];
            neuronsCount[neuronsCount.GetLength(0)-1] = _NumOutputNeurons;

            for (int i = 0; i < _DataSetANN.Tables[0].Rows.Count; i++)
            {
                int neuronCount = Convert.ToInt32(_DataSetANN.Tables[0].Rows[i].ItemArray[0]);

                neuronsCount[i] = neuronCount;

                _ANN.SetNumNeurons(i + 1, neuronCount);
            }

            _Network = new ActivationNetwork(new SigmoidFunction(_SigmoidAlpha), _NumInputNeurons, neuronsCount);

            DrawANN();
        }

        private void LoadAnnButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "NeuralNetwork"; // Default file name
            dlg.DefaultExt = ".klu"; // Default file extension
            dlg.Filter = "KLU ANN (.klu)|*.klu"; // Filter files by extension
            dlg.Title = "Load Neural Network";

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                _Network = (ActivationNetwork) ActivationNetwork.Load(dlg.FileName);

                _DataSetANN.Clear();

                _ANN.NumLayers = _Network.LayersCount + 1;
                _ANN.SetNumNeurons(0, _Network.InputsCount);
                
                for (int i=0; i<_Network.LayersCount; i++)
                {
                    _ANN.SetNumNeurons(i + 1, _Network[i].NeuronsCount);

                    if (i < (_Network.LayersCount - 1))
                    {
                        _DataSetANN.Tables[0].Rows.Add(_Network[i].NeuronsCount);
                    }
                }

                DrawANN();
            }
        }

        private void SaveAnnButton_Click(object sender, RoutedEventArgs e)
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "NeuralNetwork"; // Default file name
            dlg.DefaultExt = ".klu"; // Default file extension
            dlg.Filter = "KLU ANN (.klu)|*.klu"; // Filter files by extension
            dlg.Title = "Where to save your new Neural Network?";

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                _Network.Save(dlg.FileName);
            }
        }
    }
}
