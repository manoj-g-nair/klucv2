/**
 * Copyright (C) 2010, Konrad Kleine and Jens Lukowski, all rights reserved.
 *
 * $Id$
 */

#ifndef COMMON_HPP
#define COMMON_HPP

//#define CV_NO_BACKWARD_COMPATIBILITY
//#define HIGHGUI_NO_BACKWARD_COMPATIBILITY

/**
 * OpenCV includes
 */
#include <cv.h>
#include <highgui.h>
#include <ml.h>

/**
 * C includes
 */
#include <cstdio>
#include <cstdlib>
#include <cstring>
#include <cassert>
#include <cmath>

#include <float.h>
#include <limits.h>
#include <time.h>
#include <ctype.h>

/**
 * C++ / STL includes
 */
#include <iostream>
#include <vector>

/**
 * Decide how to handle inline functions.
 */
#ifdef WIN32
#	define KLU_INLINE
#else
	// Assume GCC
#	define KLU_INLINE __inline__
#endif

/**
 * \name Predefined colors
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
 */
namespace klu
{
	/**
	 * \name Datatypes for Feature Points.
	 * @{
	 */

	/**
	 * The \c EyeFeaturePoints struct contains the X-Y coordinates of
	 * all the feature points for one eye. Coordinates are considered
	 * to be image and not region coordinates.
	 *
	 * @author Konrad Kleine, Jens Lukowski
	 */
	struct EyeFeaturePoints
	{
		cv::Point2i upperLid;
		cv::Point2i bottomLid;
		cv::Point2i cornerLeft; // from point of view
		cv::Point2i cornerRight;// from point of view
	};

	/**
	 * @}
	 */

	/**
	 * The \c GrayStats structure contains statistical information of an
	 * OpenCV \c IplImage.
	 *
	 * @see getGrayStats()
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
	 */
	KLU_INLINE void visDebug(const char * wndName, const IplImage * img);

	/**
     * Saves the \a image to "basename"_TIMESTAMP.png.
     *
     * @author Konrad Kleine, Jens Lukowski
     */
	void saveImage(IplImage * image, const char * basename);

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
	 * Returns the calculate average gray value of the image \a img.
	 * If \a minVal and \a maxVal are not \c NULL, the minimal and maximum
	 * gray-value will be stored in them.
	 * \note cv::mean didn't work out as expected.
	 *
	 * @author Konrad Kleine, Jens Lukowski
	 */
	double getAvgMinMaxGrayValue(const IplImage * img,
			                     unsigned char * minVal=NULL,
			                     unsigned char * maxVal=NULL);

	/**
	 * @see getAvgMinMaxGrayValue(const IplImage *, unsigned char *, unsigned char *)
	 *
	 * @author Konrad Kleine, Jens Lukowski
	 */
	double getAvgMinMaxGrayValue(const IplImage * img,
								 unsigned char grayRegionStart,
								 unsigned char grayRregionEnd,
								 unsigned char * minVal=NULL,
								 unsigned char * maxVal=NULL);

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
	 *	Detects the all the eye-feature points (see \c EyeFeaturePoints) in
	 *	the \a image. You should set the ROI of for the \a image using the
	 *	OpenCV function \c cvSetImageROI() to the eye region before calling
	 *	this function.
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
	EyeFeaturePoints detectEyeFeaturePoints(const IplImage * image,
                                            CvMemStorage * storage,
											const char * windowContrastStretch1 = NULL,
											const char * windowContrastStretch2 = NULL,
											const char * windowThreshold = NULL,
											const char * windowContour = NULL,
											const char * windowFeaturePoints = NULL);

}

#endif /* #ifndef COMMON_HPP */

