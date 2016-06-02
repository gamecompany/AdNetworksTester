#import <UIKit/UIKit.h>
#import "MNWebView.h"

#ifndef MN_AUDIO_DEFAULT_UNMUTED
#define MN_AUDIO_DEFAULT_UNMUTED NO
#endif

typedef enum {
    MNStreamClosed = 0,
    MNStreamConnecting,
    MNStreamOpen,
    MNStreamClosing,
    MNStreamReconnecting
} MNStreamStatus;

@interface MNStream : NSObject

@property (nonatomic, assign, readonly) MNStreamStatus status;

- (instancetype)initWithWebView:(MNWebView *)webView;
- (BOOL)webView:(MNWebView *)webView shouldLoadWithRequest:(NSURLRequest *)request;

- (void)stop;

@end
