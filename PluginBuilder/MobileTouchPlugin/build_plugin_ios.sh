#!/bin/sh

XCODE_PROJECT_NAME=MobileTouchPlugin
XCODE_PROJECT_PATH=../$XCODE_PROJECT_NAME.xcodeproj

xcodebuild -project $XCODE_PROJECT_PATH -sdk iphoneos -configuration Release -arch armv7 -arch armv7s -arch arm64 clean build
xcodebuild -project $XCODE_PROJECT_PATH -sdk iphonesimulator -configuration Release -arch i386 -arch x86_64 clean build
xcrun -sdk iphoneos lipo -create ../build/Release-iphonesimulator/lib$XCODE_PROJECT_NAME.a ../build/Release-iphoneos/lib$XCODE_PROJECT_NAME.a -output lib$XCODE_PROJECT_NAME.a

mv lib$XCODE_PROJECT_NAME.a ../../iOS/

rm -rf ../build
