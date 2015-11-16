#ifndef TouchPlugin_h
#define TouchPlugin_h

extern "C" {
    void initPlugin(const char *gameObject, const char* callback);
}

#import <Foundation/Foundation.h>
@interface TouchPlugin : NSObject
+(void) touchedCallback:(NSString *)message;
@end

#endif