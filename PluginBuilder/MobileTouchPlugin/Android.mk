include $(CLEAR_VARS)

LOCAL_ARM_MODE  := arm
LOCAL_PATH      := $(NDK_PROJECT_PATH)
LOCAL_MODULE    := libMobileTouchPlugin
LOCAL_CFLAGS    := -Werror
LOCAL_SRC_FILES := MobileTouchPlugin.cpp
LOCAL_LDLIBS    := -llog

include $(BUILD_SHARED_LIBRARY)
