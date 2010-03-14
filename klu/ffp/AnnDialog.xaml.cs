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
    /// Interaction logic for AnnDialog.xaml
    /// </summary>
    public partial class AnnDialog : Window
    {
        /// <summary>
        /// This contains all the information about the hidden layers of the ANN.
        /// We use a DataSet for this purpose because it can be easily sychronized with
        /// a DataGrid.
        /// </summary>
        DataSet _DataSet;

        /// <summary>
        /// This is a small wrapper around the structure NOT the logic of an ANN.
        /// </summary>
        ANN _ANN;

        /// <summary>
        /// This member is a wrapper object provides access to the klu C DLL.
        /// </summary>
        Klu _Klu;

        /// <summary>
        /// Constructs a new AnnDialog.
        /// </summary>
        public AnnDialog(ref Klu Klu)
        {
            InitializeComponent();

            _Klu = Klu;

            #region Initialize ANN stuff
            _ANN = new ANN();
            _ANN.NumLayers = 3;
            _ANN.SetNumNeurons(0, 16);
            _ANN.SetNumNeurons(1, 6);
            _ANN.SetNumNeurons(2, 1);

            // Bind certain labels to ANN stuff
            AnnNumLayers.DataContext = _ANN;

            // Bind DataGrid to ANN-DataSet now
            _DataSet = new DataSet("HiddenLayer");
            _DataSet.Tables.Add("HiddenLayerTable");
            uint tmp = 0;
            _DataSet.Tables[0].Columns.Add("Neurons", tmp.GetType());
            tmp = 6;
            _DataSet.Tables[0].Rows.Add(tmp);
            HiddenLayerDataGrid.DataContext = _DataSet.Tables[0];
            #endregion
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

            double neuronDiameter = 20.0;
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
            _ANN.NumLayers = 2 + _DataSet.Tables[0].Rows.Count;
            _ANN.SetNumNeurons(0, 16);
            _ANN.SetNumNeurons(_DataSet.Tables[0].Rows.Count + 1, 1);

            for (int i = 0; i < _DataSet.Tables[0].Rows.Count; i++)
            {
                _ANN.SetNumNeurons(i + 1, Convert.ToInt32(_DataSet.Tables[0].Rows[i].ItemArray[0]));
            }

            DrawANN();
        }

        /// <summary>
        /// Saves the current ANN settings to an OpenCV ANN file which can be loaded
        /// later for initialization, training and use later.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAnnButton_Click(object sender, RoutedEventArgs e)
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
                string filepath = dlg.FileName;

                Console.WriteLine("Saving neural network to " + filepath);

                Hashtable hashtable = new Hashtable();
                hashtable.Add("identity", ANN.ActivationFunction.Identity);
                hashtable.Add("sigmoid", ANN.ActivationFunction.Sigmoid);
                hashtable.Add("gaussian", ANN.ActivationFunction.Gaussian);
                hashtable.Add("Identity", ANN.ActivationFunction.Identity);
                hashtable.Add("Sigmoid", ANN.ActivationFunction.Sigmoid);
                hashtable.Add("Gaussian", ANN.ActivationFunction.Gaussian);

                ANN.ActivationFunction actFunc = ANN.ActivationFunction.Sigmoid;
                if (hashtable.ContainsKey(AnnActivationFunction.Text))
                {
                    // TODO: (Ko) Use combobox key and not it's values (important for translation)
                    Console.WriteLine("Using activation function: " + hashtable[AnnActivationFunction.Text]);
                    actFunc = (ANN.ActivationFunction)hashtable[AnnActivationFunction.Text];
                }

                _Klu.CreateAndSaveAnn(_ANN.NumNeuronsPerLayer, actFunc, Convert.ToDouble(AnnAlpha.Text), Convert.ToDouble(AnnBeta.Text), filepath);
            }
        }

        private void LoadAnnButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
