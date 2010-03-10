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


    KLULIB_API typedef enum
    {
        IdentityActivation = 0,
        SigmoidActivation = 1,
        GaussianActivation = 2
    } KluAnnActivation;

    KLULIB_API typedef enum
    {
        MaxIterationTermination = 0,
        EpsilonTermination = 1        
    } KluTrainingTerminationType;    

    KLULIB_API typedef struct KluTerminationCriteria
    {
        int terminationType;
        float epsilon;
        int maxIteration;
    } KluTerminationCriteria;

    KLULIB_API typedef enum
    {
        // LeCun, L. Bottou, G.B. Orr and K.-R. Muller,
        // “Efficient backprop”,
        // in Neural Networks—Tricks of the Trade, Springer Lecture Notes in Computer Sciences 1524, pp.5-50, 1998.
        BackpropAlgorithm = 0,
        // Riedmiller and H. Braun,
        // “A Direct Adaptive Method for Faster Backpropagation Learning: The RPROP Algorithm”,
        // Proc. ICNN, San Francisco (1993)
        RpropAlgorithm = 1        
    } KluTrainingAlgorithm;  

    KLULIB_API typedef struct KluTrainOptions
    {
        KluTerminationCriteria termination;
        KluTrainingAlgorithm algorithm;

        // backpropagation parameters
        double backpropDeltaWeightScale;    // default = 0.1
        double backpropMomentumScale;       // default = 0.1

        // rprop parameters
        double rpropDeltaWeight0;           // default = 1
        double rpropDeltaWeightPlus;        // default = 1.2
        double rpropDeltaWeightMinus;       // default = 0.5
        double rpropDeltaWeightMin;         // default = FLT_EPSILON (smallest floating point number such that (1.0+FLT_EPSILON != 1.0)
        double rpropDeltaWeightMax;         // default = 50
    } KluTrainOptions;

    KLULIB_API typedef struct KluProcessOptions
    {
        int drawAnthropometricPoints;
        int drawSearchRectangles;
        int drawFaceRectangle;
        int drawDetectionTime;
        int drawFeaturePoints;
        int doEyeProcessing;
        int doMouthProcessing;
        int doVisualDebug;
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
        CvRect faceRegion;
    } KluFaceFeaturePoints;

    /**
    * @}
    */
}

#endif
