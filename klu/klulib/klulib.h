// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the KLULIB_EXPORTS
// symbol defined on the command line. this symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// KLULIB_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#include "klu_types.h"

extern "C" {

#define ANN_INPUT_SIZE 4;
#define ANN_OUTPUT_SIZE 1;

    KLULIB_API int klu_createCapture(void);

    KLULIB_API int klu_freeCapture(void);

    KLULIB_API int klu_getLastProcessedImage(unsigned char ** data, 
        int * width, 
        int * height, 
        int * nChannels, 
        int * widthStep);

    KLULIB_API int klu_getLastProcessedImageDims(int * width, int * height);

    KLULIB_API int klu_initializeLibrary(void);

    KLULIB_API int klu_deinitializeLibrary(void);

    KLULIB_API int klu_createAndSaveAnn(int * numNeuronsPerLayer, 
        int numLayers, 
        int activationFunction, 
        double alpha,
        double beta,
        const char * filepath); 

    /**
     * TODO: (Ko) Complete the implementation and create + fill numNeuronsPerLayer
     * TODO: (Ko) Add alpha and beta
     * TODO: (Ko) Complete activation function
     */
    KLULIB_API int klu_loadAnnAndGetStructure(int * numNeuronsPerLayer, 
        int * numLayers, 
        int * activationFunction, 
        const char * filepath);

    KLULIB_API int klu_loadAnn(const char * filepath);

    KLULIB_API int klu_saveAnn(const char * filepath);

    /**
     * Automatically initializes the weights and normalizes the inputs.
     */
    KLULIB_API int klu_trainAnn(const KluTrainOptions * options,
                                int numTrainingSets,
                                float * inputs,
                                int numInputNeurons,
                                float * outputs,
                                int numOutputNeurons);

    KLULIB_API int klu_predictAnn(float * inputs,
                                  int numInputNeurons,
                                  float * results,
                                  int numResults);

    KLULIB_API int klu_processStillImage(const char * filepath, 
        KluProcessOptions * processOptions, 
        KluFaceFeaturePoints * ffp);

    KLULIB_API int klu_processCaptureImage(KluProcessOptions * processOptions, 
        KluFaceFeaturePoints * ffp);

    KLULIB_API int klu_configureCaptureDialog();

}
