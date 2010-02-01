project(anthropometric_mouth)

CMAKE_MINIMUM_REQUIRED(VERSION 2.6)
if(COMMAND cmake_policy)
      cmake_policy(SET CMP0003 NEW)
endif(COMMAND cmake_policy)

include_directories(
	/usr/local/include
	/usr/local/include/opencv
)

add_executable(
    feature_point_detection_from_mouth_region
    feature_point_detection_from_mouth_region.cpp
    ../../common/src/common.cpp
)

target_link_libraries(
    feature_point_detection_from_mouth_region
    cv cxcore cvaux ml highgui
)

#set_target_properties(feature_point_detection_from_eye_region PROPERTIES PROJECT_LABEL "Feature Point Detection from Mouth Region")
