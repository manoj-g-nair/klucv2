#define CV_NO_BACKWARD_COMPATIBILITY
#define HIGHGUI_NO_BACKWARD_COMPATIBILITY

#include <cv.h>
#include <highgui.h>

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <assert.h>
#include <math.h>
#include <float.h>
#include <limits.h>
#include <time.h>
#include <ctype.h>
#include <iostream>

#define WINDOW_RESULT "Result"
#define WINDOW_INTEGRAL_IMAGE_SUM "Integral Image (sum)"
#define WINDOW_INTEGRAL_IMAGE_SQUARED_SUM "Integral Image (squared sum)"
#define WINDOW_INTEGRAL_IMAGE_TILTED_SUM "Integral Image (tilted sum)"

using namespace std;
using namespace cv;

// This struct encapsulates all program options and variables
struct ApplicationEnvironment
{
	CvCapture * capture; // NULL if no capture is being used.
	bool detectFace;
	bool isSelecting;
	bool selectionEnabled;
	bool showIntegralImages;
	CvRect selection;
	bool isExiting;
	CvFont font;
	IplImage * frame; // Pointer to the current frame
	// BEGIN Facedetect parameters
	CascadeClassifier cascade;
	CascadeClassifier nestedCascade;	
    double scale;
	// END Facedetect parameters
} app;

// Timing function to start the global timer
static double ticStart = 0;
void tic()
{
    ticStart = (double)cvGetTickCount();
}

// Timing function that stops the global timer and
// prints the time since the last call to tic().
double toc()
{
    ticStart = ((double)cvGetTickCount() - ticStart) / ( (double) cvGetTickFrequency() * 1000.0);
	// Print to unbuffered standard error for immediate output
    //fprintf(stderr, "detection time = %gms\n", ticStart);
    return ticStart;
}

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

// Callback for image ROI selection
void onMouseSelection(int event, int x, int y, int flags, void * frame)
{
	static CvPoint origin;

	IplImage * image = static_cast<IplImage*>(frame);

    if( !image )
	{
        return;
	}

    if( image->origin == IPL_ORIGIN_BL ) // bottom-left (BL) or top-left (TL)? 
	{		
        y = image->height - y;
	}

	if( app.isSelecting )
    {
		//cerr << "Selecting object" << endl;
		app.selection.x = MIN(x,origin.x);
        app.selection.y = MIN(y,origin.y);
        app.selection.width = app.selection.x + CV_IABS(x - origin.x);
        app.selection.height = app.selection.y + CV_IABS(y - origin.y);
        
        app.selection.x = MAX( app.selection.x, 0 );
        app.selection.y = MAX( app.selection.y, 0 );
        app.selection.width = MIN( app.selection.width, image->width );
        app.selection.height = MIN( app.selection.height, image->height );
        app.selection.width -= app.selection.x;
        app.selection.height -= app.selection.y;
    }

    switch( event )
    {
    case CV_EVENT_LBUTTONDOWN:		
        origin = cvPoint(x,y);
        app.selection = cvRect(x,y,0,0);
		app.isSelecting = true;
		// Enable selection if it was disabled.
		app.selectionEnabled = true;
        break;
    case CV_EVENT_LBUTTONUP:
        app.isSelecting = false;
        if( app.selection.width > 0 && app.selection.height > 0 )
		{
			//cerr << "Setting track_object to -1" << endl;
            //track_object = -1;
		}
        break;
    }
}

