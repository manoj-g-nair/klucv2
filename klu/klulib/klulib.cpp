// klulib.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "klulib.h"
#include "klu_common.hpp"

using namespace klu;

extern "C" {    
    static IplImage * g_lastImage = NULL;
    //------------------------------------------------------------------------------
    klu::ApplicationEnvironment app;
    //------------------------------------------------------------------------------
    KLULIB_API int createCapture(void)
    {
        if ( !app.capture )
        {
            app.capture = cvCreateCameraCapture(CV_CAP_ANY);

            if ( app.capture )
            {        
                // Manipulate properties of the camera.
                cvSetCaptureProperty(app.capture, CV_CAP_PROP_FRAME_WIDTH, double(320));
                cvSetCaptureProperty(app.capture, CV_CAP_PROP_FRAME_HEIGHT, double(240));
                int camWidth = cvGetCaptureProperty(app.capture, CV_CAP_PROP_FRAME_WIDTH);
                int camHeight = cvGetCaptureProperty(app.capture, CV_CAP_PROP_FRAME_HEIGHT);

                if ( camWidth != 320 || camHeight != 240 )
                {
                    freeCapture();
                }
            }
        }

        return app.capture ? 1 : 0;
    }
    //------------------------------------------------------------------------------
    KLULIB_API void freeCapture(void)
    {
        if ( app.capture )
        {
            cvReleaseCapture(&app.capture);
            app.capture = NULL; // This is probably not needed. Anyway, be safe.
        }
    }
    //------------------------------------------------------------------------------
    KLULIB_API void queryCaptureImage(unsigned char ** data, 
        int * width, 
        int * height, 
        int * nChannels, 
        int * widthStep)
    {
        IplImage * image = cvQueryFrame(app.capture);
        *data = reinterpret_cast<unsigned char*>(image->imageData);
        *width = image->width;
        *height = image->height;
        *nChannels = image->nChannels;
        *widthStep = image->widthStep;
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
    KLULIB_API int klu_createAndSaveAnn(int * numNeuronsPerLayer, 
        int numLayers, 
        int activationFunction, 
        const char * filepath)
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
    KLULIB_API int klu_processStillImage(const char * filepath,
        KluProcessOptions * processOptions, 
        KluFaceFeaturePoints * ffp)
    {
        if ( !filepath || !processOptions || !ffp )
        {
            return 0;
        }

        if ( !app.capture )
        {
            cvReleaseCapture(&app.capture);
        }

        app.mode = ProcessStill;

        // Do NOT release the image if it wasn't a captured image.
        if ( app.lastImage && app.mode != ProcessCapture )
        {
            cvReleaseImage(&app.lastImage);
        }

        // This image won't be deleted until the next to call to a klu_processXXX function.
        // The calling application thereby gets the opportunity to query and display the image.
        app.lastImage = cvLoadImage( filepath, 1 );

        if ( app.grayscale )
        {
            cvReleaseImage(&app.grayscale);
        }

        // Prepare the grayscale image... speeds up processing function for capture images
        app.grayscale = cvCreateImage(cvSize(app.lastImage->width, app.lastImage->height), IPL_DEPTH_8U, 1);

        bool result = processImageFrame(app.lastImage, processOptions, ffp);

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

        bool result = processImageFrame(app.lastImage, processOptions, ffp);
    
        return result ? 1 : 0;
    }
    //------------------------------------------------------------------------------
} // end extern "C" {