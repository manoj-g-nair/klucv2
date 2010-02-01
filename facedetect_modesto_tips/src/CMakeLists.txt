project(facedetect_modesto_tips)

CMAKE_MINIMUM_REQUIRED(VERSION 2.6)
if(COMMAND cmake_policy)
      cmake_policy(SET CMP0003 NEW)
endif(COMMAND cmake_policy)

include_directories(
	/usr/local/include
	/usr/local/include/opencv
)

add_executable(
    facedetect_modesto_tips
    facedetect_modesto_tips.cpp
    ../../common/src/common.cpp
)

target_link_libraries(
    facedetect_modesto_tips
    cv200 cxcore200 ml200 highgui200 cvaux200 cv200
)

#set_target_properties(feature_point_detection_from_eye_region PROPERTIES PROJECT_LABEL "Face Detection with Tips from Modesto")

