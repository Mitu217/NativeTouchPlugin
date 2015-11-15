#import "UnityView+Touch.h"
#import "TouchPlugin.h"

@implementation UnityView (Touch)

- (void)touchesBegan:(NSSet *)touches withEvent:(UIEvent *)event {
    [TouchPlugin touchedCallback:[self parseTouchData:touches action:@"began"]];
    UnitySendTouchesBegin(touches, event);
}
- (void)touchesEnded:(NSSet *)touches withEvent:(UIEvent *)event {
    [TouchPlugin touchedCallback:[self parseTouchData:touches action:@"ended"]];
    UnitySendTouchesEnded(touches, event);
}
- (void)touchesCancelled:(NSSet*)touches withEvent:(UIEvent*)event {
    [TouchPlugin touchedCallback:[self parseTouchData:touches action:@"cancelled"]];
    UnitySendTouchesCancelled(touches, event);
}
- (void)touchesMoved:(NSSet*)touches withEvent:(UIEvent*)event {
    [TouchPlugin touchedCallback:[self parseTouchData:touches action:@"moved"]];
    UnitySendTouchesMoved(touches, event);
}

- (NSString *) parseTouchData:(NSSet *)touches action:(NSString*)action{
    NSMutableString* result = [NSMutableString	string];
    [result appendString:[NSString stringWithFormat:@"%@,", action]];
    [result appendString:[NSString stringWithFormat:@"%d,", (int)[touches count]]];
    NSString* timestamp;
    NSMutableString* param = [NSMutableString string];
    for (UITouch *touch in touches) {
        timestamp = [NSString stringWithFormat:@"%d", (int)([touch timestamp]*1000)];
        CGPoint point = [touch locationInView:self];
        [param appendString:[NSString stringWithFormat:@"%d,%f,%f,%f,%f,",
                             (int)[touch hash], point.x, point.y, [touch majorRadius], [touch force]]];
    }
    [result appendString:[NSString stringWithFormat:@"%@,%@", timestamp, param]];
    return result;
}
@end