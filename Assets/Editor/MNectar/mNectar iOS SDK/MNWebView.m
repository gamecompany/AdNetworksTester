#import "MNWebView.h"
#import "MNStream.h"

@interface WKWebView(SynchronousEvaluateJavaScript)

- (NSString *)stringByEvaluatingJavaScript:(NSString *)script;

@end

@implementation WKWebView(SynchronousEvaluateJavaScript)

- (NSString *)stringByEvaluatingJavaScript:(NSString *)script
{
    __block BOOL finished = NO;
    __block NSString *resultString = nil;
    
    [self evaluateJavaScript:script completionHandler:^(id result, NSError *error) {
        finished = YES;

        if (error == nil) {
            if (result != nil) {
                resultString = [NSString stringWithFormat:@"%@", result];
            }
        } else {
            NSLog(@"evaluateJavaScript error : %@", error.localizedDescription);
        }
    }];
    
    while (!finished) {
        [[NSRunLoop currentRunLoop] runMode:NSDefaultRunLoopMode beforeDate:[NSDate dateWithTimeIntervalSinceNow:0]];
    }
    
    return resultString;
}

@end

@interface MNWebView () <UIWebViewDelegate, WKNavigationDelegate>

@property (nonatomic, strong) UIView *webView;
@property (nonatomic, strong) MNStream *stream;

@end

@implementation MNWebView

- (instancetype)initWithFrame:(CGRect)frame
{
    self = [super initWithFrame:frame];
    if (self) {
        if (NSClassFromString(@"WKWebView")) {
            WKWebViewConfiguration *config = [[WKWebViewConfiguration alloc] init];
            [config setAllowsInlineMediaPlayback:YES];
            [config setMediaPlaybackRequiresUserAction:NO];

            WKWebView *webView = [[WKWebView alloc] initWithFrame:frame configuration:config];
            _webView = webView;
            [[webView scrollView] setScrollEnabled:NO];
            [webView setNavigationDelegate:self];
        } else {
            UIWebView *webView = [[UIWebView alloc] initWithFrame:frame];
            _webView = webView;
            [[webView scrollView] setScrollEnabled:NO];
            [webView setAllowsInlineMediaPlayback:YES];
            [webView setMediaPlaybackRequiresUserAction:NO];
            [webView setDelegate:self];
        }

        [_webView setBackgroundColor:[UIColor clearColor]];
        [_webView setOpaque:NO];
        
        [self addSubview:_webView];

        _stream = [[MNStream alloc] initWithWebView:self];
    }

    return self;
}

- (void)dealloc
{
    if ([_webView isKindOfClass:NSClassFromString(@"WKWebView")]) {
        [(WKWebView *)_webView setNavigationDelegate:nil];
    } else {
        [(UIWebView *)_webView setDelegate:nil];
    }

    [_stream stop];
}

- (void)setFrame:(CGRect)frame
{
    [super setFrame:frame];

    [_webView setFrame:frame];
}

- (UIScrollView *)scrollView
{
    if ([_webView isKindOfClass:NSClassFromString(@"WKWebView")]) {
        return [(WKWebView *)_webView scrollView];
    } else {
        return [(UIWebView *)_webView scrollView];
    }
}

- (void)setScalesPageToFit:(BOOL)scale
{
    if (![_webView isKindOfClass:NSClassFromString(@"WKWebView")]) {
        [(UIWebView *)_webView setScalesPageToFit:scale];
    }
}

- (NSString *)stringByEvaluatingJavaScriptFromString:(NSString *)string
{
    if ([_webView isKindOfClass:NSClassFromString(@"WKWebView")]) {
        return [(WKWebView *)_webView stringByEvaluatingJavaScript:string];
    } else {
        return [(UIWebView *)_webView stringByEvaluatingJavaScriptFromString:string];
    }
}

