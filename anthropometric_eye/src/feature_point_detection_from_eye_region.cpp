#include "../../common/src/common.hpp"

using namespace klu;
using namespace std;
using namespace cv;

#define WINDOW_RESULT "Result"
#define WINDOW_ORIGINAL "Original"


// This struct encapsulates all program options and variables
struct ApplicationEnvironment
{
	CvCapture * capture; // NULL if no capture is being used.
	bool isExiting;
	IplImage * frame; // Pointer to the current frame
} app;

/*!
	The \c EyeFeaturePoints struct contains the X-Y coordinates of
	all the feature points for one eye. Coordinates are considered
	to be image and not region coordinates.
*/
struct EyeFeaturePoints
{
	cv::Point2i upperLid;
	cv::Point2i bottomLid;
	cv::Point2i cornerLeft; // from point of view
	cv::Point2i cornerRight;// from point of view
};

/*!
	Detects the all the eye-feature points (see \c EyeFeaturePoints) in the \a region
	of the \a image.

	See: Abu Sayeed Md. Sohail and Prabir Bhattacharya
	     "Detection of Facial Feature Points Using Anthropometric Face Model"
	     p. 660
*/
EyeFeaturePoints detectEyeFeaturePoints(IplImage * image, const CvRect & region = cvRect(0,0,0,0))
{
	EyeFeaturePoints featurePoints;

	// Create a temporary copy of the image region
	IplImage * regionImage = NULL;

	try
	{
		// Test if parameters are OK
		if ( !image || image->depth != IPL_DEPTH_8U || image->nChannels != 1 )
		{
			throw std::string("Image must be an IPL_DEPTH_8U grayscale image with one channel only!");
		}

		if ( region.height != 0 && region.width != 0 )
		{
			regionImage = cvCreateImage(cvSize(region.width, region.height), IPL_DEPTH_8U, 1);
			// @TODO Find a different way to copy subimage to "tmp" so "image" parameter can be const again.
			CvRect currentRoi = cvGetImageROI(image);
			cvSetImageROI(image, region);
			cvResize(image, regionImage);
			cvSetImageROI(image, currentRoi);
		}
		else
		{
			// No need to set ROI, simply copy "image" to "tmp".
			regionImage = cvCloneImage(image);
		}

		if ( !regionImage )
		{
			throw std::string("Failed to create copy of image region!");
		}

		// Calculate the mean intensity value of all pixel values in the image region
		cv::Mat frameMatrix = image;
		cv::Scalar m = cv::mean(frameMatrix);
		int mean = m[0];

		// Determine which will be the minimum and maximum bound that make up the
		// values around the mean value. For example, let 110 be the mean grayscale
		// value of all pixels. Then the lower bound is 0.5 * 110=55 and the upper
		// bound is 1.5 * 110=165.
		int upperBound = 1.5 * mean;
		int lowerBound = 0.5 * mean;

		// Output all values once to console
		static bool done = false;
		if ( !done )
		{
			std::cerr << "mean = " << mean << std::endl;
			std::cerr << "upperBound = " << upperBound << std::endl;
			std::cerr << "lowerBound = " << lowerBound << std::endl;
			done = true;
		}

		// Saturate!

		// @TODO Maby we need to re-consider what is meant by the lower half of 50%
		//       of the image intensity cumulative distribution! Think!

		// Saturate lower half of 50% of the cumulative intensity distribution to 0
		// Saturate upper half of 50% of the cumulative intensity distribution to 1
		// ...

		// Iterate over rows
		for ( int r=0; r < regionImage->height; r++ )
		{
			// Iterate over columns
			for ( int c=0; c < regionImage->width; c++)
			{
				// Get the pointer to the intensity value
				unsigned char * intensity = (unsigned char*) 	regionImage->imageData +
																r * regionImage->widthStep +
																c * regionImage->nChannels;

				if ( *intensity > upperBound )
				{
					*intensity = 255;
				}
				else if ( *intensity < lowerBound )
				{
					*intensity = 0;
				}
			}
		}

	}
	catch ( const std::string & e )
	{
		std::cerr << "detectEyeFeaturePoints(): " << e << std::endl;
	}

	// Release temporary image
	if ( regionImage )
	{
		cvReleaseImage(&regionImage);
	}

	return featurePoints;
}

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

	app.capture = 0; //cvCreateCameraCapture(CV_CAP_ANY);	

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
		image = cvLoadImage( "../../data/fig3a.png", 1);
	}

	// Grayscale / Intensity image	
	IplImage * grayscaleFrame = cvCreateImage(cvSize(image->width, image->height), IPL_DEPTH_8U, 1);
	cvCvtColor(image, grayscaleFrame, CV_RGB2GRAY);

	// Contour stuff
	CvMemStorage * 	storage = cvCreateMemStorage(0); // block size = 0 means default
	CvSeq * contours = NULL;
	CvSeq * firstContour = NULL;

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

		// Calc mean
		cv::Mat frameMatrix = app.frame;
		cv::Scalar m = cv::mean(frameMatrix);
		int mean = m[0];
		//cerr << "mean = " << mean << endl;

		// 50 % of mean
		int upperBound = mean + 0.5 * (255 - mean); // old idea: 1.5 * mean
		//int upperBound = 1.5 * mean;
		int lowerBound = 0.5 * mean;

