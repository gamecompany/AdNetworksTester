#import <UIKit/UIKit.h>
#import <WebKit/WebKit.h>

@class MNWebView;

@protocol MNWebViewDelegate <NSObject>

- (BOOL)webView:(MNWebView *)webView shouldStartLoadWithRequest:(NSURLRequest *)request;
- (void)webView:(MNWebView *)webView didFailLoadWithError:(NSError *)error;

@end

@interface MNWebView : UIView

@property (nonatomic, weak) id<MNWebViewDelegate> delegate;

- (instancetype)initWithFrame:(CGRect)frame;

- (void)setFrame:(CGRect)frame;
- (UIScrollView *)scrollView;
- (void)setScalesPageToFit:(BOOL) scale;
- (void)evaluateJavaScriptFromString:(NSString *)string;
- (NSString *)stringByEvaluatingJavaScriptFromString:(NSString *)string;
- (void)loadHTML:(NSString *)html baseURL:(NSURL *)baseURL;

- (void)loadRequest:(NSURLRequest *)request;
- (void)reload;
- (void)stopLoading;
- (void)goBack;
- (void)goForward;
- (BOOL)canGoBack;
- (BOOL)canGoForward;
- (BOOL)isLoading;

@end
