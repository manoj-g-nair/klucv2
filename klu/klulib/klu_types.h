#ifndef KLU_TYPES_H_
#define KLU_TYPES_H_

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

#ifdef KLULIB_EXPORTS
#define KLULIB_API __declspec(dllexport)
#else
#define KLULIB_API __declspec(dllimport)
#endif

extern "C" {
    KLULIB_API typedef struct KluTestStruct
    {
        int x;
        int y;
    } KluTestStruct;

    KLULIB_API typedef enum
    {
        IdentityActivation = 0,
        SigmoidActivation = 1,
        GaussianActivation = 2
    } KluAnnActivation;

    KLULIB_API typedef struct KluProcessOptions
    {
        // TODO: (Ko) Add options here
    } KluProcessOptions;

    /**
    * \name ffp_datatypes Datatypes for Feature Points.
    * @{
    */

    /**
    * The \c KluEyeFeaturePoints struct contains the X-Y coordinates of
    * all the feature points for one eye. Coordinates are considered
    * to be image and not region coordinates.
    *
    * @author Konrad Kleine, Jens Lukowski
    */  
    KLULIB_API typedef struct KluEyeFeaturePoints
    {
        CvPoint center;
        CvPoint upperLid;
        CvPoint lowerLid;
        CvPoint cornerLeft; // from your point of view
        CvPoint cornerRight;// from your point of view
    } KluEyeFeaturePoints;

    /**
    * The \c KluMouthFeaturePoints struct contains the X-Y coordinates of
    * all the feature points for the mouth. Coordinates are considered
    * to be image and not region coordinates.
    *
    * @author Konrad Kleine, Jens Lukowski
    */  
    KLULIB_API typedef struct KluMouthFeaturePoints
    {
        CvPoint upperLipMiddle;
        CvPoint lowerLipMiddle;
        CvPoint upperLipRight;
        CvPoint lowerLipRight;
        CvPoint upperLipLeft;
        CvPoint lowerLipLeft;
        CvPoint cornerLeft; // from your point of view
        CvPoint cornerRight;// from your point of view
    } KluMouthFeaturePoints;

    /**
    * The \c KluFaceFeaturePoints struct combines all feature points for
    * eye, mouth etc. in one group.
    *
    * @author Konrad Kleine, Jens Lukowski
    */
    KLULIB_API typedef struct KluFaceFeaturePoints
    {
        KluEyeFeaturePoints leftEye;
        KluEyeFeaturePoints rightEye;
        KluMouthFeaturePoints mouth;
    } KluFaceFeaturePoints;

    /**
    * @}
    */
}

#endif