- (void)evaluateJavaScriptFromString:(NSString *)string
{
    if ([_webView isKindOfClass:NSClassFromString(@"WKWebView")]) {
        [(WKWebView *)_webView evaluateJavaScript:string completionHandler:nil];
    } else {
        [(UIWebView *)_webView stringByEvaluatingJavaScriptFromString:string];
    }
}

- (void)loadHTML:(NSString *)html baseURL:(NSURL *)baseURL
{
    if ([_webView isKindOfClass:NSClassFromString(@"WKWebView")]) {
        [(WKWebView *)_webView loadHTMLString:html baseURL:baseURL];
    } else {
        [(UIWebView *)_webView loadHTMLString:html baseURL:baseURL];
    }
}

#pragma mark - UIWebViewDelegate

- (void)webView:(UIWebView *)webView didFailLoadWithError:(NSError *)error
{
    [_delegate webView:self didFailLoadWithError:error];
}

- (BOOL)webView:(UIWebView *)webView shouldStartLoadWithRequest:(NSURLRequest *)request navigationType:(UIWebViewNavigationType)navigationType
{
    return [_stream webView:self shouldLoadWithRequest:request] && [_delegate webView:self shouldStartLoadWithRequest:request];
}

#pragma mark - WKNavigationDelegate

- (void)webView:(WKWebView *)webView didFailNavigation:(WKNavigation *)navigation withError:(NSError *)error{
    [_delegate webView:self didFailLoadWithError:error];
}

- (void)webView:(WKWebView *)webView decidePolicyForNavigationAction:(WKNavigationAction *)navigationAction decisionHandler:(void (^)(WKNavigationActionPolicy))decisionHandler
{
    if ([_stream webView:self shouldLoadWithRequest:[navigationAction request]] && [_delegate webView:self shouldStartLoadWithRequest:[navigationAction request]]) {
        decisionHandler(WKNavigationActionPolicyAllow);
    } else {
        decisionHandler(WKNavigationActionPolicyCancel);
    }
}

#pragma mark - generic webview methods

- (void)loadRequest:(NSURLRequest *)request
{
    if ([_webView isKindOfClass:NSClassFromString(@"WKWebView")]) {
        [(WKWebView *)_webView loadRequest:request];
    } else {
        [(UIWebView *)_webView loadRequest:request];
    }
}

- (void)reload
{
    if ([_webView isKindOfClass:NSClassFromString(@"WKWebView")]) {
        [(WKWebView *)_webView reload];
    } else {
        [(UIWebView *)_webView reload];
    }
}

- (void)stopLoading
{
    if ([_webView isKindOfClass:NSClassFromString(@"WKWebView")]) {
        [(WKWebView *)_webView stopLoading];
    } else {
        [(UIWebView *)_webView stopLoading];
    }
}

- (void)goBack
{
    if ([_webView isKindOfClass:NSClassFromString(@"WKWebView")]) {
        [(WKWebView *)_webView goBack];
    } else {
        [(UIWebView *)_webView goBack];
    }
}

- (void)goForward
{
    if ([_webView isKindOfClass:NSClassFromString(@"WKWebView")]) {
        [(WKWebView *)_webView goForward];
    } else {
        [(UIWebView *)_webView goForward];
    }
}

- (BOOL)canGoBack
{
    if ([_webView isKindOfClass:NSClassFromString(@"WKWebView")]) {
        return [(WKWebView *)_webView canGoBack];
    } else {
        return [(UIWebView *)_webView canGoBack];
    }
}

- (BOOL)canGoForward
{
    if ([_webView isKindOfClass:NSClassFromString(@"WKWebView")]) {
        return [(WKWebView *)_webView canGoForward];
    } else {
        return [(UIWebView *)_webView canGoForward];
    }
}

- (BOOL)isLoading
{
    if ([_webView isKindOfClass:NSClassFromString(@"WKWebView")]) {
        return [(WKWebView *)_webView isLoading];
    } else {
        return [(UIWebView *)_webView isLoading];
    }
}

@end
