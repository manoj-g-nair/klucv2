project(neural_network)

CMAKE_MINIMUM_REQUIRED(VERSION 2.6)
if(COMMAND cmake_policy)
      cmake_policy(SET CMP0003 NEW)
endif(COMMAND cmake_policy)

include_directories(
	/usr/local/include
	/usr/local/include/opencv
)

add_executable(
    neural_network
    neural_network.cpp
    ../../common/src/common.cpp
)

target_link_libraries(
    neural_network
    cv cxcore cvaux ml highgui
)

#set_target_properties(feature_point_detection_from_eye_region PROPERTIES PROJECT_LABEL "Neural Network")
