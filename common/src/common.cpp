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
		cvNamedWindow(wndName, 0);
		cvShowImage(wndName, img);
	}
}
//------------------------------------------------------------------------------
// Saves the "image" to "basename"_TIMESTAMP.png
void saveImage(IplImage * image, const char * basename)
{
	if ( !image || !basename )
	{
		return;
	}
	
	char filename[255];
	memset(filename, '\0', 255);	
	sprintf(filename, "%s %ld.png", basename, time(NULL));
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
	cvThreshold(regImg, regImg, t, 255, CV_THRESH_BINARY);
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
			    CV_RGB(255, 255, 0),		// external color
			    CV_RGB(0, 0, 255),	// hole color
			    1,			// Vary max_level and compare results
			    2, // thickness
			    8 // type
		    );
            visDebug(windowContour, regImg);
        }


		// Go through all contour points and find the right- and leftmost

		// @TODO also save the seq IDX for further contour processing and search for upper and lower lid.

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
			//fprintf(stderr, "%d %p\n", biggestContour->total, (CvPoint*) cvGetSeqElem(biggestContour, 0));
		}

		// Draw right- and leftmost contour points
//			cvDrawLine(app.frame, rightmost, rightmost, CV_RGB(0, 255, 0), 10);
//			cvDrawLine(app.frame, leftmost, leftmost, CV_RGB(0, 255, 0), 10);

	}

	cvReleaseImage(&regImg);

	return fp;
}
//------------------------------------------------------------------------------
}
