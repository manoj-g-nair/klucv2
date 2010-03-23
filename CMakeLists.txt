project(klucv2)

CMAKE_MINIMUM_REQUIRED(VERSION 2.6)
if(COMMAND cmake_policy)
      cmake_policy(SET CMP0003 NEW)
endif(COMMAND cmake_policy)

include_directories(
	/usr/local/include
	/usr/local/include/opencv
)

set(CVLIBS cv200 cxcore200 cvaux200 ml200 highgui200)

add_executable(
    facedetect_roi
    ${PROJECT_SOURCE_DIR}/facedetect_roi/src/facedetect_roi.cpp
	${PROJECT_SOURCE_DIR}/common/src/common.cpp
)
target_link_libraries(facedetect_roi ${CVLIBS})

