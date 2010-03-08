/**
* Copyright (C) 2010, Konrad Kleine and Jens Lukowski, all rights reserved.
*
* $Id$
*/

#ifndef KLU_COMMON_H_
#define KLU_COMMON_H_

/**
* Project includes
*/
#include "klu_types.h"

/**
* C++ / STL includes
*/
#include <iostream>
#include <vector>

/**
* Decide how to handle inline functions.
*/
#ifdef WIN32
#	define KLU_INLINE inline
#else
// Assume GCC
#	define KLU_INLINE __inline__
#endif

/**
* \name predefined_colors Predefined colors
* @{
*/
#define COL_LAWN_GREEN 		CV_RGB(124, 252, 0)
#define COL_LIME_GREEN 		CV_RGB(50, 205, 50)
#define COL_LIME_GREEN 		CV_RGB(50, 205, 50)
#define COL_YELLOW 			CV_RGB(255, 255, 0)
#define COL_RED 			CV_RGB(255, 0, 0)
#define COL_GREEN 			CV_RGB(0, 255, 0)
#define COL_BLUE 			CV_RGB(0, 0, 255)
#define COL_BLACK 			CV_RGB(0, 0, 0)
#define COL_WHITE 			CV_RGB(255, 255, 255)
/**
* @}
*/

/**
* This namespace contains common convenience functions and data types.
*
* @author Konrad Kleine, Jens Lukowski
*/
namespace klu
{
    extern long g_autoSaveImages;
    extern bool g_enableVisDebug;

    enum ProcessMode { ProcessStill, ProcessCapture };
    /**
    * This struct encapsulates all program options and variables.
    *
    * @author Konrad Kleine, Jens Lukowski
    */
    struct ApplicationEnvironment
    {
        CvCapture * capture; // NULL if no capture is being used.
        bool isExiting;
        bool drawSearchRects;
        bool drawAnthropometricPoints;
        bool drawFfps;
        CvFont font;
        ProcessMode mode;
        IplImage * grayscale; 
        IplImage * lastImage;
        CvHaarClassifierCascade * cascadeFace;
        CvHaarClassifierCascade * cascadeMouth;
        CvHaarClassifierCascade * cascadeRightEye;
        CvHaarClassifierCascade * cascadeLeftEye;
        CvMemStorage * memStorage;
    };

    /**
    * Returns \c true if all library functionalities could be initialized;
    * otherwise \c false is returned.
    *
    * @author Konrad Kleine, Jens Lukowski
    */
    bool initializeLibrary(void);

    /**
    * Returns \c true if all library functionalities could be deinitialized;
    * otherwise \c false is returned.
    *
    * @author Konrad Kleine, Jens Lukowski
    */
    bool deinitializeLibrary(void);

    /**
    * The \c GrayStats structure contains statistical information of an
    * OpenCV \c IplImage.
    *
    * @see getGrayStats()
    * @author Konrad Kleine, Jens Lukowski
    */
    struct GrayStats
    {
        double avg;
        unsigned char min;
        unsigned char max;
        int pixelCount;
    };

    /**
    * Start the global timer.
    * This function is similar to MATLAB's tic() function.
    * @see toc()
    *
    * @author Konrad Kleine, Jens Lukowski
    */
    void tic();

    /**
    * Stops the global timer that was started with a previous call to \c tic()
    * and prints the result to \c stderr if \a doPrint is \c true.
    *
    * @author Konrad Kleine, Jens Lukowski
    */
    double toc(bool doPrint=false);

    /**
    * This function enables visual debugging by showing the image \a img in the
    * window named after \a wndName.
    *
    * If the window hasn't been created already, it will be created on-the-fly.
    *
    * If \c g_autoSaveImages is set to \c true, all windows will save the images
    * they are presenting.
    */
    KLU_INLINE void visDebug(const char * wndName, const IplImage * img);

    /**
    * Saves the \a image to "basename"_TIMESTAMP.png.
    *
    * @author Konrad Kleine, Jens Lukowski
    */
    void saveImage(const IplImage * image, const char * basename);

    /**
    * Detects objects in \a img that match the Haar classifier \a cascade and
    * have at least a minimal size of \a minSize. The found objects are returned
    * as a vector of OpenCV rectangles.
    *
    * @author Konrad Kleine, Jens Lukowski
    */
    std::vector<CvRect> detectObjects(IplImage * img, CvHaarClassifierCascade * cascade, CvMemStorage * storage, CvSize minSize = cvSize(200, 200));

    /**
    * Draws the rectangle \a r onto the image \a img using the given \a color.
    *
    * @author Konrad Kleine, Jens Lukowski
    */
    void drawRect(IplImage * img, const CvRect & r, CvScalar color = COL_RED);

    /**
    * Returns the center of the rectangle \a r in x/y coordinates.
    *
    * @author Konrad Kleine, Jens Lukowski
    */
    CvPoint getRectMidPoint(const CvRect & r);

