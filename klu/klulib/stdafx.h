/**
 * Copyright (C) 2010, Konrad Kleine and Jens Lukowski, all rights reserved.
 *
 * $Id$
 */

// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#ifndef KLULIB_STDAFX_H_
#define KLULIB_STDAFX_H_

#include "targetver.h"

#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
// Windows Header Files:
#include <windows.h>

// TODO: reference additional headers your program requires here

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

#endif /* KLULIB_STDAFX_H_ */