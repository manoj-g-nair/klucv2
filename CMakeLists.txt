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
    feature_point_detection_from_eye_region
    ${PROJECT_SOURCE_DIR}/anthropometric_eye/src/feature_point_detection_from_eye_region.cpp
    ${PROJECT_SOURCE_DIR}/common/src/common.cpp    
)
target_link_libraries(feature_point_detection_from_eye_region ${CVLIBS})

add_executable(
    feature_point_detection_from_mouth_region
    ${PROJECT_SOURCE_DIR}/anthropometric_mouth/src/feature_point_detection_from_mouth_region.cpp
    ${PROJECT_SOURCE_DIR}/common/src/common.cpp
)
target_link_libraries(feature_point_detection_from_mouth_region ${CVLIBS})

add_executable(
    facedetect_modesto_tips
    ${PROJECT_SOURCE_DIR}/facedetect_modesto_tips/src/facedetect_modesto_tips.cpp
    ${PROJECT_SOURCE_DIR}/common/src/common.cpp
)
target_link_libraries(facedetect_modesto_tips ${CVLIBS})

add_executable(
    facedetect_roi
    ${PROJECT_SOURCE_DIR}/facedetect_roi/src/facedetect_roi.cpp
	${PROJECT_SOURCE_DIR}/common/src/common.cpp
)
target_link_libraries(facedetect_roi ${CVLIBS})

add_executable(
    neural_network
    ${PROJECT_SOURCE_DIR}/neural_network/src/neural_network.cpp
    ${PROJECT_SOURCE_DIR}/common/src/common.cpp
)
target_link_libraries(neural_network ${CVLIBS})