void drawSelectionInfo()
{
	static char metricsText[255];
	
	if( app.selectionEnabled && app.selection.width > 0 && app.selection.height > 0)
	{		
		// Invert the image if we are currently selecting
		if ( app.isSelecting )
		{		
			cvSetImageROI( app.frame, app.selection );
			cvXorS( app.frame, cvScalarAll(255), app.frame, 0 );
			cvResetImageROI( app.frame );
		}

		CvPoint org;
		CvSize textBoundings;
		int ymin;

		// Draw the text with the image and selection metrics.
		memset(metricsText, '\0', 255);
		sprintf(metricsText, "Img: %dx%d", app.frame->width, app.frame->height);
		cvGetTextSize(metricsText, &app.font, &textBoundings, &ymin);
		org = cvPoint(app.selection.x, app.selection.y - 25);		
		cvRectangle(app.frame, cvPoint(org.x, org.y + ymin), cvPoint(org.x + textBoundings.width, org.y - textBoundings.height), cvScalar(0,0,0), CV_FILLED); 
		cvPutText(app.frame, metricsText, org, &app.font, CV_RGB(0,255,0));									
		
		float ratio = (float)(app.selection.width * app.selection.height) / (float)(app.frame->width * app.frame->height);
		memset(metricsText, '\0', 255);
		sprintf(metricsText, "Sel: %dx%d~%0.3f%%", app.selection.width, app.selection.height, ratio * 100.0f);
		cvGetTextSize(metricsText, &app.font, &textBoundings, &ymin);
		org = cvPoint(app.selection.x, app.selection.y - 10);
		cvRectangle(app.frame, cvPoint(org.x, org.y + ymin), cvPoint(org.x + textBoundings.width, org.y - textBoundings.height), cvScalar(0,0,0), CV_FILLED); 
		cvPutText(app.frame, metricsText, org, &app.font, CV_RGB(0,255,0));	

		
		// Draw the ROI
		CvPoint p1 = {app.selection.x, app.selection.y};
		CvPoint p2 = {p1.x + app.selection.width, p1.y + app.selection.height};
		cvRectangle(app.frame, p1, p2, CV_RGB(0,255,0));
	}	
}

// Detects faces in "img" and returns the processing time
double detectAndDraw( Mat& img,
                   CascadeClassifier& cascade,
				   CascadeClassifier& nestedCascade,
                   double scale)
{
	if (!app.detectFace)
	{
		return 0;
	}

    int i = 0;    
    const static Scalar colors[] =  { CV_RGB(0,0,255),
        CV_RGB(0,128,255),
        CV_RGB(0,255,255),
        CV_RGB(0,255,0),
        CV_RGB(255,128,0),
        CV_RGB(255,255,0),
        CV_RGB(255,0,0),
        CV_RGB(255,0,255)} ;
    Mat gray;
	Mat smallImg( cvRound (img.rows/scale), cvRound(img.cols/scale), CV_8UC1 );

    cvtColor( img, gray, CV_BGR2GRAY );
    resize( gray, smallImg, smallImg.size(), 0, 0, INTER_LINEAR );
    equalizeHist( smallImg, smallImg );

	tic();
	vector<Rect> faces;
    cascade.detectMultiScale(
		smallImg,
		faces,
        1.1, 2, 0
        //|CV_HAAR_FIND_BIGGEST_OBJECT
        //|CV_HAAR_DO_ROUGH_SEARCH
        |CV_HAAR_SCALE_IMAGE,
        Size(30, 30)
	);
    double t = toc();

    for( vector<Rect>::const_iterator r = faces.begin(); r != faces.end(); r++, i++ )
    {        
        vector<Rect> nestedObjects;
        Scalar color = colors[i%8];

		Point center( cvRound((r->x + r->width*0.5)*scale),
				      cvRound((r->y + r->height*0.5)*scale) );

        int radius = cvRound((r->width + r->height)*0.25*scale);
        
		circle( img, center, radius, color, 3, 8, 0 );

        if( !nestedCascade.empty() )
		{
			Mat smallImgROI = smallImg(*r);

			nestedCascade.detectMultiScale(
				smallImgROI,
				nestedObjects,
				1.1, 2, 0
				//|CV_HAAR_FIND_BIGGEST_OBJECT
				//|CV_HAAR_DO_ROUGH_SEARCH
				//|CV_HAAR_DO_CANNY_PRUNING
				|CV_HAAR_SCALE_IMAGE
				,
				Size(30, 30) );
			for( vector<Rect>::const_iterator nr = nestedObjects.begin(); nr != nestedObjects.end(); nr++ )
			{
				center.x = cvRound((r->x + nr->x + nr->width*0.5)*scale);
				center.y = cvRound((r->y + nr->y + nr->height*0.5)*scale);
				radius = cvRound((nr->width + nr->height)*0.25*scale);
				circle( img, center, radius, color, 3, 8, 0 );
			}
		}
    }  
    //cv::imshow( "result", img );
	return t;
}