    /**
    * Draws a cross of 4x4 pixel at point \a p onto the image \a img using the
    * given \a color.
    *
    * @author Konrad Kleine, Jens Lukowski
    */
    void drawCross(IplImage * img, const CvPoint & p, CvScalar color = COL_YELLOW);

    /**
    * Returns the distance (length of difference vector) from \a p1 to \a p2.
    *
    * @author Konrad Kleine, Jens Lukowski
    */
    double getDist(const CvPoint & p1, const CvPoint & p2);

    /**
    * Prints the OpenCV matrix \a A to \c stdout.
    * @see http://blog.weisu.org/2007/11/opencv-print-matrix.html
    */
    void printMat(const CvMat *A);

    /**
    * @TODO TBD
    */
    GrayStats getGrayStats(const IplImage * img);

    /**
    * Stretches the contrast of the grayscale image \a img by manipulating the
    * pixel values between \a srcMin and \a srcMax inclusively. The new value of
    * the pixels lies at the corresponding relative location between \a dstMin
    * and \a dstMax.
    *
    * Example:
    *
    *   Suppose you have the followin values.
    *
    *   srcMin = 100
    *   srcMax = 150
    *   dstMin = 125
    *   dstMin = 255
    *
    *   Pick one individual pixel value be p_in = 125.
    *   The resulting pixel value will be p_out = 190.
    *
    * @author Konrad Kleine, Jens Lukowski
    */
    void stretchContrast(IplImage * img,
        unsigned char srcMin, unsigned char srcMax,
        unsigned char dstMin, unsigned char dstMax);

    /**
    * Assumes that \a image has a ROI set and that you want to create a copy of
    * this ROI in grayscale.
    *
    * \internal
    */
    IplImage * extractGrayScaleROI(const IplImage * image);

    /**
    *	Detects the all the eye-feature points (see \c KluEyeFeaturePoints) in
    *	the \a image. You should set the ROI of for the \a image using the
    *	OpenCV function \c cvSetImageROI() to the eye region before calling
    *	this function.
    *  The \a eyeCenter is a 2D point of the previously detected center of
    *  the eye in global image coordinates. With the \a eyeCenter we can
    *  ensure that the correct eye contour is selected during the image processing.
    *  We ensure that the eyeCenter lies within the tested eye contour;
    *  otherwise we might falsely the eyebrow instead of the eye.
    *
    *	If \a window... is not \c NULL, some temporary images will be shown
    *	in this window. This is useful for visual debugging.
    *
    *	See: Abu Sayeed Md. Sohail and Prabir Bhattacharya
    *       "Detection of Facial Feature Points Using Anthropometric Face Model"
    *	     p. 660
    *
    * @author Konrad Kleine, Jens Lukowski
    */
    KluEyeFeaturePoints detectEyeFeaturePoints(const IplImage * image,
        const CvPoint & eyeCenter,
        CvMemStorage * storage,                                            
        const char * windowContrastStretch1 = NULL,
        const char * windowContrastStretch2 = NULL,
        const char * windowThreshold = NULL,
        const char * windowContour = NULL,
        const char * windowFeaturePoints = NULL);

    /**
    *	Detects the all the mouth-feature points (see \c KluMouthFeaturePoints) in
    *	the \a image. You should set the ROI of for the \a image using the
    *	OpenCV function \c cvSetImageROI() to the mouth region before calling
    *	this function.
    *
    *	If \a window... is not \c NULL, some temporary images will be shown
    *	in this window. This is useful for visual debugging.
    *
    *	See: Abu Sayeed Md. Sohail and Prabir Bhattacharya
    *       "Detection of Facial Feature Points Using Anthropometric Face Model"
    *	     p. 662
    *
    * @author Konrad Kleine, Jens Lukowski
    */
    KluMouthFeaturePoints detectMouthFeaturePoints(const IplImage * image,
        CvMemStorage * storage,
        const char * windowContrastStretch1,
        const char * windowContrastStretch2,
        const char * windowThreshold,
        const char * windowContour,
        const char * windowFeaturePoints);

    /**
    * This function draws all the Facial Feature Points (FFPs) stored in \a fpp
    * into the \a image.
    *
    * @author Konrad Kleine, Jens Lukowski
    */
    void drawFfps(IplImage * image, const KluFaceFeaturePoints & ffp);

    /**
    * This function processes an image frame. The image frame can come from a
    * still image or from a webcam/video stream. The function returns \c true
    * if everything went well; otherwise \c false is returned.
    * TODO: (Ko) Find a way to show the neat little debug windows.
    *
    * @author Konrad Kleine, Jens Lukowski
    */
    bool processImageFrame(IplImage * image,
        const KluProcessOptions * options, 
        KluFaceFeaturePoints * ffp);
}

extern klu::ApplicationEnvironment app;

#endif