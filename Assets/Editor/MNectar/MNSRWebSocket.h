//
//   Copyright 2012 Square Inc.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//

#import <Foundation/Foundation.h>
#import <Security/SecCertificate.h>

typedef NS_ENUM(NSInteger, MNSRReadyState) {
    MNSR_CONNECTING   = 0,
    MNSR_OPEN         = 1,
    MNSR_CLOSING      = 2,
    MNSR_CLOSED       = 3,
};

typedef enum MNSRStatusCode : NSInteger {
    // 0–999: Reserved and not used.
    MNSRStatusCodeNormal = 1000,
    MNSRStatusCodeGoingAway = 1001,
    MNSRStatusCodeProtocolError = 1002,
    MNSRStatusCodeUnhandledType = 1003,
    // 1004 reserved.
    MNSRStatusNoStatusReceived = 1005,
    MNSRStatusCodeAbnormal = 1006,
    MNSRStatusCodeInvalidUTF8 = 1007,
    MNSRStatusCodePolicyViolated = 1008,
    MNSRStatusCodeMessageTooBig = 1009,
    MNSRStatusCodeMissingExtension = 1010,
    MNSRStatusCodeInternalError = 1011,
    MNSRStatusCodeServiceRestart = 1012,
    MNSRStatusCodeTryAgainLater = 1013,
    // 1014: Reserved for future use by the WebSocket standard.
    MNSRStatusCodeTLSHandshake = 1015,
    // 1016–1999: Reserved for future use by the WebSocket standard.
    // 2000–2999: Reserved for use by WebSocket extensions.
    // 3000–3999: Available for use by libraries and frameworks. May not be used by applications. Available for registration at the IANA via first-come, first-serve.
    // 4000–4999: Available for use by applications.
} MNSRStatusCode;

@class MNSRWebSocket;

extern NSString *const MNSRWebSocketErrorDomain;
extern NSString *const MNSRHTTPResponseErrorKey;

#pragma mark - MNSRWebSocketDelegate

@protocol MNSRWebSocketDelegate;

#pragma mark - MNSRWebSocket

@interface MNSRWebSocket : NSObject <NSStreamDelegate>

@property (nonatomic, weak) id <MNSRWebSocketDelegate> delegate;

@property (nonatomic, readonly) MNSRReadyState readyState;
@property (nonatomic, readonly, retain) NSURL *url;


@property (nonatomic, readonly) CFHTTPMessageRef receivedHTTPHeaders;

// Optional array of cookies (NSHTTPCookie objects) to apply to the connections
@property (nonatomic, readwrite) NSArray * requestCookies;

// This returns the negotiated protocol.
// It will be nil until after the handshake completes.
@property (nonatomic, readonly, copy) NSString *protocol;

// Protocols should be an array of strings that turn into Sec-WebSocket-Protocol.
- (id)initWithURLRequest:(NSURLRequest *)request protocols:(NSArray *)protocols allowsUntrustedSSLCertificates:(BOOL)allowsUntrustedSSLCertificates;
- (id)initWithURLRequest:(NSURLRequest *)request protocols:(NSArray *)protocols;
- (id)initWithURLRequest:(NSURLRequest *)request;

// Some helper constructors.
- (id)initWithURL:(NSURL *)url protocols:(NSArray *)protocols allowsUntrustedSSLCertificates:(BOOL)allowsUntrustedSSLCertificates;
- (id)initWithURL:(NSURL *)url protocols:(NSArray *)protocols;
- (id)initWithURL:(NSURL *)url;

// Delegate queue will be dispatch_main_queue by default.
// You cannot set both OperationQueue and dispatch_queue.
- (void)setDelegateOperationQueue:(NSOperationQueue*) queue;
- (void)setDelegateDispatchQueue:(dispatch_queue_t) queue;

// By default, it will schedule itself on +[NSRunLoop SR_networkRunLoop] using defaultModes.
- (void)scheduleInRunLoop:(NSRunLoop *)aRunLoop forMode:(NSString *)mode;
- (void)unscheduleFromRunLoop:(NSRunLoop *)aRunLoop forMode:(NSString *)mode;

// SRWebSockets are intended for one-time-use only.  Open should be called once and only once.
- (void)open;

- (void)close;
- (void)closeWithCode:(NSInteger)code reason:(NSString *)reason;

// Send a UTF8 String or Data.
- (void)send:(id)data;

// Send Data (can be nil) in a ping message.
- (void)sendPing:(NSData *)data;

@end

#pragma mark - MNSRWebSocketDelegate

@protocol MNSRWebSocketDelegate <NSObject>

// message will either be an NSString if the server is using text
// or NSData if the server is using binary.
- (void)webSocket:(MNSRWebSocket *)webSocket didReceiveMessage:(id)message;

@optional

- (void)webSocketDidOpen:(MNSRWebSocket *)webSocket;
- (void)webSocket:(MNSRWebSocket *)webSocket didFailWithError:(NSError *)error;
- (void)webSocket:(MNSRWebSocket *)webSocket didCloseWithCode:(NSInteger)code reason:(NSString *)reason wasClean:(BOOL)wasClean;
- (void)webSocket:(MNSRWebSocket *)webSocket didReceivePong:(NSData *)pongPayload;

@end

#pragma mark - NSURLRequest (MNSRCertificateAdditions)

@interface NSURLRequest (MNSRCertificateAdditions)

@property (nonatomic, retain, readonly) NSArray *MNSR_SSLPinnedCertificates;

@end

#pragma mark - NSMutableURLRequest (MNSRCertificateAdditions)

@interface NSMutableURLRequest (MNSRCertificateAdditions)

@property (nonatomic, retain) NSArray *MNSR_SSLPinnedCertificates;

@end

#pragma mark - NSRunLoop (MNSRWebSocket)

@interface NSRunLoop (MNSRWebSocket)

+ (NSRunLoop *)MNSR_networkRunLoop;

@end
