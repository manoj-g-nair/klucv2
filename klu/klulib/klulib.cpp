// klulib.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "klulib.h"
#include "klu_common.hpp"

extern "C" {
//------------------------------------------------------------------------------
CvCapture * g_capture = NULL;
//------------------------------------------------------------------------------
KLULIB_API int createCapture(void)
{
    if ( !g_capture )
    {
        g_capture = cvCreateCameraCapture(CV_CAP_ANY);

        if ( g_capture )
        {        
            // Manipulate properties of the camera.
		    cvSetCaptureProperty(g_capture, CV_CAP_PROP_FRAME_WIDTH, double(320));
		    cvSetCaptureProperty(g_capture, CV_CAP_PROP_FRAME_HEIGHT, double(240));
		    int camWidth = cvGetCaptureProperty(g_capture, CV_CAP_PROP_FRAME_WIDTH);
		    int camHeight = cvGetCaptureProperty(g_capture, CV_CAP_PROP_FRAME_HEIGHT);

            if ( camWidth != 320 || camHeight != 240 )
            {
                freeCapture();
            }
        }
    }
    
    return g_capture ? 1 : 0;
}
//------------------------------------------------------------------------------
KLULIB_API void freeCapture(void)
{
    if ( g_capture )
    {
        cvReleaseCapture(&g_capture);
        g_capture = NULL; // This is probably not needed. Anyway, be safe.
    }
}
//------------------------------------------------------------------------------
KLULIB_API void queryCaptureImage(unsigned char ** data, int * width, int * height, int * nChannels, int * widthStep)
{
    IplImage * image = cvQueryFrame(g_capture);
    *data = reinterpret_cast<unsigned char*>(image->imageData);
    *width = image->width;
    *height = image->height;
    *nChannels = image->nChannels;
    *widthStep = image->widthStep;
}
//------------------------------------------------------------------------------
KLULIB_API int klu_initializeLibrary(void)
{
    return klu::initializeLibrary() ? 1 : 0;
}
//------------------------------------------------------------------------------
KLULIB_API int klu_deinitializeLibrary(void)
{
    return klu::deinitializeLibrary() ? 1 : 0;
}
//------------------------------------------------------------------------------

KLULIB_API int testStruct(KluTestStruct * p)
{
    return p->x + p->y;
}
//------------------------------------------------------------------------------
KLULIB_API int klu_createAndSaveAnn(int * numNeuronsPerLayer, int numLayers, int activationFunction, const char * filepath)
{
    if ( !numNeuronsPerLayer || !filepath || numLayers < 0 )
    {
        return 0;
    }

    CvMat layerSizes = cvMat(1, numLayers, CV_32SC1, numNeuronsPerLayer);

	// Parameters for activation function
	double alpha = 1.0;
	double beta = 1.0;

	//// Create the ANN
	CvANN_MLP net(&layerSizes, activationFunction, alpha, beta);

    net.save(filepath, "FFPANN");
  
    return 1;
}
//------------------------------------------------------------------------------
} // end extern "C" {