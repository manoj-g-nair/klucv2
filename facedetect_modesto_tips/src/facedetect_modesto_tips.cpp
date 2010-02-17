#include "../../common/src/common.hpp"

#define WINDOW_RESULT "Facial Feature Regions"

using namespace klu;
using namespace std;
using namespace cv;

// This struct encapsulates all program options and variables
struct ApplicationEnvironment
{
	CvCapture * capture; // NULL if no capture is being used.
	bool isExiting;
	CvFont font;
	IplImage * frame; // Pointer to the current frame

    // BEGIN Old OpenCV 1.x interface params
	CvHaarClassifierCascade * cascadeFace;
	//CvHaarClassifierCascade * cascadeEyes;
	CvHaarClassifierCascade * cascadeMouth;
	CvHaarClassifierCascade * cascadeRightEye;
	CvHaarClassifierCascade * cascadeLeftEye;
    CvMemStorage * memStorage;
    // END Old OpenCV 1.x interface params
} app;

int main(int argc, char * argv[])
{    
	// Print keys
	cerr << "Facial Feature Points (FFP) Detection" << endl;
	cerr << endl;
	cerr << "Keys:" << endl;
	cerr << "---------------" << endl;
	cerr << "p   = Saves the images of all windows and visual debugs (even if not visable) to an image file (e.g. \"WINDOWNAME TIMESTAMP.png\")" << endl;
    cerr << "d   = Toggle visual debugging on and off." << endl;
	cerr << "ESC = Exit the program" << endl;

	// BEGIN Initialize the application

	app.isExiting = false;
	cvInitFont(&(app.font), CV_FONT_HERSHEY_SIMPLEX, 0.5, 0.5, 0, 1, CV_AA);
	app.frame = 0;	
	// Initialize old OpenCV 1.x stuff for object detection
	app.cascadeFace = (CvHaarClassifierCascade*) cvLoad("../data/haarcascades/haarcascade_frontalface_alt.xml", 0, 0, 0 );
    app.cascadeLeftEye = (CvHaarClassifierCascade*) cvLoad("../data/haarcascades/haarcascade_mcs_lefteye.xml", 0, 0, 0 );
	app.cascadeRightEye = (CvHaarClassifierCascade*) cvLoad("../data/haarcascades/haarcascade_mcs_righteye.xml", 0, 0, 0 );
	app.cascadeMouth = (CvHaarClassifierCascade*) cvLoad("../data/haarcascades/haarcascade_mcs_mouth.xml", 0, 0, 0 );
	app.memStorage = cvCreateMemStorage(0);

	// END Initialize the application

	// Create the result window
    cvNamedWindow(WINDOW_RESULT);

	app.capture = 0;//cvCreateCameraCapture(CV_CAP_ANY);//CV_CAP_ANY);

	IplImage * image = NULL;

	if ( app.capture )
	{
		cerr << "Found a capture interface." << endl;

//		// Manipulate properties of the camera.
//		cvSetCaptureProperty(app.capture, CV_CAP_PROP_FRAME_WIDTH, double(640));
//		cvSetCaptureProperty(app.capture, CV_CAP_PROP_FRAME_HEIGHT, double(480));
//		int camWidth = cvGetCaptureProperty(app.capture, CV_CAP_PROP_FRAME_WIDTH);
//		int camHeight = cvGetCaptureProperty(app.capture, CV_CAP_PROP_FRAME_HEIGHT);
//		cout << "Frame width    = " << camWidth << endl;
//		cout << "Frame heigth   = " << camHeight << endl;

		image = cvQueryFrame(app.capture);
	}
	else
	{
		cerr << "No capture found. Using static image." << endl;		
		// Load the image
		image = cvLoadImage( "../data/gesicht_mann.jpg", 1 );
	}

    IplImage * grayscaleFrame = cvCreateImage(cvSize(image->width, image->height), IPL_DEPTH_8U, 1);

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

        // Convert frame to grayscale / intensity and show
		cvCvtColor(app.frame, grayscaleFrame, CV_RGB2GRAY);

		tic();
		CvSize minFaceSize = cvSize(200, 200);
		vector<CvRect> faceRects = detectObjects(app.frame, app.cascadeFace, app.memStorage, minFaceSize);
		CvRect rightEyeSearchWindow;
		CvRect leftEyeSearchWindow;
		CvRect mouthSearchWindow;
		CvRect rightEyeRect;
		CvRect leftEyeRect;
		vector<CvRect> mouthRects;
		CvRect mouthRect;
        FaceFeaturePoints ffp;

		if (faceRects.size() > 0)
		{
			CvRect faceRect = faceRects[0];
			if ( faceRect.width != -1 && faceRect.height != -1 )
			{
				CvSize eyeSize = cvSize(40, 30);

				/**
				 * Find right eye
				 */
				rightEyeSearchWindow = faceRect;
				rightEyeSearchWindow.height /= 4;
				rightEyeSearchWindow.y += rightEyeSearchWindow.height;
				rightEyeSearchWindow.width /= 2;
				cvSetImageROI(app.frame, rightEyeSearchWindow);
				vector<CvRect> eyeRects = detectObjects(app.frame, app.cascadeRightEye, app.memStorage, eyeSize);
				if (eyeRects.size() > 0)
				{
					rightEyeRect = eyeRects[0];
					rightEyeRect.x += rightEyeSearchWindow.x;
					rightEyeRect.y += rightEyeSearchWindow.y;

                    // Find right eye feature points
                    cvSetImageROI(grayscaleFrame, rightEyeRect);
                    ffp.rightEye = detectEyeFeaturePoints(grayscaleFrame, app.memStorage, "RE Contrast Stretch 1", "RE Contrast Stretch 2","RE Threshold", "RE Contour", "RE Feature Points");
                    cvResetImageROI(grayscaleFrame);
				}
				cvResetImageROI(app.frame);

				/**
				 * Find left eye
				 */
				leftEyeSearchWindow = faceRect;
				leftEyeSearchWindow.height /= 4;
				leftEyeSearchWindow.y += leftEyeSearchWindow.height;
				leftEyeSearchWindow.width /= 2;
				leftEyeSearchWindow.x += leftEyeSearchWindow.width;
				cvSetImageROI(app.frame, leftEyeSearchWindow);
				eyeRects = detectObjects(app.frame, app.cascadeLeftEye, app.memStorage, eyeSize);
				if (eyeRects.size() > 0)
				{
					leftEyeRect = eyeRects[0];
					leftEyeRect.x += leftEyeSearchWindow.x;
					leftEyeRect.y += leftEyeSearchWindow.y;

                    // Find left eye feature points
                    cvSetImageROI(grayscaleFrame, leftEyeRect);
                    ffp.leftEye = detectEyeFeaturePoints(grayscaleFrame, app.memStorage, "LE Contrast Stretch 1", "LE Contrast Stretch 2","LE Threshold", "LE Contour", "LE Feature Points");
                    cvResetImageROI(grayscaleFrame);
				}
				cvResetImageROI(app.frame);

				/**
				 * Find mouth
				 */
				CvPoint rightEyeMidPoint = getRectMidPoint(rightEyeRect);
				CvPoint leftEyeMidPoint = getRectMidPoint(leftEyeRect);
				double eyeDist = getDist(leftEyeMidPoint, rightEyeMidPoint);
				CvPoint mouthCenter = cvPoint(rightEyeMidPoint.x + eyeDist * 0.5, rightEyeMidPoint.y + eyeDist * 1.1);

				mouthSearchWindow = faceRect;
				mouthSearchWindow.height /= 2;
				mouthSearchWindow.y += mouthSearchWindow.height;

				cvSetImageROI(app.frame, mouthSearchWindow);
				mouthRects = detectObjects(app.frame, app.cascadeMouth, app.memStorage, cvSize(120, 60));
				if (mouthRects.size() > 0)
				{
					mouthRect = mouthRects[0];
					mouthRect.x += mouthSearchWindow.x;
					mouthRect.y += mouthSearchWindow.y;
				}
				cvResetImageROI(app.frame);
			}
		}
		double processingTime = toc();

		/**
		 * Draw object rects
		 */
		for (int i=0; i<faceRects.size(); i++)
		{
			drawRect(app.frame, faceRects[i], COL_RED);
		}
		drawRect(app.frame, rightEyeSearchWindow, COL_GREEN);
		drawRect(app.frame, leftEyeSearchWindow, COL_BLUE);
		drawRect(app.frame, rightEyeRect, COL_GREEN);
		drawRect(app.frame, leftEyeRect, COL_BLUE);
		// Draw eye mid-points
		drawCross(app.frame, getRectMidPoint(rightEyeRect));
		drawCross(app.frame, getRectMidPoint(leftEyeRect));
		// Draw eye-midpoint connection line
		CvPoint rightEyeMidPoint = getRectMidPoint(rightEyeRect);
		CvPoint leftEyeMidPoint = getRectMidPoint(leftEyeRect);
		cvLine(app.frame, rightEyeMidPoint, leftEyeMidPoint, COL_RED, 1);
		// Draw anthropometric eyebrow distance line (0.33% of eye mid-points distance)
		double eyeDist = getDist(leftEyeMidPoint, rightEyeMidPoint);
		cvLine(app.frame, rightEyeMidPoint, cvPoint(rightEyeMidPoint.x, rightEyeMidPoint.y - eyeDist * 0.33), COL_RED, 1);
		cvLine(app.frame, leftEyeMidPoint, cvPoint(leftEyeMidPoint.x, leftEyeMidPoint.y - eyeDist * 0.33), COL_RED, 1);
		// Draw nose tip
		CvPoint noseTip = cvPoint(rightEyeMidPoint.x + eyeDist * 0.5, rightEyeMidPoint.y + eyeDist * 0.6);
		drawCross(app.frame, noseTip);
		// Mouth
		CvPoint mouthCenter = cvPoint(rightEyeMidPoint.x + eyeDist * 0.5, rightEyeMidPoint.y + eyeDist * 1.1);
		drawCross(app.frame, mouthCenter);
		drawRect(app.frame, mouthSearchWindow, COL_BLUE);
		drawRect(app.frame, mouthRect, COL_LIME_GREEN);
        
        // Draw eye feature points
        drawCross(app.frame, ffp.rightEye.cornerLeft, COL_YELLOW);
        drawCross(app.frame, ffp.rightEye.cornerRight, COL_YELLOW);
        drawCross(app.frame, ffp.rightEye.upperLid, COL_YELLOW);
        drawCross(app.frame, ffp.rightEye.lowerLid, COL_YELLOW);
        drawCross(app.frame, ffp.leftEye.cornerLeft, COL_YELLOW);
        drawCross(app.frame, ffp.leftEye.cornerRight, COL_YELLOW);
        drawCross(app.frame, ffp.leftEye.upperLid, COL_YELLOW);
        drawCross(app.frame, ffp.leftEye.lowerLid, COL_YELLOW);

		// Print processing time for detection (after ROI reset!!)
		memset(buf, '\0', 255);
		sprintf(buf, "Detection Time: %0.3gms", processingTime);
		CvSize textBoundings;
		int ymin;
		cvGetTextSize(buf, &app.font, &textBoundings, &ymin);
		CvPoint org = cvPoint(0, app.frame->height - 8);
		cvRectangle(app.frame, cvPoint(org.x, org.y + ymin), cvPoint(org.x + textBoundings.width, org.y - textBoundings.height), cvScalar(0,0,0,0.5), CV_FILLED);
		cvPutText(app.frame, buf, cvPoint(0, app.frame->height - 8), &app.font, CV_RGB(0,255,0));		

		// Show the composited image
		cvShowImage(WINDOW_RESULT, app.frame );	

		// Control the program
        int key = cvWaitKey(10);

        if( (char) key == 27 )
		{
            app.isExiting = true;
		}

        // Automatically disable automatic image saving feature.
        g_autoSaveImages = 0;

		switch( (char) key )
		{
		// Print/Save image to file
		case 'p':
            g_autoSaveImages = time(NULL);
			saveImage(app.frame, WINDOW_RESULT);            
            break;
        // Toggle visual debugging
        case 'd':
            g_enableVisDebug = g_enableVisDebug ? false : true;
            break;
		}

		// Release the current frame (DON'T DO THIS IF CAPTURING!!!)
		if ( !app.capture )
		{			
			cvReleaseImage(&app.frame);
		}
    }

	// Cleanup OpenCV
	cvReleaseHaarClassifierCascade(&app.cascadeFace);
	cvReleaseHaarClassifierCascade(&app.cascadeMouth);
	cvReleaseHaarClassifierCascade(&app.cascadeLeftEye);
	cvReleaseHaarClassifierCascade(&app.cascadeRightEye);
	cvReleaseMemStorage(&app.memStorage);
	// Release the sample frame (either static image or first video frame) (DON'T DO THIS IF CAPTURING!!!)
	if ( !app.capture )
	{
		cvReleaseImage(&image);
	}
	cvReleaseCapture(&app.capture);
    cvReleaseImage(&grayscaleFrame);
	cvDestroyAllWindows();

    return 0;
}
