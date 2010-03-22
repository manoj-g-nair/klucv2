/**
 * Copyright (C) 2010, Konrad Kleine and Jens Lukowski, all rights reserved.
 */

#include "stdafx.h"
#include "klu_common.hpp"

using namespace std;
using namespace cv;

namespace klu
{
    //------------------------------------------------------------------------------
    long g_autoSaveImages = 0;
    //------------------------------------------------------------------------------
    bool initializeLibrary(void)
    {
        cvInitFont(&(app.font), CV_FONT_HERSHEY_SIMPLEX, 0.5, 0.5, 0, 1, CV_AA);
        app.grayscale = NULL;	
        app.lastImage = NULL;
        app.mode = ProcessStill;
        // Initialize old OpenCV 1.x stuff for object detection
        app.cascadeFace = (CvHaarClassifierCascade*) cvLoad("../../../../data/haarcascades/haarcascade_frontalface_alt.xml", 0, 0, 0 );
        app.cascadeLeftEye = (CvHaarClassifierCascade*) cvLoad("../../../../data/haarcascades/haarcascade_mcs_lefteye.xml", 0, 0, 0 );
        app.cascadeRightEye = (CvHaarClassifierCascade*) cvLoad("../../../../data/haarcascades/haarcascade_mcs_righteye.xml", 0, 0, 0 );
        app.cascadeMouth = (CvHaarClassifierCascade*) cvLoad("../../../../data/haarcascades/haarcascade_mcs_mouth.xml", 0, 0, 0 );
        app.memStorage = cvCreateMemStorage(0); 
        app.kernel1 = cvCreateStructuringElementEx(3, 3, 1, 1, CV_SHAPE_CROSS, NULL);

        if ( app.kernel1 && app.cascadeFace && app.cascadeLeftEye && app.cascadeRightEye && app.cascadeMouth && app.memStorage )
        {        
            return true;
        }
        else
        {
            return false;
        }
    }
    //------------------------------------------------------------------------------
    bool deinitializeLibrary(void)
    {
        if ( app.cascadeFace )
        {
            cvReleaseHaarClassifierCascade(&app.cascadeFace);
        }

        if ( app.cascadeMouth )
        {
            cvReleaseHaarClassifierCascade(&app.cascadeMouth);
        }

        if ( app.cascadeLeftEye )
        {
            cvReleaseHaarClassifierCascade(&app.cascadeLeftEye);
        }

        if ( app.cascadeRightEye )
        {
            cvReleaseHaarClassifierCascade(&app.cascadeRightEye);
        }

        if ( app.memStorage )
        {
            cvReleaseMemStorage(&app.memStorage);
        }

        if ( app.capture )
        {
            cvReleaseCapture(&app.capture);
        }

        if ( app.grayscale )
        {			
            cvReleaseImage(&app.grayscale);
        }

        if ( app.lastImage && app.mode != ProcessCapture )
        {
            cvReleaseImage(&app.lastImage);
        }

        if ( app.kernel1 )
        {
            cvReleaseStructuringElement(&app.kernel1);
        }

        cvDestroyAllWindows();

        return true;
    }
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
            if (app.processOptions.doVisualDebug)
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
                    *p = (unsigned char) dstMin + x * (dstMax - dstMin);
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
    //int djdjd = 0;
    KluEyeFeaturePoints detectEyeFeaturePoints(const IplImage * image,
        const CvPoint & eyeCenter,
        CvMemStorage * storage,                                        
        const char * windowContrastStretch1,
        const char * windowContrastStretch2,
        const char * windowThreshold,
        const char * windowContour,
        const char * windowFeaturePoints)
    {
        CV_Assert(image != NULL && storage != NULL);

        KluEyeFeaturePoints fp;
        memset(&fp, '\0', sizeof(fp));

        // Create a copy of the eye region image
        IplImage * regImg = extractGrayScaleROI(image);

        // Fix eye center coordinate: From global to ROI coordinates
        CvRect region = cvGetImageROI(image);
        fp.center = getRectMidPoint(region);
        fp.center.x -= region.x;
        fp.center.y -= region.y;

        cvEqualizeHist(regImg, regImg);
        //char eq[255];
        //memset(eq, '\0', sizeof(eq));
        //sprintf(eq, "equalized %d", djdjd++);
        //saveImage(regImg, eq);

        // Get some statistical information about the grayscale distribution
        GrayStats stats = getGrayStats(regImg);

        // Calculate the upper and lower gray value bounds
        unsigned char upperBound = (unsigned char) stats.avg + (stats.max - stats.avg) / 2;
        unsigned char lowerBound = (unsigned char) stats.min + (stats.avg - stats.min) / 2;

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

        stretchContrast(regImg, stats.min, (unsigned char) stats.avg, 0, stats.min);
        visDebug(windowContrastStretch2, regImg);

        // Threshold iteration
        unsigned char t;
        unsigned char tNew = (unsigned char) stats.avg;
        do {
            t = tNew;
            GrayStats statsM1 = getGrayStats(regImg, 0, t);
            GrayStats statsM2 = getGrayStats(regImg, t+1, 255);
            tNew = (unsigned char) (statsM1.avg + statsM2.avg) / 2;
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

            // EDIT: We ensure now, that the mid point of the eye lies in the contour's rectangle.
            CvRect cr = cvContourBoundingRect(c);

            if ( (area*area) > biggestArea &&
                fp.center.x > cr.x &&
                fp.center.x < (cr.x + cr.width) &&
                fp.center.y > cr.y &&
                fp.center.y < (cr.y + cr.height))
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
        fp.center.x += region.x;
        fp.center.y += region.y;
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
    KluMouthFeaturePoints detectMouthFeaturePoints(IplImage * image,
        CvMemStorage * storage,
        const char * windowContrastStretch1,
        const char * windowContrastStretch2,
        const char * windowThreshold,
        const char * windowContour,
        const char * windowFeaturePoints)
    {
        CV_Assert(image != NULL && storage != NULL);

        KluMouthFeaturePoints fp;
        memset(&fp, '\0', sizeof(fp));

        // Create a copy of the eye region image
        IplImage * regImg = extractGrayScaleROI(image);

        fp.mouthCenter = getRectMidPoint(cvGetImageROI(image));

        cvEqualizeHist(regImg, regImg);

        //char eq[255];
        //memset(eq, '\0', sizeof(eq));
        //sprintf(eq, "equalized %f", fp.mouthCenter.x);
        //saveImage(regImg, eq);

        //cvFloodFill(regImg, 

        // Get some statistical information about the grayscale distribution
        GrayStats stats = getGrayStats(regImg);

        // Calculate the upper and lower gray value bounds
        unsigned char upperBound = (unsigned char) stats.avg + (stats.max - stats.avg) / 2;
        unsigned char lowerBound = (unsigned char) stats.min + (stats.avg - stats.min) / 2;

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
        
        ////cvErode(regImg, regImg, app.erodeKernel1, 1);
        //cvDilate(regImg, regImg, app.kernel1, 2);
        //visDebug("Contrast strectch 1 after kernel applied", regImg);

        stretchContrast(regImg, stats.min, stats.avg, 0, stats.min);
        visDebug(windowContrastStretch2, regImg);

        //cvErode(regImg, regImg, app.erodeKernel1, 1);

        //cvDilate(regImg, regImg, app.kernel1, 1);
        //visDebug("Contrast strectch 1 after kernel applied", regImg);

        // Threshold iteration
        unsigned char t;
        unsigned char tNew = (unsigned char) stats.avg;
        do {
            t = tNew;
            GrayStats statsM1 = getGrayStats(regImg, 0, t);
            GrayStats statsM2 = getGrayStats(regImg, t+1, 255);
            tNew = (unsigned char) (statsM1.avg + statsM2.avg) / 2;
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
    void drawFfps(IplImage * image, const KluFaceFeaturePoints & ffp)
    {
        if ( !image )
        {
            return;
        }

        // Lips
        if (app.processOptions.doMouthProcessing)
        {
            cvLine(image, ffp.mouth.cornerLeft, ffp.mouth.upperLipLeft, COL_YELLOW, 1);
            cvLine(image, ffp.mouth.upperLipLeft, ffp.mouth.upperLipMiddle, COL_YELLOW, 1);
            cvLine(image, ffp.mouth.upperLipLeft, ffp.mouth.upperLipMiddle, COL_YELLOW, 1);
            cvLine(image, ffp.mouth.upperLipMiddle, ffp.mouth.upperLipRight, COL_YELLOW, 1);
            cvLine(image, ffp.mouth.upperLipRight, ffp.mouth.cornerRight, COL_YELLOW, 1);
            cvLine(image, ffp.mouth.cornerRight, ffp.mouth.lowerLipRight, COL_YELLOW, 1);
            cvLine(image, ffp.mouth.lowerLipRight, ffp.mouth.lowerLipMiddle, COL_YELLOW, 1);
            cvLine(image, ffp.mouth.lowerLipMiddle, ffp.mouth.lowerLipLeft, COL_YELLOW, 1);
            cvLine(image, ffp.mouth.lowerLipLeft, ffp.mouth.cornerLeft, COL_YELLOW, 1);       
        }

        
        if (app.processOptions.doEyeProcessing)
        {
            // Draw eye centers
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
    }
    //------------------------------------------------------------------------------
    bool processImageFrame(IplImage * image,
        KluFaceFeaturePoints * ffp)
    {
        static char buf[255];
        static double processingTime = 0.0;

        if ( !image || !ffp )
        {
            return false;
        }

        // Set all FFPs to zero
        memset(ffp, '\0', sizeof(ffp));

        cvCvtColor(image, app.grayscale, CV_RGB2GRAY);

        //==========================
        //==========================
        //==========================

        tic();

        // Set the minimal rectangle size a face must fit in.
        // The smaller the rectangle the longer the processing time.
        // Therefore set the rectangle small only if the image is small.
        // The same goes for the eyes and the mouth.
        CvSize minFaceSize = cvSize(200, 200);
        if (image->height < 500)
        {
            minFaceSize = cvSize(100, 100);
        }
        vector<CvRect> faceRects = detectObjects(image, app.cascadeFace, app.memStorage, minFaceSize);
        
        // TODO: (Ko) Maby return here, to speed up things a little bit
        //if (faceRects.size() < 0)
        //{
        //    return false;
        //}
        
        CvRect rightEyeSearchWindow;
        CvRect leftEyeSearchWindow;
        CvRect mouthSearchWindow;
        CvRect rightEyeRect;
        CvRect leftEyeRect;
        vector<CvRect> mouthRects;
        CvRect mouthRect;

        if (faceRects.size() > 0)
        {
            CvRect faceRect = faceRects[0];
            if ( faceRect.width != -1 && faceRect.height != -1 )
            {
                ffp->faceRegion = faceRect;

                CvSize eyeMinSize = cvSize(40, 30);
                if (image->height < 500)
                {
                    eyeMinSize = cvSize(8, 6);
                }

                /**
                * Find right eye
                */
                rightEyeSearchWindow = faceRect;
                rightEyeSearchWindow.height /= 4;
                rightEyeSearchWindow.y += rightEyeSearchWindow.height;
                rightEyeSearchWindow.width /= 2;
                cvSetImageROI(image, rightEyeSearchWindow);
                vector<CvRect> eyeRects = detectObjects(image, app.cascadeRightEye, app.memStorage, eyeMinSize);
                if (eyeRects.size() > 0)
                {
                    rightEyeRect = eyeRects[0];
                    rightEyeRect.x += rightEyeSearchWindow.x;
                    rightEyeRect.y += rightEyeSearchWindow.y;

                    if ( app.processOptions.doEyeProcessing)
                    {
                        // Find right eye feature points
                        cvSetImageROI(app.grayscale, rightEyeRect);
                        
                        //cvSetImageROI(image, rightEyeRect);
                        //saveImage(image, "right eye orig col");
                        //saveImage(app.grayscale, "right eye orig gray");
                        //cvResetImageROI(image);

                        ffp->rightEye = detectEyeFeaturePoints(app.grayscale, getRectMidPoint(rightEyeRect), app.memStorage, "RE Contrast Stretch 1", "RE Contrast Stretch 2","RE Threshold", "RE Contour", "RE Feature Points");
                        cvResetImageROI(app.grayscale);
                    }
                    else
                    {                    
                        ffp->rightEye.center = getRectMidPoint(rightEyeRect);
                    }
                }
                cvResetImageROI(image);

                /**
                * Find left eye
                */
                leftEyeSearchWindow = faceRect;
                leftEyeSearchWindow.height /= 4;
                leftEyeSearchWindow.y += leftEyeSearchWindow.height;
                leftEyeSearchWindow.width /= 2;
                leftEyeSearchWindow.x += leftEyeSearchWindow.width;
                cvSetImageROI(image, leftEyeSearchWindow);
                eyeRects = detectObjects(image, app.cascadeLeftEye, app.memStorage, eyeMinSize);
                if (eyeRects.size() > 0)
                {
                    leftEyeRect = eyeRects[0];
                    leftEyeRect.x += leftEyeSearchWindow.x;
                    leftEyeRect.y += leftEyeSearchWindow.y;

                    if ( app.processOptions.doEyeProcessing)
                    {
                        // Find left eye feature points
                        cvSetImageROI(app.grayscale, leftEyeRect);

                        //cvSetImageROI(image, leftEyeRect);
                        //saveImage(image, "left eye orig col");
                        //saveImage(app.grayscale, "left eye orig gray");
                        //cvResetImageROI(image);

                        ffp->leftEye = detectEyeFeaturePoints(app.grayscale, getRectMidPoint(leftEyeRect), app.memStorage, "LE Contrast Stretch 1", "LE Contrast Stretch 2","LE Threshold", "LE Contour", "LE Feature Points");
                        cvResetImageROI(app.grayscale);
                    }
                    else
                    {
                        ffp->leftEye.center = getRectMidPoint(leftEyeRect);
                    }
                }
                cvResetImageROI(image);

                /**
                * Find mouth
                */
                if (app.processOptions.doMouthProcessing )
                {
                    double eyeDist = getDist(ffp->leftEye.center, ffp->rightEye.center);
                    CvPoint mouthCenter = cvPoint((int) ffp->rightEye.center.x + eyeDist * 0.5, (int) ffp->rightEye.center.y + eyeDist * 1.1);

                    mouthSearchWindow = faceRect;
                    mouthSearchWindow.height /= 2;
                    mouthSearchWindow.y += mouthSearchWindow.height;

                    CvSize mouthMinSize = cvSize(120, 90);
                    if (image->height < 500)
                    {
                        mouthMinSize = cvSize(24, 18);
                    }

                    cvSetImageROI(image, mouthSearchWindow);
                    mouthRects = detectObjects(image, app.cascadeMouth, app.memStorage, mouthMinSize);
                    if (mouthRects.size() > 0)
                    {
                        mouthRect = mouthRects[0];
                        mouthRect.x += mouthSearchWindow.x;
                        mouthRect.y += mouthSearchWindow.y;

                        cvSetImageROI(app.grayscale, mouthRect);

                        //cvSetImageROI(image, mouthRect);
                        //saveImage(image, "mouth orig col");
                        //saveImage(app.grayscale, "mouth orig gray");
                        //cvResetImageROI(image);

                        ffp->mouth = detectMouthFeaturePoints(app.grayscale, app.memStorage, "M Contrast Stretch 1", "M Contrast Stretch 2","M Threshold", "M Contour", "M Feature Points");
                        cvResetImageROI(app.grayscale);
                    }
                    cvResetImageROI(image);
                }
            }
        
            processingTime = toc();

            /**
            * Draw object/face rects
            */        
            for (vector<CvRect>::size_type i=0; app.processOptions.drawFaceRectangle && i<faceRects.size(); i++)
            {
                drawRect(image, faceRects[i], COL_RED);
            }

            double eyeDist = getDist(ffp->leftEye.center, ffp->rightEye.center);

            if (app.processOptions.drawSearchRectangles)
            {
                if (app.processOptions.doEyeProcessing)
                {
                    drawRect(image, rightEyeSearchWindow, COL_GREEN);
                    drawRect(image, leftEyeSearchWindow, COL_BLUE);
                    drawRect(image, rightEyeRect, COL_GREEN);
                    drawRect(image, leftEyeRect, COL_BLUE);
                }

                if (app.processOptions.doMouthProcessing)
                {
                    // Mouth rect
                    drawRect(image, mouthSearchWindow, COL_BLUE);
                    drawRect(image, mouthRect, COL_LIME_GREEN);
                }
            }

            if (app.processOptions.drawAnthropometricPoints)
            {
                // Draw eye-midpoint connection line
                cvLine(image, ffp->rightEye.center, ffp->leftEye.center, COL_BLUE, 1);

                // Draw anthropometric eyebrow distance line (0.33% of eye mid-points distance)		    
                cvLine(image, ffp->rightEye.center, cvPoint((int) ffp->rightEye.center.x, (int) ffp->rightEye.center.y - eyeDist * 0.33), COL_RED, 1);
                cvLine(image, ffp->leftEye.center, cvPoint((int) ffp->leftEye.center.x, (int) ffp->leftEye.center.y - eyeDist * 0.33), COL_RED, 1);

                // Draw nose tip
                CvPoint noseTip = cvPoint((int) ffp->rightEye.center.x + eyeDist * 0.5, (int) ffp->rightEye.center.y + eyeDist * 0.6);
                drawCross(image, noseTip);

                // Draw estimated mouth center
                CvPoint mouthCenter = cvPoint((int) ffp->rightEye.center.x + eyeDist * 0.5, (int) ffp->rightEye.center.y + eyeDist * 1.1);
                drawCross(image, mouthCenter);
            }

            if (app.processOptions.drawFeaturePoints)
            {
                drawFfps(image, *ffp);
            }
        }

        if (app.processOptions.drawDetectionTime)
        {
            // Print processing time for detection (after ROI reset!!)
            memset(buf, '\0', 255);
            sprintf(buf, "Detection Time: %0.3gms", processingTime);
            CvSize textBoundings;
            int ymin;
            cvGetTextSize(buf, &app.font, &textBoundings, &ymin);
            CvPoint org = cvPoint(0, image->height - 8);
            cvRectangle(image, cvPoint(org.x, org.y + ymin), cvPoint(org.x + textBoundings.width, org.y - textBoundings.height), cvScalar(0,0,0,0.5), CV_FILLED);
            cvPutText(image, buf, cvPoint(0, image->height - 8), &app.font, CV_RGB(0,255,0));		
        }

        //saveImage(image, "result");

        //==========================
        //==========================
        //==========================

        return true;
    }
    //------------------------------------------------------------------------------
}
