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
} // end extern "C" {