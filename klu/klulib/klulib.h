// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the KLULIB_EXPORTS
// symbol defined on the command line. this symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// KLULIB_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#include "klu_types.h"

extern "C" {

KLULIB_API int createCapture(void);

KLULIB_API void freeCapture(void);

KLULIB_API void queryCaptureImage(unsigned char ** data, int * width, int * height, int * nChannels, int * widthStep);

KLULIB_API int klu_initializeLibrary(void);

KLULIB_API int klu_deinitializeLibrary(void);

KLULIB_API int testStruct(KluTestStruct * p);

}