int main(int argc, char * argv[])
{    
	// Print keys
	cerr << "This is a highly modified version of the OpenCV facedetect demo." << endl;
	cerr << "You can select a ROI (Region Of Interest) of a picture and see the detection time go down." << endl;
	cerr << endl;
	cerr << "Keys:" << endl;
	cerr << "---------------" << endl;
	cerr << "f   = Toggle face detection" << endl;
	cerr << "r   = Reset ROI (enables or disables a selection)" << endl;
	cerr << "i   = Toggle integral image windows (sum/squared sum/tilted sum)" << endl;
	cerr << "p   = Saves all currently open windows to image files (e.g. \"Result TIMESTAMP.png\")" << endl;
	cerr << "ESC = Exit the program" << endl;

	// BEGIN Initialize the application

	app.detectFace = false;
	app.isSelecting = false;
	app.showIntegralImages = false;
	app.isExiting = false;
	app.selectionEnabled = true;
	cvInitFont(&(app.font), CV_FONT_HERSHEY_SIMPLEX, 0.5, 0.5, 0, 1, CV_AA);
	app.frame = 0;	
	app.scale = 1;

	// Load the cascades
	if( !app.cascade.load("../../data/haarcascades/haarcascade_mcs_eyepair_small.xml") )
	{
		cerr << "Failed to load cascade" << endl;
		exit(1);
	}

	//if( !app.nestedCascade.load("../data/haarcascades/haarcascade_mcs_mouth.xml") )
	//{
	//	cerr << "Failed to load  nested cascade" << endl;
	//	exit(1);
	//}	

	// END Initialize the application

	// Create the result window
    cvNamedWindow(WINDOW_RESULT, 1);	    

	app.capture = cvCreateCameraCapture(CV_CAP_ANY);	

	IplImage * image = NULL;

	if ( app.capture )
	{
		cerr << "Found a caputure interface." << endl;	
		image = cvQueryFrame(app.capture);		
	}
	else
	{
		cerr << "No capture found. Using static image." << endl;		
		// Load the image
		image = cvLoadImage( "../../data/lena.jpg", 1 );
	}

	// See http://opencv.willowgarage.com/documentation/miscellaneous_image_transformations.html#integral
	// "Using these integral images, one may calculate sum, mean and standard deviation over a specific
	//  up-right or rotated rectangular region of the image in a constant time [...]"
	cvNamedWindow(WINDOW_INTEGRAL_IMAGE_SUM, 1);
	cvNamedWindow(WINDOW_INTEGRAL_IMAGE_SQUARED_SUM, 1);
	cvNamedWindow(WINDOW_INTEGRAL_IMAGE_TILTED_SUM, 1);
	IplImage * integralImageSum = cvCreateImage(cvSize(image->width+1, image->height+1), 64, image->nChannels);	
	IplImage * integralImageSquaredSum = cvCreateImage(cvSize(image->width+1, image->height+1), 64, image->nChannels);	
	IplImage * integralImageTiltedSum = cvCreateImage(cvSize(image->width+1, image->height+1), 64, image->nChannels);	

	// Set a mouse callback for selection
	cvSetMouseCallback(WINDOW_RESULT, onMouseSelection, image );

	// A text buffer for detection processing time
	char buf[255];

	while ( !app.isExiting )
	{
		if ( app.capture )
		{
			app.frame = cvQueryFrame(app.capture);
		}
		else		
		{
			// Capture a frame or simply clone the image if it's still
			app.frame = cvCloneImage(image);
		}

		// Show integral image
		if ( app.showIntegralImages )
		{			
			cvIntegral(app.frame, integralImageSum, integralImageSquaredSum, integralImageTiltedSum);
			cvShowImage(WINDOW_INTEGRAL_IMAGE_SUM, integralImageSum);
			cvShowImage(WINDOW_INTEGRAL_IMAGE_SQUARED_SUM, integralImageSquaredSum);
			cvShowImage(WINDOW_INTEGRAL_IMAGE_TILTED_SUM, integralImageTiltedSum);
		}

		// If selection is enabled, set the ROI otherwise reset the ROI
		if ( app.selectionEnabled && app.selection.width > 0 && app.selection.height > 0 )
		{
			cvSetImageROI(app.frame, app.selection);
		}
		else
		{
			cvResetImageROI(app.frame);
		}

		// Create a matrix from the current frame
		Mat frameMatrix = app.frame;

		// Detect face
		double processingTime = detectAndDraw(frameMatrix, app.cascade, app.nestedCascade, app.scale);

		// Reset the image ROI, otherwise only the ROI will be shown
		cvResetImageROI(app.frame);	

		// Print processing time for detection (after ROI reset!!)
		if ( app.detectFace )
		{
			memset(buf, '\0', 255);
			sprintf(buf, "Detection Time: %0.3gms", processingTime);
			CvSize textBoundings;
			int ymin;
			cvGetTextSize(buf, &app.font, &textBoundings, &ymin);
			CvPoint org = cvPoint(0, app.frame->height - 8);
			cvRectangle(app.frame, cvPoint(org.x, org.y + ymin), cvPoint(org.x + textBoundings.width, org.y - textBoundings.height), cvScalar(0,0,0,0.5), CV_FILLED);
			cvPutText(app.frame, buf, cvPoint(0, app.frame->height - 8), &app.font, CV_RGB(0,255,0));		
		}

		// Draw the selection information immediately before showing the image		
		drawSelectionInfo();

		// Show the composited image
		cvShowImage(WINDOW_RESULT, app.frame );	

		// Control the program
        int key = cvWaitKey(10);

        if( (char) key == 27 )
		{
            app.isExiting = true;
		}

		switch( (char) key )
		{
		// Toggle face-detection
		case 'f':
			app.detectFace = app.detectFace ? false : true;
			break;
		// Toogle selection enabled
		case 's':
		case 'r':
			app.selectionEnabled = app.selectionEnabled ? false : true;
			//cerr << "Selection enabled status = " << app.selectionEnabled << endl;
			break;
		// Print/Save image to file
		case 'p':
			saveImage(app.frame, WINDOW_RESULT);
			if ( app.showIntegralImages )
			{
				saveImage(integralImageSum, WINDOW_INTEGRAL_IMAGE_SUM);
				saveImage(integralImageSquaredSum, WINDOW_INTEGRAL_IMAGE_SQUARED_SUM);
				saveImage(integralImageTiltedSum, WINDOW_INTEGRAL_IMAGE_TILTED_SUM);
			}
			break;
		// Show integral image
        case 'i':
			app.showIntegralImages = app.showIntegralImages ? false : true;
            if( !app.showIntegralImages )
			{
				cvDestroyWindow(WINDOW_INTEGRAL_IMAGE_SUM);
				cvDestroyWindow(WINDOW_INTEGRAL_IMAGE_SQUARED_SUM);
				cvDestroyWindow(WINDOW_INTEGRAL_IMAGE_TILTED_SUM);
			}
            else
			{
                cvNamedWindow(WINDOW_INTEGRAL_IMAGE_SUM, 1 );
				cvNamedWindow(WINDOW_INTEGRAL_IMAGE_SQUARED_SUM, 1 );
				cvNamedWindow(WINDOW_INTEGRAL_IMAGE_TILTED_SUM, 1 );
			}
            break;
		}

		// Release the current frame (DON'T DO THIS IF CAPTURING!!!)
		if ( !app.capture )
		{			
			cvReleaseImage(&app.frame);
		}
    }

	// Cleanup OpenCV
	cvReleaseCapture(&app.capture);
	cvReleaseImage(&integralImageSum);
	cvReleaseImage(&integralImageSquaredSum);
	cvReleaseImage(&integralImageTiltedSum);
	cvReleaseImage(&image);    
	cvDestroyAllWindows();

    return 0;
}
