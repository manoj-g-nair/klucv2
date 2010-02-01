#include "../../common/src/common.hpp"

#define WINDOW_RESULT "Facial Feature Regions"

using namespace klu;
using namespace std;
using namespace cv;

#define WINDOW_ORIGINAL "Original"

// This struct encapsulates all program options and variables
struct ApplicationEnvironment
{
	CvCapture * capture; // NULL if no capture is being used.
	bool isExiting;
	IplImage * frame; // Pointer to the current frame
} app;

int main(int argc, char * argv[])
{    
	// Print keys
	std::cerr << "Detects feature points from mouth region." << std::endl;
	std::cerr << std::endl;
	std::cerr << "Keys:" << std::endl;
	std::cerr << "---------------" << std::endl;
	std::cerr << "p   = Saves all currently open windows to image files (e.g. \"Result TIMESTAMP.png\")" << std::endl;
	std::cerr << "ESC = Exit the program" << std::endl;

	// BEGIN Initialize the application

	app.isExiting = false;
	app.frame = NULL;

	// END Initialize the application

	// Create the windows
	//cvNamedWindow(WINDOW_ORIGINAL, 0);
    cvNamedWindow(WINDOW_RESULT, 0);

	app.capture = 0;//cvCreateCameraCapture(CV_CAP_ANY);

	IplImage * image = NULL;

	if ( app.capture )
	{
		std::cerr << "Found a caputure interface." << std::endl;
		image = cvQueryFrame(app.capture);		
	}
	else
	{
		std::cerr << "No capture found. Using static image." << std::endl;
		// Load the image
		image = cvLoadImage( "../data/image_0235.jpg", 1);
	}

	// Grayscale / Intensity image	
	IplImage * grayscaleFrame = cvCreateImage(cvSize(image->width, image->height), IPL_DEPTH_8U, 1);
	cvCvtColor(image, grayscaleFrame, CV_RGB2GRAY);

	// Contour stuff
	CvMemStorage * 	storage = cvCreateMemStorage(0); // block size = 0 means default

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

		//cvShowImage(WINDOW_ORIGINAL, app.frame);
		//cvResizeWindow(WINDOW_ORIGINAL, 320, 240);

		// Convert frame to grayscale / intensity and show
		cvCvtColor(image, grayscaleFrame, CV_RGB2GRAY);


		// Saturate lower half of 50% of the cumulative intensity distribution to 0
		//cvThreshold(grayscaleFrame, grayscaleFrame, lowerBorder, 0, THRESH_TOZERO);
		// Saturate upper half of 50% of the cumulative intensity distribution to 1
		// ...
//		for ( int r=0; r < grayscaleFrame->height; r++ )
//		{
//			for ( int c=0; c < grayscaleFrame->width; c++)
//			{
//				unsigned char * p = (unsigned char*) grayscaleFrame->imageData +
//						r * grayscaleFrame->widthStep +
//						c * grayscaleFrame->nChannels;
//
//			}
//		}

		// Threshold (Fig.3c)
		unsigned char t;
		unsigned char tNew = avg;
		do {
			t = tNew;
			unsigned char m1 = getAvgMinMaxGrayValue(grayscaleFrame, 0, t);
			unsigned char m2 = getAvgMinMaxGrayValue(grayscaleFrame, t, 255);
			tNew = (m1 + m2) / 2;
		} while (t != tNew );
		cvThreshold(grayscaleFrame, grayscaleFrame, t, 255, CV_THRESH_BINARY);

		cvResizeWindow(WINDOW_RESULT, 320, 240);
		cvShowImage(WINDOW_RESULT, grayscaleFrame);

		// Find contours in inverted binary image

		cvThreshold(grayscaleFrame, grayscaleFrame, 1, 255, CV_THRESH_BINARY);
		int nContours = cvFindContours(grayscaleFrame, storage, &firstContour, sizeof(CvContour), CV_RETR_EXTERNAL, CV_CHAIN_APPROX_NONE );
		
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
			//cvDrawContours(
			//	app.frame, // target image
			//	biggestContour, // contour
			//	CV_RGB(255, 255, 0),		// external color
			//	CV_RGB(0, 0, 255),	// hole color
			//	1,			// Vary max_level and compare results
			//	2, // thickness
			//	8 // type
			//);


			// Go through all contour points and find the right- and leftmost

			// @TODO also save the seq IDX for further contour processing and search for upper and lower lid.

			CvPoint rightmost = cvPoint(0, 0);
			CvPoint leftmost = cvPoint(app.frame->width, app.frame->height);

			int nElements = biggestContour->total;

			for ( int i = 0; i < nElements; i++)
			{
				CvPoint p = *((CvPoint*) cvGetSeqElem(biggestContour, i));

				if ( p.x > rightmost.x )
				{
					rightmost = p;
				}
				if ( p.x < leftmost.x )
				{
					leftmost = p;
				}
				//fprintf(stderr, "%d %p\n", biggestContour->total, (CvPoint*) cvGetSeqElem(biggestContour, 0));
			}

			// Draw right- and leftmost contour points
//			cvDrawLine(app.frame, rightmost, rightmost, CV_RGB(0, 255, 0), 10);
//			cvDrawLine(app.frame, leftmost, leftmost, CV_RGB(0, 255, 0), 10);

		}

		biggestContour = NULL;

		// Control the program
        int key = cvWaitKey(10);

        if( (char) key == 27 )
		{
            app.isExiting = true;
		}

		switch( (char) key )
		{
		// Print/Save image to file
		case 'p':
			saveImage(app.frame, WINDOW_RESULT);
			break;
		}

		// Release the current frame (DON'T DO THIS IF CAPTURING!!!)
		if ( !app.capture )
		{			
			cvReleaseImage(&app.frame);
		}

    }

	// Cleanup OpenCV
	cvReleaseMemStorage(&storage);
	if (!app.capture)
	{
		cvReleaseImage(&image);
	}
	cvReleaseCapture(&app.capture);
	cvReleaseImage(&grayscaleFrame);
	cvDestroyAllWindows();

    return 0;
}
