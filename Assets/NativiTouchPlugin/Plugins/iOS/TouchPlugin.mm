#import "TouchPlugin.h"

extern "C" {
    NSString* mGameObject;
    NSString* mCallback;
    void initPlugin(const char* gameObject, const char* callback) {
        mGameObject = [NSString stringWithUTF8String:gameObject];
        mCallback = [NSString stringWithUTF8String:callback];
    }
}

@implementation TouchPlugin
+ (void) touchedCallback:(NSString *)message {
    UnitySendMessage([mGameObject UTF8String], [mCallback UTF8String], [message UTF8String]);
}

@end