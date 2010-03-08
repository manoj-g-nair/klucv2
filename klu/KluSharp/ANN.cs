using System;
using System.Linq;

namespace KluSharp
{
    /// <summary>
    /// Represents the basic structure of an artifical neural network
    /// </summary>
    public class ANN
    {
        public enum ActivationFunction { Identity = 0, Sigmoid = 1, Gaussian = 2 };

        private int[] numNeuronsPerLayer = null;

        /// <summary>
        /// Gets the number of neurons on each layer as an array.
        /// </summary>
        public int[] NumNeuronsPerLayer
        {
            get { return numNeuronsPerLayer; }
        }

        /// <summary>
        /// Gets or sets the number of layers in the neural network.
        /// </summary>
        public int NumLayers
        {
            get {
                return numNeuronsPerLayer.GetLength(0);
            }
            set {
                numNeuronsPerLayer = new int[value];
            }
        }

        /// <summary>
        /// Returns the total number of neurons
        /// </summary>
        /// <returns></returns>
        public int GetTotalNumberOfNeurons()
        {
            return numNeuronsPerLayer.Sum();
        }

        /// <summary>
        /// Gets the total number of neurons in the ANN.
        /// </summary>
        public int TotalNumberOfNeurons
        {
            get { return GetTotalNumberOfNeurons(); }
        }

        /// <summary>
        /// Returns the number of neurons on all layers before layer
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public int GetNumberOfNeuronsBefore(int layer)
        {
            int res = 0;
            for (int l = 0; l < NumLayers && l < layer; l++)
            {
                res += numNeuronsPerLayer[l];
            }
            return res;                
        }

        /// <summary>
        /// Sets the number of neurons on layer to numNeurons.
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="numNeurons"></param>
        public void SetNumNeurons(int layer, int numNeurons)
        {
            if (layer > (NumLayers - 1) || layer < 0)
            {
                throw new Exception("Not a valid layer: " + layer);
            }
            numNeuronsPerLayer[layer] = numNeurons;
        }

        /// <summary>
        /// Returns the number of neurons on layer.
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public int GetNumNeurons(int layer)
        {
            if (layer > (NumLayers - 1) || layer < 0)
            {
                throw new Exception("Not a valid layer: " + layer);
            }
            return numNeuronsPerLayer[layer];
        }
    }
}
