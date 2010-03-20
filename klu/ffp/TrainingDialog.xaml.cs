using System;
using System.Collections;
using System.Data;
using System.ComponentModel;
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
using System.Threading;
using AForge;
using AForge.Controls;
using AForge.Math;
using AForge.Neuro;
using AForge.Neuro.Learning;
using ffp.TrainingDataSetTableAdapters;

namespace ffp
{
    /// <summary>
    /// Interaction logic for TrainingDialog.xaml
    /// </summary>
    public partial class TrainingDialog : Window
    {        
        TrainingDataSet _DataSet;
        Chart _Chart;
        BackgroundWorker _BackgroundWorker;
        static int _Series = 0;
        double _LearningRate;
        double _SigmoidAlpha;
        int _MaxIterations;
        double _Epsilon;
        double _Momentum;

        struct ProgressState
        {
            public double Error;
            public int Iteration;
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
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            TrainingDialog obj = (TrainingDialog) e.Argument;

            _Series++;

            #region Prepare data to be trained. Involves copying.

            int numTrainingSets = obj._DataSet.Training.Rows.Count;
            const int numInputNeurons = 38;
            const int numOutputNeurons = 7;
            double[][] inputs = new double[numTrainingSets][];
            double[][] outputs = new double[numTrainingSets][];

            for (int i = 0; i < numTrainingSets; i++)
            {
                inputs[i] = new double[numInputNeurons];
                inputs[i][0] = obj._DataSet.Training[i].LipCornerLeftX;
                inputs[i][1] = obj._DataSet.Training[i].LipCornerLeftY;
                inputs[i][2] = obj._DataSet.Training[i].LipCornerRightX;
                inputs[i][3] = obj._DataSet.Training[i].LipCornerRightY;
                inputs[i][4] = obj._DataSet.Training[i].LipUpLeftX;
                inputs[i][5] = obj._DataSet.Training[i].LipUpLeftY;
                inputs[i][6] = obj._DataSet.Training[i].LipUpCenterX;
                inputs[i][7] = obj._DataSet.Training[i].LipUpCenterY;
                inputs[i][8] = obj._DataSet.Training[i].LipUpRightX;
                inputs[i][9] = obj._DataSet.Training[i].LipUpRightY;
                inputs[i][10] = obj._DataSet.Training[i].LipBottomLeftX;
                inputs[i][11] = obj._DataSet.Training[i].LipBottomLeftY;
                inputs[i][12] = obj._DataSet.Training[i].LipBottomCenterX;
                inputs[i][13] = obj._DataSet.Training[i].LipBottomCenterY;
                inputs[i][14] = obj._DataSet.Training[i].LipBottomRightX;
                inputs[i][15] = obj._DataSet.Training[i].LipBottomRightY;
                inputs[i][16] = obj._DataSet.Training[i].LeftEyeCenterX;
                inputs[i][17] = obj._DataSet.Training[i].LeftEyeCenterY;
                inputs[i][18] = obj._DataSet.Training[i].LeftLidBottomX;
                inputs[i][19] = obj._DataSet.Training[i].LeftLidBottomY;
                inputs[i][20] = obj._DataSet.Training[i].LeftLidCornerLeftX;
                inputs[i][21] = obj._DataSet.Training[i].LeftLidCornerLeftY;
                inputs[i][22] = obj._DataSet.Training[i].LeftLidCornerRightX;
                inputs[i][23] = obj._DataSet.Training[i].LeftLidCornerRightY;
                inputs[i][24] = obj._DataSet.Training[i].LeftLidUpX;
                inputs[i][25] = obj._DataSet.Training[i].LeftLidUpY;
                inputs[i][26] = obj._DataSet.Training[i].MouthCenterX;
                inputs[i][27] = obj._DataSet.Training[i].MouthCenterY;
                inputs[i][28] = obj._DataSet.Training[i].RightEyeCenterX;
                inputs[i][29] = obj._DataSet.Training[i].RightEyeCenterY;
                inputs[i][30] = obj._DataSet.Training[i].RightLidBottomX;
                inputs[i][31] = obj._DataSet.Training[i].RightLidBottomY;
                inputs[i][32] = obj._DataSet.Training[i].RightLidCornerLeftX;
                inputs[i][33] = obj._DataSet.Training[i].RightLidCornerLeftY;
                inputs[i][34] = obj._DataSet.Training[i].RightLidCornerRightX;
                inputs[i][35] = obj._DataSet.Training[i].RightLidCornerRightY;
                inputs[i][36] = obj._DataSet.Training[i].RightLidUpX;
                inputs[i][37] = obj._DataSet.Training[i].RightLidUpY;

                outputs[i] = new double[numOutputNeurons];
                int eid = obj._DataSet.Training[i].ExpressionOID;
                string expression = obj._DataSet.Expression.FindByExpressionOID(eid).Expression.ToLower();

                outputs[i][0] = expression.Contains("anger") ? 1 : 0;
                outputs[i][1] = expression.Contains("disg") ? 1 : 0;
                outputs[i][2] = expression.Contains("fear") ? 1 : 0;
                outputs[i][3] = expression.Contains("happy") ? 1 : 0;
                outputs[i][4] = expression.Contains("neutr") ? 1 : 0;
                outputs[i][5] = expression.Contains("sad") ? 1 : 0;
                outputs[i][6] = expression.Contains("surp") ? 1 : 0;
            }
            #endregion

            ActivationNetwork network = new ActivationNetwork(
                new SigmoidFunction(obj._SigmoidAlpha),
                numInputNeurons,
                6,
                5,
                numOutputNeurons
            );

            BackPropagationLearning teacher = new BackPropagationLearning(network);
            teacher.LearningRate = obj._LearningRate;
            teacher.Momentum = obj._Momentum;

            double error = 1;
            double maxError = -10000.0;
            int maxIterations = obj._MaxIterations;
            double epsilon = obj._Epsilon;

            // Prepare the error Chart
            double[,] errorValues = new double[maxIterations, 2];

            ProgressState state = new ProgressState();

            for (int i = 0; i < maxIterations || error <= epsilon; i++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                error = teacher.RunEpoch(inputs, outputs);

                if (maxError < error)
                {
                    maxError = error;
                }

                errorValues[i, 0] = i;
                errorValues[i, 1] = error;

                if (i == 0)
                {
                    Console.WriteLine("Error at i=" + i + ": " + error);
                }

                if (i == (maxIterations - 1))
                {
                    Console.WriteLine("Error at i=" + i + ": " + error);
                }

                if (i % 10 == 0 || i == (maxIterations-1))
                {
                    state.Iteration = i + 1;
                    state.Error = error;

                    worker.ReportProgress((int)((float)(100 * (i + 1)) / (float)maxIterations), state);
                }
            }


            obj._Chart.RangeX = new DoubleRange(0.0, (double)maxIterations);
            obj._Chart.RangeY = new DoubleRange(0.0, maxError);

            // add new data series to the chart
            obj._Chart.AddDataSeries("Error " + _Series, System.Drawing.Color.DarkGreen, Chart.SeriesType.ConnectedDots, 2);

            // update the chart
            obj._Chart.UpdateDataSeries("Error " + _Series, errorValues);
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
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

            _LearningRate = Convert.ToDouble(LearningRate.Text);
            _SigmoidAlpha = Convert.ToDouble(SigmoidAlpha.Text);
            _MaxIterations = Convert.ToInt32(MaxIterations.Text);
            _Epsilon = Convert.ToDouble(Epsilon.Text);
            _Momentum = Convert.ToDouble(Momentum.Text);

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
    }
}
