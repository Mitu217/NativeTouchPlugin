#ifndef TouchPlugin_h
#define TouchPlugin_h

extern "C" {
    void initPlugin(const char *gameObject, const char* callback);
}

@interface TouchPlugin : NSObject
+(void) touchedCallback:(NSString *)message;
@end

#endif