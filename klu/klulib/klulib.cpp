// klulib.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "klulib.h"
#include "klu_common.hpp"
#include <Vfw.h>

#ifdef WIN32
typedef struct CvCaptureCAM_VFW
{
    void* vtable;
    CAPDRIVERCAPS caps;
    HWND   capWnd;
    VIDEOHDR* hdr;
    DWORD  fourcc;
    HIC    hic;
    IplImage* rgb_frame;
    IplImage frame;
} CvCaptureCAM_VFW;
#endif

using namespace klu;

extern "C" {    
    static IplImage * g_lastImage = NULL;
    //------------------------------------------------------------------------------
    klu::ApplicationEnvironment app;
    //------------------------------------------------------------------------------
    KLULIB_API int klu_createCapture(void)
    {
        if ( !app.capture )
        {  
#ifdef WIN32
            app.capture = cvCreateCameraCapture(CV_CAP_VFW);
#else
            app.capture = cvCreateCameraCapture(CV_CAP_ANY);
#endif
        }

        return app.capture ? 1 : 0;
    }
    //------------------------------------------------------------------------------
    KLULIB_API int klu_freeCapture(void)
    {
        if ( app.capture )
        {
            cvReleaseCapture(&app.capture);
            app.capture = NULL; // This is probably not needed. Anyway, be safe.
        }
        return 1;
    }
    //------------------------------------------------------------------------------
    KLULIB_API int klu_getLastProcessedImage(unsigned char ** data, 
        int * width, 
        int * height, 
        int * nChannels, 
        int * widthStep)
    {
        if ( !data || !width || !height || !nChannels || !widthStep )
        {
            return 0;
        }

        *data = reinterpret_cast<unsigned char*>(app.lastImage->imageData);
        *width = app.lastImage->width;
        *height = app.lastImage->height;
        *nChannels = app.lastImage->nChannels;
        *widthStep = app.lastImage->widthStep;

        return 1;
    }
    //------------------------------------------------------------------------------
    KLULIB_API int klu_getLastProcessedImageDims(int * width, int * height)
    {
        if ( !width || !height || !app.lastImage)
        {
           return 0;
        }

        *width = app.lastImage->width;
        *height = app.lastImage->height;
        return 1;
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
    KLULIB_API int klu_createAndSaveAnn(int * numNeuronsPerLayer, 
        int numLayers, 
        int activationFunction,
        double alpha,
        double beta,
        const char * filepath)
    {
        if ( !numNeuronsPerLayer || !filepath || numLayers < 0 )
        {
            return 0;
        }

        //CvMat layerSizes = cvMat(1, numLayers, CV_32SC1, numNeuronsPerLayer);
        CvMat layerSizes = cvMat(numLayers, 1, CV_32SC1, numNeuronsPerLayer);

        //// Create the ANN
        CvANN_MLP net(&layerSizes, activationFunction, alpha, beta);

        net.save(filepath, "FFPANN");

        return 1;
    }
    //------------------------------------------------------------------------------
    KLULIB_API int klu_loadAnnAndGetStructure(int * numNeuronsPerLayer, 
        int * numLayers, 
        int * activationFunction, 
        const char * filepath)
    {
        if ( !numNeuronsPerLayer || !filepath || !numLayers || !activationFunction )
        {
            return 0;
        }

        app.ann.load(filepath, "FFPANN");

        *numLayers = app.ann.get_layer_count();
        
        // TODO: (Ko) Complete the implementation and create + fill numNeuronsPerLayer 
        // TODO: (Ko) Add alpha and beta
        // TODO: (Ko) Complete activation function

        return 1;
    }
    //------------------------------------------------------------------------------
    KLULIB_API int klu_loadAnn(const char * filepath)
    {
        if ( !filepath )
        {
            return 0;
        }

        app.ann.load(filepath, "FFPANN");

        return 1;
    }
    //------------------------------------------------------------------------------
    KLULIB_API int klu_saveAnn(const char * filepath)
    {
        if ( !filepath )
        {
            return 0;
        }

        app.ann.save(filepath, "FFPANN");

        return 1;
    }
    //------------------------------------------------------------------------------
    KLULIB_API int klu_trainAnn(const KluTrainOptions * options,
                                int numTrainingSets,
                                float * inputs,
                                int numInputNeurons,
                                float * outputs,
                                int numOutputNeurons,
                                int * terminatedAfterIter)
    {
        if ( !options || !inputs || !outputs || terminatedAfterIter )
        {
            return 0;
        }

        cv::Mat inMat(numTrainingSets, numInputNeurons, CV_32FC1, inputs);
        cv::Mat outMat(numTrainingSets, numOutputNeurons, CV_32FC1, outputs);
        cv::Mat weightMat(numTrainingSets, 1, CV_32FC1, cv::Scalar(1.0f));

        CvANN_MLP_TrainParams params(
                cvTermCriteria(
                    options->termination.terminationType,
                    options->termination.maxIteration,
                    options->termination.epsilon
                ),
                options->algorithm,
                options->algorithm == BackpropAlgorithm ? options->backpropDeltaWeightScale : options->rpropDeltaWeight0,
                options->algorithm == RpropAlgorithm ? options->backpropMomentumScale : options->rpropDeltaWeightMin
        );
        
        *terminatedAfterIter = app.ann.train(inMat, outMat, weightMat, cv::Mat(), params, 0);

        return 1;
    }
    //------------------------------------------------------------------------------
    KLULIB_API int klu_predictAnn(float * inputs,
                                  int numInputNeurons,
                                  float * results,
                                  int numResults)
    {
        if ( !inputs || !results )
        {
            return 0;
        }
   
        CvMat sample = cvMat(1, numInputNeurons, CV_32FC1, inputs);
        CvMat predout = cvMat(1, numResults, CV_32FC1, results);

        for (int i=0; i<numInputNeurons; i++)
        {
            sample.data.fl[i] = inputs[i];
        }

        app.ann.predict(&sample, &predout);

        return 1;
    }
    //------------------------------------------------------------------------------
    KLULIB_API int klu_processStillImage(const char * filepath,
        KluProcessOptions * processOptions, 
        KluFaceFeaturePoints * ffp)
    {
        if ( !filepath || !processOptions || !ffp )
        {
            return 0;
        }

        app.processOptions = *processOptions;

        if ( !app.capture )
        {
            cvReleaseCapture(&app.capture);
        }        

        // Do NOT release the image if it was a captured image.
        if ( app.lastImage && app.mode != ProcessCapture )
        {
            cvReleaseImage(&app.lastImage);
        }

        app.mode = ProcessStill;

        // This image won't be deleted until the next to call to a klu_processXXX function.
        // The calling application thereby gets the opportunity to query and display the image.
        app.lastImage = cvLoadImage( filepath, 1 );

        if ( app.grayscale )
        {
            cvReleaseImage(&app.grayscale);
        }

        // Prepare the grayscale image... speeds up processing function for capture images
        app.grayscale = cvCreateImage(cvSize(app.lastImage->width, app.lastImage->height), IPL_DEPTH_8U, 1);

        bool result = processImageFrame(app.lastImage, ffp);

        return result ? 1 : 0;
    }
    //------------------------------------------------------------------------------
    KLULIB_API int klu_processCaptureImage(KluProcessOptions * processOptions, 
        KluFaceFeaturePoints * ffp)
    {
        if ( !processOptions || !ffp || !app.capture )
        {
            return 0;
        }

        app.processOptions = *processOptions;

        // Release the last image if it was a still image
        if ( app.mode == ProcessStill && app.lastImage )
        {
            cvReleaseImage(&app.lastImage);
        }

        app.lastImage = cvQueryFrame(app.capture);

        if ( !app.lastImage )
        {
            app.mode = ProcessCapture;
            return 0;
        }

        if ( app.mode != ProcessCapture )
        {
            app.mode = ProcessCapture;

            if ( app.grayscale )
            {
                cvReleaseImage(&app.grayscale);
            }

            app.grayscale = cvCreateImage(cvSize(app.lastImage->width, app.lastImage->height), IPL_DEPTH_8U, 1);
        }

        bool result = processImageFrame(app.lastImage, ffp);
    
        return result ? 1 : 0;
    }
    //------------------------------------------------------------------------------
    KLULIB_API int klu_configureCaptureDialog(void)
    {
        if ( !app.capture )
        {
            return 0;
        }

//#ifdef WIN32
//        CvCaptureCAM_VFW * cap = (CvCaptureCAM_VFW*) app.capture;
//        HWND capHandle = cap->capWnd;
//        capDlgVideoSource(capHandle);
//#else
//        return 0;
//#endif

        return 1;
    }
    //------------------------------------------------------------------------------
    KLULIB_API int klu_configureCaptureResolutionDialog(void)
    {
        if ( !app.capture )
        {
            return 0;
        }

//#ifdef WIN32
//        CvCaptureCAM_VFW * cap = (CvCaptureCAM_VFW*) app.capture;
//        HWND capHandle = cap->capWnd;
//        capDlgVideoFormat(capHandle);
//        /*CAPDRIVERCAPS caps = {0};  
//        capDriverGetCaps(m_hCaptureWindow, &caps, sizeof(caps));*/
//#else
//        return 0;
//#endif

        return 1;
    }
    //------------------------------------------------------------------------------
} // end extern "C" {