//		cvNormalize(grayscaleFrame, grayscaleFrame, 0, lowerBound, CV_MINMAX);//CV_MINMAX);
//		cvNormalize(grayscaleFrame, grayscaleFrame, upperBound, 255, CV_MINMAX);//CV_MINMAX);

		static bool done = false;
		if ( !done )
		{
			cerr << "Color info:" << endl;
			cerr << "mean = " << mean << std::endl;
			std::cerr << "upperBorder = " << upperBound << std::endl;
			std::cerr << "lowerBorder = " << lowerBound << std::endl;
			done = true;
		}
		// Saturate lower half of 50% of the cumulative intensity distribution to 0
		//cvThreshold(grayscaleFrame, grayscaleFrame, lowerBorder, 0, THRESH_TOZERO);
		// Saturate upper half of 50% of the cumulative intensity distribution to 1
		// ...
		for ( int r=0; r < grayscaleFrame->height; r++ )
		{
			for ( int c=0; c < grayscaleFrame->width; c++)
			{
				unsigned char * p = (unsigned char*) grayscaleFrame->imageData +
						r * grayscaleFrame->widthStep +
						c * grayscaleFrame->nChannels;
				// Create the mask
//				if ( *p > upperBound ) { (frameMatrix.ptr(r))[c] = 1; }
//				else { (frameMatrix.ptr(r))[c] = 0; }
				//else if ( *p < lowerBound ) { *p = 0; }

				//if ( *p > mean ) { *p = 255; }
				// Intensity window looks like Figure 3.b with this configuration:
				/*
				if ( *p > mean ) { *p = 255; }
				// It seems that in Figure 3.b, no pixel is set to 0,
				// therefore commented out
				//else if ( *p < lowerBound ) { *p = 0; }
				*/

				// This was the original implementation we though would be correct
//				if ( *p > upperBound )	{ *p = 255; }
//				else if ( *p < lowerBound )	{ *p = 0; }

			}
		}

	    cv::Mat src = cv::cvarrToMat(grayscaleFrame);
	    cv::Mat dst = cv::cvarrToMat(grayscaleFrame);
	    cv::Mat & mask = frameMatrix;
	    CV_Assert( dst.size() == src.size() && src.channels() == dst.channels() );
	    //cv::normalize( src, dst, 0, 255, CV_MINMAX, dst.type(), mask );
		//cvNormalize(grayscaleFrame, grayscaleFrame, upperBound, 255, CV_MINMAX, frameMatrix);//CV_MINMAX);

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
