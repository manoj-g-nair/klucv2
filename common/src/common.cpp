/**
 * Copyright (C) 2010, Konrad Kleine and Jens Lukowski, all rights reserved.
 *
 * $Id$
 */

#include "common.hpp"

using namespace std;
using namespace cv;

namespace klu
{
//------------------------------------------------------------------------------
long g_autoSaveImages = 0;
bool g_enableVisDebug = false;
//------------------------------------------------------------------------------
static double ticStart = 0;
void tic()
{
    ticStart = (double)cvGetTickCount();
}
//------------------------------------------------------------------------------
double toc(bool doPrint)
{
    ticStart = ((double)cvGetTickCount() - ticStart) / ( (double) cvGetTickFrequency() * 1000.0);
	if ( doPrint )
	{
		// Print to unbuffered standard error for immediate output
		fprintf(stderr, "processing time = %gms\n", ticStart);
	}
    return ticStart;
}
//------------------------------------------------------------------------------
KLU_INLINE void visDebug(const char * wndName, const IplImage * img)
{
	if ( wndName && img )
	{
        if (g_enableVisDebug)
        {
		    cvNamedWindow(wndName, 0);
		    cvShowImage(wndName, img);
        }
        else
        {
            cvDestroyWindow(wndName);
        }

        if (g_autoSaveImages)
        {
            saveImage(img, wndName);
        }
	}
}
//------------------------------------------------------------------------------
// Saves the "image" to "basename"_TIMESTAMP.png
void saveImage(const IplImage * image, const char * basename)
{
	if ( !image || !basename )
	{
		return;
	}
	
	char filename[255];
	memset(filename, '\0', 255);	
	sprintf(filename, "%s %ld.png", basename, g_autoSaveImages);
	cout << "Saving image " << filename << endl;	
	cvSaveImage(filename, image);
}
//------------------------------------------------------------------------------
// Function to detect and draw any faces that is present in an image
vector<CvRect> detectObjects( IplImage * img, CvHaarClassifierCascade * cascade, CvMemStorage * storage, CvSize minSize)
{
	vector<CvRect> objRects = vector<CvRect>();

    // Find whether the cascade is loaded, to find the faces. If yes, then:
    if( !cascade || !storage )
    {
    	return objRects;
    }

	int scale = 1;

    // Create a new image based on the input image
    IplImage* temp = cvCreateImage( cvSize(img->width/scale,img->height/scale), 8, 3 );

    // Clear the memory storage which was used before
    cvClearMemStorage( storage );

	// There can be more than one face in an image. So create a growable sequence of faces.
	// Detect the objects and store them in the sequence
	CvSeq* objects = cvHaarDetectObjects( img, cascade, storage,
										1.1, 2, CV_HAAR_FIND_BIGGEST_OBJECT,
										minSize ); // originally: 40x40

	// Loop the number of faces found.
	for (int i=0; i<(objects ? (objects->total>2?2:objects->total) : 0); i++)
	{
	   // Create a new rectangle for drawing the face
		CvRect * r = (CvRect*)cvGetSeqElem( objects, i );
		objRects.push_back(*r);
	}

    // Release the temp image created.
    cvReleaseImage( &temp );

    return objRects;
}
//------------------------------------------------------------------------------
void drawRect(IplImage * img, const CvRect & r, CvScalar color)
{
	CvPoint pt1 = cvPoint(r.x, r.y);
	CvPoint pt2 = cvPoint(r.x + r.width, r.y + r.height);

	// Draw the rectangle in the input image
	cvRectangle( img, pt1, pt2, color, 1 );
}
//------------------------------------------------------------------------------
CvPoint getRectMidPoint(const CvRect & r)
{
	return cvPoint(r.x + r.width/2, r.y + r.height/2);;
}
//------------------------------------------------------------------------------
void drawCross(IplImage * img, const CvPoint & p, CvScalar color)
{
	cvLine(img, p, cvPoint(p.x,     p.y + 4), color, 1);
	cvLine(img, p, cvPoint(p.x,     p.y - 4), color, 1);
	cvLine(img, p, cvPoint(p.x - 4, p.y),     color, 1);
	cvLine(img, p, cvPoint(p.x + 4, p.y),     color, 1);
}
//------------------------------------------------------------------------------
double getDist(const CvPoint & p1, const CvPoint & p2)
{
	CvPoint diff = cvPoint(p2.x-p1.x, p2.y-p1.y);
	return sqrt((double)diff.x * diff.x + diff.y * diff.y);
}
//------------------------------------------------------------------------------
void printMat(const CvMat *A)
{
	if ( !A )
	{
		return;
	}

	int i, j;
	for (i = 0; i < A->rows; i++) {
		printf("\n");
		switch (CV_MAT_DEPTH(A->type)) {
		case CV_32F:
		case CV_64F:
			for (j = 0; j < A->cols; j++)
				printf("%8.3f ", (float) cvGetReal2D(A, i, j));
			break;
		case CV_8U:
		case CV_16U:
			for (j = 0; j < A->cols; j++)
				printf("%6d", (int) cvGetReal2D(A, i, j));
			break;
		default:
			break;
		}
	}
	printf("\n");
}
//------------------------------------------------------------------------------
GrayStats getGrayStats(const IplImage * img)
{
	CV_Assert(img && img->nChannels == 1);

	// Initialize with reasonable defaults
	GrayStats d;
	d.avg = 0.0;
	d.min = 255;
	d.max = 0;
	d.pixelCount = 0;

	// The pixel value
	unsigned char p;

	// Find the average and min/max values
	for ( int r=0; r < img->height; r++ )
	{
		for ( int c=0; c < img->width; c++)
		{
			p = *((unsigned char*) img->imageData +
					            r * img->widthStep +
					            c * img->nChannels);

			d.avg += p;
			d.pixelCount++;

			if (d.max < p)
			{
				d.max = p;
			}
			else if (d.min > p)
			{
				d.min = p;
			}
		}
	}

	if ( img->width > 0 && img->height > 0 )
	{
		d.avg = d.avg / (double) d.pixelCount;
	}

	return d;
}
//------------------------------------------------------------------------------
GrayStats getGrayStats(const IplImage * img,
					   unsigned char grayRegionStart,
					   unsigned char grayRegionEnd)
{
	CV_Assert( img && img->nChannels == 1);

	// Initialize with reasonable defaults
	GrayStats d;
	d.avg = 0.0;
	d.min = 255;
	d.max = 0;
	d.pixelCount = 0;

	// The pixel value
	unsigned char p;

	// Find the average and min/max values
	for ( int r=0; r < img->height; r++ )
	{
		for ( int c=0; c < img->width; c++)
		{
			p = *((unsigned char*) img->imageData +
									r * img->widthStep +
									c * img->nChannels);

			if ( p >= grayRegionStart && p <= grayRegionEnd )
			{
				d.avg += p;
				d.pixelCount++;

				if (d.max < p)
				{
					d.max = p;
				}
				else if (d.min > p)
				{
					d.min = p;
				}
			}
		}
	}

	if ( img->width > 0 && img->height > 0 )
	{
		d.avg = d.avg / (double) d.pixelCount;
	}

	return d;
}
//------------------------------------------------------------------------------
double getAvgMinMaxGrayValue(const IplImage * img,  unsigned char * minVal, unsigned char * maxVal)
{
	CV_Assert( img && img->nChannels == 1);

	int i = 0;

	double avg = 0.0;

	// Initialize min/max with reasonable defaults
	if ( minVal )
	{
		*minVal = 255;
	}

	if ( maxVal )
	{
		*maxVal = 0;
	}

	// Find the average and min/max values
	for ( int r=0; r < img->height; r++ )
	{
		for ( int c=0; c < img->width; c++)
		{
			unsigned char * p = (unsigned char*) img->imageData +
					r * img->widthStep +
					c * img->nChannels;

			avg += *p;
			i++;

			if (maxVal && *maxVal < *p)
			{
				*maxVal = *p;
			}

			if (minVal && *minVal > *p)
			{
				*minVal = *p;
			}
		}
	}

	if ( i > 0 )
	{
		avg = avg / (double) i;
	}

	return avg;
}
//------------------------------------------------------------------------------
double getAvgMinMaxGrayValue(	const IplImage * img,
								unsigned char grayRegionStart,
								unsigned char grayRegionEnd,
								unsigned char * minVal,
								unsigned char * maxVal)
{
	CV_Assert( img && img->nChannels == 1);

	int i = 0;

	double avg = 0.0;

	// Initialize min/max with reasonable defaults
	if ( minVal )
	{
		*minVal = 255;
	}

	if ( maxVal )
	{
		*maxVal = 0;
	}

	// Find the average and min/max values
	for ( int r=0; r < img->height; r++ )
	{
		for ( int c=0; c < img->width; c++)
		{
			unsigned char * p = (unsigned char*) img->imageData +
					r * img->widthStep +
					c * img->nChannels;

			if ( *p >= grayRegionStart && *p <= grayRegionEnd )
			{
				avg += *p;
				i++;

				if (maxVal && *maxVal < *p)
				{
					*maxVal = *p;
				}

				if (minVal && *minVal > *p)
				{
					*minVal = *p;
				}
			}
		}
	}

	if ( i > 0 )
	{
		avg = avg / (double) i;
	}

	return avg;
}
//------------------------------------------------------------------------------
void stretchContrast(IplImage * img,
					 unsigned char srcMin, unsigned char srcMax,
					 unsigned char dstMin, unsigned char dstMax)
{
	CV_Assert( img && img->nChannels == 1 && srcMin <= srcMax && dstMin <= dstMax );

	for ( int r=0; r < img->height; r++ )
	{
		for ( int c=0; c < img->width; c++)
		{
			unsigned char * p = (unsigned char*) img->imageData +
					r * img->widthStep +
					c * img->nChannels;
			
			if ( *p >= srcMin && *p <= srcMax )
			{
				// x = [1;0]
				double x = (double) (*p - srcMin) / (double) (srcMax - srcMin);
				*p = dstMin + x * (dstMax - dstMin);
			}	
		}
	}
}
//------------------------------------------------------------------------------
IplImage * extractGrayScaleROI(const IplImage * image)
{
	CV_Assert(image != NULL);

	// Create a copy of the eye region image.
	IplImage * regImg = cvCloneImage(image);

	// Ensure, the region image is 8 bit grayscale
	if ( regImg && regImg->depth != IPL_DEPTH_8U && regImg->nChannels != 1 )
	{
		// @TODO ensure the source format is RGB (at the moment this is just heuristic)
		cvCvtColor(regImg, regImg, CV_RGB2GRAY);
	}
	return regImg;
}
//------------------------------------------------------------------------------
EyeFeaturePoints detectEyeFeaturePoints(const IplImage * image,
                                        CvMemStorage * storage,
										const char * windowContrastStretch1,
										const char * windowContrastStretch2,
										const char * windowThreshold,
										const char * windowContour,
										const char * windowFeaturePoints)
{
	CV_Assert(image != NULL && storage != NULL);

	EyeFeaturePoints fp;

	// Create a copy of the eye region image
	IplImage * regImg = extractGrayScaleROI(image);

	// Get some statistical information about the grayscale distribution
	GrayStats stats = getGrayStats(regImg);

	// Calculate the upper and lower gray value bounds
	unsigned char upperBound = stats.avg + (stats.max - stats.avg) / 2;
	unsigned char lowerBound = stats.min + (stats.avg - stats.min) / 2;

	// Print the stats
	static bool hasPrintedStats = false;
	if ( !hasPrintedStats )
	{
		cerr << "avg        = " << stats.avg << endl;
		cerr << "minVal     = " << (int) stats.min << endl;
		cerr << "maxVal     = " << (int) stats.max << endl;
		cerr << "upperBound = " << (int) upperBound << endl;
		cerr << "lowerBound = " << (int) lowerBound << endl;
		cerr << "pixelCount = " << stats.pixelCount;
		hasPrintedStats = true;
	}

	// Saturation (this is not very well documented in
	stretchContrast(regImg, lowerBound, stats.max, upperBound, 255);
	visDebug(windowContrastStretch1, regImg);

	stretchContrast(regImg, stats.min, stats.avg, 0, stats.min);
	visDebug(windowContrastStretch2, regImg);

	// Threshold iteration
	unsigned char t;
	unsigned char tNew = stats.avg;
	do {
		t = tNew;
		GrayStats statsM1 = getGrayStats(regImg, 0, t);
		GrayStats statsM2 = getGrayStats(regImg, t+1, 255);
		tNew = (statsM1.avg + statsM2.avg) / 2;
	} while (t != tNew );
    cvThreshold(regImg, regImg, t, 255, CV_THRESH_BINARY_INV);
	visDebug(windowThreshold, regImg);
    

    // Find contours in inverted binary image
    CvSeq * contours = NULL;
	CvSeq * firstContour = NULL;
	int nContours = cvFindContours(regImg, storage, &firstContour, sizeof(CvContour), CV_RETR_EXTERNAL, CV_CHAIN_APPROX_NONE);

	CvSeq * biggestContour = NULL;
	double biggestArea = 0;

	for( CvSeq* c = firstContour; c!=NULL; c=c->h_next )
	{
		double area = cvContourArea(c);

		if ( (area*area) > biggestArea )
		{
			biggestContour = c;
			biggestArea = (area*area);            
		}
	}

	if ( biggestContour )
	{
        if ( windowContour )
        {
		    cvDrawContours(
			    regImg, // target image
			    biggestContour, // contour
			    CV_RGB(255, 255, 255),		// external color
			    CV_RGB(255, 0, 0),	// hole color
			    1,			// Vary max_level and compare results
			    1, // thickness
			    8 // type
		    );
            visDebug(windowContour, regImg);
        }

		// Go through all contour points and find the right- and leftmost contour point.
        // Initialize with reasonable defaults that are very likely to be overwritten.
        fp.cornerRight = cvPoint(0, 0);
        fp.cornerLeft = cvPoint(regImg->width, regImg->height);

		int nElements = biggestContour->total;

		for ( int i = 0; i < nElements; i++)
		{
			CvPoint p = *((CvPoint*) cvGetSeqElem(biggestContour, i));

			if ( p.x > fp.cornerRight.x )
			{
				fp.cornerRight = p;
			}
			else if ( p.x < fp.cornerLeft.x )
			{
				fp.cornerLeft = p;
			}
		}

		// Go through all contour points and find the right- and leftmost contour point.

        // The upper and lower lid feature points must lie in the middle third of the
        // range from the left to the right corner.
        int minX = fp.cornerLeft.x + (fp.cornerRight.x - fp.cornerLeft.x) / 3;
        int maxX = fp.cornerLeft.x + 2 * (fp.cornerRight.x - fp.cornerLeft.x) / 3;

        // Initialize with reasonable defaults that are very likely to be overwritten.
        fp.upperLid = cvPoint(regImg->width, regImg->height);
        fp.lowerLid = cvPoint(0,0);

		for ( int i = 0; i < nElements; i++)
		{
			CvPoint p = *((CvPoint*) cvGetSeqElem(biggestContour, i));

			if ( p.x > minX && p.x < maxX && p.y < fp.upperLid.y )
			{
				fp.upperLid = p;
			}
            else if ( p.x > minX && p.x < maxX && p.y > fp.lowerLid.y )
			{
				fp.lowerLid = p;
			}	
        }


        drawCross(regImg, fp.cornerRight, COL_WHITE);
        drawCross(regImg, fp.cornerLeft, COL_WHITE);
        drawCross(regImg, fp.upperLid, COL_WHITE);
        drawCross(regImg, fp.lowerLid, COL_WHITE);
        visDebug(windowFeaturePoints, regImg);
	}

	cvReleaseImage(&regImg);

    // Fix coordinates: ROI to global image coordinates
    CvRect region = cvGetImageROI(image);
    fp.center = getRectMidPoint(region);
    fp.lowerLid.x += region.x;
    fp.cornerLeft.x += region.x;
    fp.cornerRight.x += region.x;
    fp.upperLid.x += region.x;
    fp.lowerLid.y += region.y;
    fp.cornerLeft.y += region.y;
    fp.cornerRight.y += region.y;
    fp.upperLid.y += region.y;

	return fp;
}
//------------------------------------------------------------------------------
MouthFeaturePoints detectMouthFeaturePoints(const IplImage * image,
											CvMemStorage * storage,
											const char * windowContrastStretch1,
											const char * windowContrastStretch2,
											const char * windowThreshold,
											const char * windowContour,
											const char * windowFeaturePoints)
{
	CV_Assert(image != NULL && storage != NULL);

	MouthFeaturePoints fp;

	// Create a copy of the eye region image
	IplImage * regImg = extractGrayScaleROI(image);

	// Get some statistical information about the grayscale distribution
	GrayStats stats = getGrayStats(regImg);

	// Calculate the upper and lower gray value bounds
	unsigned char upperBound = stats.avg + (stats.max - stats.avg) / 2;
	unsigned char lowerBound = stats.min + (stats.avg - stats.min) / 2;

	// Print the stats
	static bool hasPrintedStats = false;
	if ( !hasPrintedStats )
	{
		cerr << "avg        = " << stats.avg << endl;
		cerr << "minVal     = " << (int) stats.min << endl;
		cerr << "maxVal     = " << (int) stats.max << endl;
		cerr << "upperBound = " << (int) upperBound << endl;
		cerr << "lowerBound = " << (int) lowerBound << endl;
		cerr << "pixelCount = " << stats.pixelCount;
		hasPrintedStats = true;
	}

	// Saturation (this is not very well documented in
	stretchContrast(regImg, lowerBound, stats.max, upperBound, 255);
	visDebug(windowContrastStretch1, regImg);

	stretchContrast(regImg, stats.min, stats.avg, 0, stats.min);
	visDebug(windowContrastStretch2, regImg);

	// Threshold iteration
	unsigned char t;
	unsigned char tNew = stats.avg;
	do {
		t = tNew;
		GrayStats statsM1 = getGrayStats(regImg, 0, t);
		GrayStats statsM2 = getGrayStats(regImg, t+1, 255);
		tNew = (statsM1.avg + statsM2.avg) / 2;
	} while (t != tNew );
    cvThreshold(regImg, regImg, t, 255, CV_THRESH_BINARY_INV);
	visDebug(windowThreshold, regImg);
    

    // Find contours in inverted binary image
    CvSeq * contours = NULL;
	CvSeq * firstContour = NULL;
	int nContours = cvFindContours(regImg, storage, &firstContour, sizeof(CvContour), CV_RETR_EXTERNAL, CV_CHAIN_APPROX_NONE);

	CvSeq * biggestContour = NULL;
	double biggestArea = 0;

	for( CvSeq* c = firstContour; c!=NULL; c=c->h_next )
	{
		double area = cvContourArea(c);

		if ( (area*area) > biggestArea )
		{
			biggestContour = c;
			biggestArea = (area*area);            
		}
	}

	if ( biggestContour )
	{
        if ( windowContour )
        {
		    cvDrawContours(
			    regImg, // target image
			    biggestContour, // contour
			    CV_RGB(255, 255, 255),		// external color
			    CV_RGB(255, 0, 0),	// hole color
			    1,			// Vary max_level and compare results
			    1, // thickness
			    8 // type
		    );
            visDebug(windowContour, regImg);
        }

		// Go through all contour points and find the right- and leftmost contour point.
        // Initialize with reasonable defaults that are very likely to be overwritten.
        fp.cornerRight = cvPoint(0, 0);
        fp.cornerLeft = cvPoint(regImg->width, regImg->height);

		int nElements = biggestContour->total;

		for ( int i = 0; i < nElements; i++)
		{
			CvPoint p = *((CvPoint*) cvGetSeqElem(biggestContour, i));

			if ( p.x > fp.cornerRight.x )
			{
				fp.cornerRight = p;
			}
			else if ( p.x < fp.cornerLeft.x )
			{
				fp.cornerLeft = p;
			}
		}

		// Go through all contour points and find the right- and leftmost contour point.

        // The upper and lower lid feature points must lie in the middle third of the
        // range from the left to the right corner.
        int minX = fp.cornerLeft.x + (fp.cornerRight.x - fp.cornerLeft.x) / 3;
        int maxX = fp.cornerLeft.x + 2 * (fp.cornerRight.x - fp.cornerLeft.x) / 3;

        // Initialize with reasonable defaults that are very likely to be overwritten.
        fp.upperLipMiddle = cvPoint(regImg->width, regImg->height);
        fp.lowerLipMiddle = cvPoint(0,0);
        fp.upperLipRight = cvPoint(regImg->width, regImg->height);
        fp.lowerLipRight = cvPoint(0,0);
        fp.upperLipLeft = cvPoint(regImg->width, regImg->height);
        fp.lowerLipLeft = cvPoint(0,0);

		for ( int i = 0; i < nElements; i++)
		{
			CvPoint p = *((CvPoint*) cvGetSeqElem(biggestContour, i));

            // TODO: Flip "<" and ">"

            // Middle Lip
			if ( p.x > minX && p.x < maxX && p.y < fp.upperLipMiddle.y )
			{
                fp.upperLipMiddle = p;
			}
            else if ( p.x > minX && p.x < maxX && p.y > fp.lowerLipMiddle.y )
			{
				fp.lowerLipMiddle = p;
			}

            // Right Lip
            if ( p.x > maxX && p.y < fp.upperLipRight.y )
			{
                fp.upperLipRight = p;
			}
            else if ( p.x > maxX && p.y > fp.lowerLipRight.y )
			{
				fp.lowerLipRight = p;
			}	

            // Left Lip
			if ( p.x < minX && p.y < fp.upperLipLeft.y )
			{
                fp.upperLipLeft = p;
			}
            else if ( p.x < minX && p.y > fp.lowerLipLeft.y )
			{
				fp.lowerLipLeft = p;
			}	
        }

        drawCross(regImg, fp.cornerRight, COL_WHITE);
        drawCross(regImg, fp.cornerLeft, COL_WHITE);
        drawCross(regImg, fp.upperLipMiddle, COL_WHITE);
        drawCross(regImg, fp.lowerLipMiddle, COL_WHITE);
        drawCross(regImg, fp.upperLipRight, COL_WHITE);
        drawCross(regImg, fp.lowerLipRight, COL_WHITE);
        drawCross(regImg, fp.upperLipLeft, COL_WHITE);
        drawCross(regImg, fp.lowerLipLeft, COL_WHITE);
        visDebug(windowFeaturePoints, regImg);
	}

	cvReleaseImage(&regImg);

    // Fix coordinates: ROI to global image coordinates
    CvRect region = cvGetImageROI(image);
    fp.lowerLipMiddle.x += region.x;
    fp.cornerLeft.x += region.x;
    fp.cornerRight.x += region.x;
    fp.upperLipMiddle.x += region.x;
    fp.lowerLipLeft.x += region.x;
    fp.lowerLipRight.x += region.x;
    fp.upperLipRight.x += region.x;
    fp.upperLipLeft.x += region.x;
    fp.lowerLipMiddle.y += region.y;
    fp.cornerLeft.y += region.y;
    fp.cornerRight.y += region.y;
    fp.upperLipMiddle.y += region.y;
    fp.lowerLipLeft.y += region.y;
    fp.lowerLipRight.y += region.y;
    fp.upperLipLeft.y += region.y;
    fp.upperLipRight.y += region.y;

	return fp;
}
//------------------------------------------------------------------------------
void drawFfps(IplImage * image, const FaceFeaturePoints & ffp)
{
    // Draw mouth feature points
    //drawCross(image, ffp.mouth.cornerLeft, COL_YELLOW);
    //drawCross(image, ffp.mouth.cornerRight, COL_YELLOW);
    //drawCross(image, ffp.mouth.upperLipMiddle, COL_YELLOW);
    //drawCross(image, ffp.mouth.lowerLipMiddle, COL_YELLOW);
    //drawCross(image, ffp.mouth.upperLipLeft, COL_YELLOW);
    //drawCross(image, ffp.mouth.lowerLipLeft, COL_YELLOW);
    //drawCross(image, ffp.mouth.upperLipRight, COL_YELLOW);
    //drawCross(image, ffp.mouth.lowerLipRight, COL_YELLOW);
    // Lips
    cvLine(image, ffp.mouth.cornerLeft, ffp.mouth.upperLipLeft, COL_YELLOW);
    cvLine(image, ffp.mouth.upperLipLeft, ffp.mouth.upperLipMiddle, COL_YELLOW);
    cvLine(image, ffp.mouth.upperLipLeft, ffp.mouth.upperLipMiddle, COL_YELLOW);
    cvLine(image, ffp.mouth.upperLipMiddle, ffp.mouth.upperLipRight, COL_YELLOW);
    cvLine(image, ffp.mouth.upperLipRight, ffp.mouth.cornerRight, COL_YELLOW);
    cvLine(image, ffp.mouth.cornerRight, ffp.mouth.lowerLipRight, COL_YELLOW);
    cvLine(image, ffp.mouth.lowerLipRight, ffp.mouth.lowerLipMiddle, COL_YELLOW);
    cvLine(image, ffp.mouth.lowerLipMiddle, ffp.mouth.lowerLipLeft, COL_YELLOW);
    cvLine(image, ffp.mouth.lowerLipLeft, ffp.mouth.cornerLeft, COL_YELLOW);       

    // Draw eye feature points
    // Centers
    drawCross(image, ffp.leftEye.center, COL_YELLOW);
    drawCross(image, ffp.rightEye.center, COL_YELLOW);
    // Lids
    drawCross(image, ffp.rightEye.cornerLeft, COL_YELLOW);
    drawCross(image, ffp.rightEye.cornerRight, COL_YELLOW);
    drawCross(image, ffp.rightEye.upperLid, COL_YELLOW);
    drawCross(image, ffp.rightEye.lowerLid, COL_YELLOW);
    drawCross(image, ffp.leftEye.cornerLeft, COL_YELLOW);
    drawCross(image, ffp.leftEye.cornerRight, COL_YELLOW);
    drawCross(image, ffp.leftEye.upperLid, COL_YELLOW);
    drawCross(image, ffp.leftEye.lowerLid, COL_YELLOW);
}
//------------------------------------------------------------------------------
}
