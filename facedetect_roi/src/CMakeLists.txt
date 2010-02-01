project(facedetect_roi)

CMAKE_MINIMUM_REQUIRED(VERSION 2.6)
if(COMMAND cmake_policy)
      cmake_policy(SET CMP0003 NEW)
endif(COMMAND cmake_policy)

include_directories(
	/usr/local/include
	/usr/local/include/opencv
)

add_executable(
    facedetect_roi
    facedetect_roi.cpp
)

target_link_libraries(
    facedetect_roi
    cv200 cxcore200 ml200 highgui200 cvaux200 cv200
)