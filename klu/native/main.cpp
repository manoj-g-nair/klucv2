/**
 * Copyright (C) 2010, Konrad Kleine and Jens Lukowski, all rights reserved.
 */

#include "../klulib/klulib.h"
#include <iostream>
#include <cstdio>
#include <cstdlib>

using namespace std;

/**
 * The \c KLU_CALL(F) macro is a shortcut for calling, checking and logging
 * a klulib function or any other function that returns a \c true if everything went well
 */
#define KLU_CALL(F) \
    fprintf(stderr, "Executing \"" #F "\"..."); \
    if ( (F) ) \
    { \
        fprintf(stderr, "FAILED\n"); \
        exit(1); \
    }\
    fprintf(stderr, "OK\n");

/**
 * The exit handler simply asks the user to press enter before exiting.
 */
void ExitHandler(void)
{
    fprintf(stderr, "Shutting down the program now! Press ENTER to continue!\n");
    getchar();
}

/**
 * The main program.
 */
int main(int argc, char * argv[])
{
    atexit(ExitHandler);

    KLU_CALL( klu_initializeLibrary() )

    

    KLU_CALL( klu_deinitializeLibrary() )

    return 0;
}
