#!/bin/sh
ANDROID_NDK_ROOT=/Users/mitu217/Library/Android/sdk/ndk-bundle

$ANDROID_NDK_ROOT/ndk-build NDK_PROJECT_PATH=. NDK_APPLICATION_MK=Application.mk $*
mv libs/armeabi/libMobileTouchPlugin.so ../../Android/

rm -rf libs
rm -rf obj
