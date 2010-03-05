using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ffp
{
    // Represents the basic structure of an artifical neural network
    class ANN
    {
        private int[] numNeuronsPerLayer = null;
        
        // The number of layers in the neural network
        public int NumLayers
        {
            get {
                return numNeuronsPerLayer.GetLength(0);
            }
            set {
                numNeuronsPerLayer = new int[value];
            }
        }

        // Returns the total number of neurons
        public int GetTotalNumberOfNeurons()
        {
            return numNeuronsPerLayer.Sum();
        }

        // Returns the number of neurons on all layers before layer
        public int GetNumberOfNeuronsBefore(int layer)
        {
            int res = 0;
            for (int l = 0; l < NumLayers && l < layer; l++)
            {
                res += numNeuronsPerLayer[l];
            }
            return res;                
        }

        // Sets the number of neurons on layer to numNeurons.
        public void SetNumNeurons(int layer, int numNeurons)
        {
            if (layer > (NumLayers - 1) || layer < 0)
            {
                throw new Exception("Not a valid layer: " + layer);
            }
            numNeuronsPerLayer[layer] = numNeurons;
        }

        // Returns the number of neurons on layer.
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
