#include "../../common/src/common.hpp"

using namespace klu;
using namespace std;
using namespace cv;

/**
 * The main program
 */
int main(int argc, char * argv[]) {
	std::cerr << "Neural Network" << std::endl;

	/**
	 * Setup ANN
	 */
	// Set how many layers we have
	const int nLayers = 3;
	int layers[nLayers] = { 3, 6, 1 };
	CvMat layerSizes = cvMat(1, nLayers, CV_32SC1, layers);
	// All neurons get the same activation function.
	// Sadly one cannot set the first layer to be ident function.
	int activationFunction = CvANN_MLP::IDENTITY;
	// Parameters for activation function
	double alpha = 1.0;
	double beta = 1.0;

	// Create the ANN
	CvANN_MLP net(&layerSizes, activationFunction, alpha, beta);
//
//	// Initialize the weights using Nguyen Widrow algorithm
//	//net.init_weights();
//
	CvMLData data;
	data.set_delimiter(',');
	if (data.read_csv("../data/sample.csv") != 0) {
		cerr << "Failed to read CSV data" << endl;
	}
//
//	//CvTrainTestSplit split(2);
//	//data.set_train_test_split(&split);
//
	const CvMat * values = data.get_values();
//	cvCloneMat()
//
	cout << "values" << endl;
    printMat(values);
//	cout << "get_train_sample_idx" << endl;
//	printMat(get_train_sample_idx);
//
//	CvFileStorage * paramsStorage = cvOpenFileStorage("../data/params.xml", NULL, CV_STORAGE_READ);
//
//	MyMLP net;
//	net.readParams(paramsStorage, NULL);
//	cvReleaseFileStorage(&paramsStorage);

	CvFileStorage * writeStorage = cvOpenFileStorage("../data/net.xml", NULL, CV_STORAGE_WRITE);
	net.write(writeStorage, "bla");
	cvReleaseFileStorage(&writeStorage);

	return 0;
}
