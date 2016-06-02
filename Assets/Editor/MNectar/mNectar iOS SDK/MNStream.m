#import "MNStream.h"
#import "MNSRWebSocket.h"
#import <AVFoundation/AVFoundation.h>

#define MN_AUDIO_MAX_LAG_MS 600

@interface MNStream ()  <MNSRWebSocketDelegate>

@property (nonatomic, weak) MNWebView *webView;
@property (nonatomic, strong) MNSRWebSocket *webSocket;
@property (nonatomic, strong) NSURL *webSocketURL;
@property (nonatomic, assign) int webSocketConnectTimeout;
@property (nonatomic, strong) NSTimer *webSocketConnectTimer;
@property (nonatomic, assign) int webSocketIdleTimeout;
@property (nonatomic, assign) BOOL webSocketIsIdle;
@property (nonatomic, strong) NSTimer *webSocketIdleTimer;

@property (nonatomic, assign) CGRect domPosition;
@property (nonatomic, assign) NSInteger domOrientation;

@property (nonatomic, assign) int videoOrientation;
@property (nonatomic, strong) NSData *videoSequenceParameterSet;
@property (nonatomic, strong) NSData *videoPictureParameterSet;
@property (nonatomic, assign) CMVideoFormatDescriptionRef videoFormatDescription;
@property (nonatomic, strong) AVSampleBufferDisplayLayer *videoLayer;

@property (nonatomic, assign) BOOL audioIsUnmuted;
@property (nonatomic, assign) int audioMaxLagMilliseconds;
@property (nonatomic, assign) BOOL audioIsRunning;
@property (nonatomic, assign) CFMutableArrayRef audioBuffer;
@property (nonatomic, assign) AudioFileStreamID audioParser;
@property (nonatomic, assign) AudioStreamBasicDescription *audioStreamDescription;
@property (nonatomic, assign) AudioQueueRef audioQueue;

@end

@implementation MNStream

- (instancetype)initWithWebView:(MNWebView *)webView;
{
    if (self = [super init]) {
        _webView = webView;
        _audioIsUnmuted = MN_AUDIO_DEFAULT_UNMUTED;
        _audioMaxLagMilliseconds = MN_AUDIO_MAX_LAG_MS;
        
        [webView evaluateJavaScriptFromString:[[NSString alloc] initWithContentsOfFile:[[NSBundle mainBundle] pathForResource:@"MNStream.bundle/mnstreamjs" ofType:nil] encoding:NSUTF8StringEncoding error:nil]];
    }
    
    return self;
}

- (void)dealloc
{
    [self stop];
}

- (BOOL)webView:(MNWebView *)webView shouldLoadWithRequest:(NSURLRequest *)request
{
    NSURL *url = [request URL];

    if ([[url scheme] isEqualToString:@"mnstream"]) {
        NSString *command = [url host];
        NSMutableDictionary *arguments = [NSMutableDictionary dictionary];

        for (NSString *component in [[url query] componentsSeparatedByString:@"&"]) {
            NSArray *nameValuePair = [component componentsSeparatedByString:@"="];

            if ([nameValuePair count]) {
                NSString *name = [nameValuePair objectAtIndex:0];
                NSString *value = nil;

                if ([nameValuePair count] > 1) {
                    value = [[[nameValuePair objectAtIndex:1] stringByReplacingOccurrencesOfString:@"+" withString:@" "] stringByReplacingPercentEscapesUsingEncoding:NSUTF8StringEncoding];
                }

                [arguments setObject:value forKey:name];
            }
        }

        if ([command isEqualToString:@"open"]) {
            _webSocketConnectTimeout = [[[[NSNumberFormatter alloc] init] numberFromString:arguments[@"ct"]] intValue];
            _webSocketIdleTimeout = [[[[NSNumberFormatter alloc] init] numberFromString:arguments[@"it"]] intValue];

            NSString *url = arguments[@"url"];

            if (url) {
                _webSocketURL = [NSURL URLWithString:url];

                [self open];
            }
        } else if ([command isEqualToString:@"close"]) {
            [self close];
        } else if ([command isEqualToString:@"position"]) {
            float offsetX = [[[[NSNumberFormatter alloc] init] numberFromString:arguments[@"x"]] floatValue];
            float offsetY = [[[[NSNumberFormatter alloc] init] numberFromString:arguments[@"y"]] floatValue];
            float width = [[[[NSNumberFormatter alloc] init] numberFromString:arguments[@"w"]] floatValue];
            float height = [[[[NSNumberFormatter alloc] init] numberFromString:arguments[@"h"]] floatValue];
        
            _domPosition = CGRectMake(offsetX, offsetY, width, height);
            _domOrientation = [[[[NSNumberFormatter alloc] init] numberFromString:arguments[@"uo"]] intValue];
            _videoOrientation = [[[[NSNumberFormatter alloc] init] numberFromString:arguments[@"vo"]] intValue];
            
            [self position];
        } else if ([command isEqualToString:@"reconnect"]) {
            _webSocketConnectTimeout = [[[[NSNumberFormatter alloc] init] numberFromString:arguments[@"ct"]] intValue];
            _webSocketIdleTimeout = [[[[NSNumberFormatter alloc] init] numberFromString:arguments[@"it"]] intValue];

            NSString *url = arguments[@"url"];
            
            if (url) {
                _webSocketURL = [NSURL URLWithString:url];
            }
            
            if (_webSocketURL) {
                [self reconnect];
            }
        } else if ([command isEqualToString:@"mute"]) {
            _audioIsUnmuted = NO;
        } else if ([command isEqualToString:@"unmute"]) {
            _audioIsUnmuted = YES;
        }
        
        return NO;
    }
    
    return YES;
}

- (void)open
{
    if (_status == MNStreamClosed) {
        [self start];
    }
}

- (void)start
{
    if (_status == MNStreamClosed || _status == MNStreamReconnecting) {
        _webSocket = [[MNSRWebSocket alloc] initWithURL:_webSocketURL];

        [_webSocket setDelegate:self];

        if (_status == MNStreamClosed) {
            _status = MNStreamConnecting;
        }

        [_webSocket open];

        [self webSocketStartConnectTimer];
    }
}

- (void)close
{
    if (_status != MNStreamClosed) {
        _status = MNStreamClosing;
    
        [self stop];
    }
}

- (void)reconnect
{
    if (_status == MNStreamOpen || _status == MNStreamClosed) {
        _status = MNStreamReconnecting;

        [self stop];
        [self start];
    }
}

- (void)mute
{
    _audioIsUnmuted = NO;
}

- (void)unmute
{
    _audioIsUnmuted = YES;
}

- (void)render:(NSData *)data
{
    if (_status == MNStreamConnecting || _status == MNStreamReconnecting) {
        if (_status == MNStreamConnecting) {
            [_webView evaluateJavaScriptFromString:[NSString stringWithFormat:@"mnstream._onopen();"]];
        }

        _status = MNStreamOpen;
    }

    uint8_t *bytes = (uint8_t *)[data bytes];
    size_t size = [data length];
    
    if (size >= 4 && (bytes[0] == 0 && bytes[1] == 0 && bytes[2] == 0 && bytes[3] == 1)) {
        [self renderVideo:data];
    } else if (size >= 1 && bytes[0] == 0xff) {
        [self renderAudio:data];
    }
}

- (void)stop
{
    if (_status == MNStreamClosed) {
        return;
    }

    if (_webSocketConnectTimer) {
        [_webSocketConnectTimer invalidate];
        
        _webSocketConnectTimer = nil;
    }

    if (_webSocketIdleTimer) {
        [_webSocketIdleTimer invalidate];
        
        _webSocketIdleTimer = nil;
    }

    if (_webSocket) {
        [_webSocket setDelegate:nil];
        [_webSocket close];

        _webSocket = nil;
    }

    _videoSequenceParameterSet = nil;
    _videoPictureParameterSet = nil;

    if (_videoFormatDescription) {
        CFRelease(_videoFormatDescription);
    
        _videoFormatDescription = nil;
    }

    if (_videoLayer) {
        [_videoLayer removeFromSuperlayer];
        
        _videoLayer = nil;
    }
    
    if (_audioParser) {
        AudioFileStreamClose(_audioParser);
        
        _audioParser = NULL;
    }

    if (_audioStreamDescription) {
        free(_audioStreamDescription);

        _audioStreamDescription = NULL;
    }

    if (_audioQueue) {
        AudioQueueDispose(_audioQueue, YES);

        _audioQueue = NULL;
    }

    if (_status == MNStreamConnecting) {
        [_webView evaluateJavaScriptFromString:[NSString stringWithFormat:@"mnstream._onfailed();"]];
    }

    if (_status == MNStreamOpen) {
        if (_webSocketIsIdle) {
            [_webView evaluateJavaScriptFromString:[NSString stringWithFormat:@"mnstream._onidletimeout();"]];
        } else {
            [_webView evaluateJavaScriptFromString:[NSString stringWithFormat:@"mnstream._onerror();"]];
        }
    }

    if (_status == MNStreamClosing) {
        [_webView evaluateJavaScriptFromString:[NSString stringWithFormat:@"mnstream._onclose();"]];
    }

    if (_status != MNStreamReconnecting) {
        _status = MNStreamClosed;
    }
}

- (void)position
{
    if (_videoLayer) {
        CGRect frame = CGRectMake(_domPosition.origin.x, _domPosition.origin.y, _domPosition.size.width, _domPosition.size.height);
        
        [_videoLayer setFrame:frame];
        [_videoLayer setAnchorPoint:CGPointMake(0.5, 0.5)];

        if (_videoOrientation == 1 || _videoOrientation == 3) {
            if (_domOrientation == 0 || _domOrientation == 180) {
                [_videoLayer setTransform:CATransform3DMakeRotation(90.0 / 180.0 * M_PI, 0.0, 0.0, 1.0)];
            } else if (_domOrientation == -90 || _domOrientation == 90) {
                [_videoLayer setTransform:CATransform3DMakeRotation(0.0 / 180.0 * M_PI, 0.0, 0.0, 1.0)];
            }
        } else if (_videoOrientation == 2 || _videoOrientation == 4) {
            if (_domOrientation == 0 || _domOrientation == 180) {
                [_videoLayer setTransform:CATransform3DMakeRotation(90.0 / 180.0 * M_PI, 0.0, 0.0, 1.0)];
            } else if (_domOrientation == -90) {
                [_videoLayer setTransform:CATransform3DMakeRotation(180.0 / 180.0 * M_PI, 0.0, 0.0, 1.0)];
            } else if (_domOrientation == 90) {
                [_videoLayer setTransform:CATransform3DMakeRotation(0.0 / 180.0 * M_PI, 0.0, 0.0, 1.0)];
            }
        }
    }
}

- (void)renderVideo:(NSData *)data
{
    uint8_t *bytes = (uint8_t *)[data bytes];
    size_t bytes_size = [data length];

    if (bytes_size < 5) {
        return;
    }

    if ((bytes[4] & 0x1f) == 7) {
        _videoSequenceParameterSet = data;
        _videoPictureParameterSet = nil;

        if (_videoFormatDescription) {
            CFRelease(_videoFormatDescription);
        
            _videoFormatDescription = NULL;
        }
    }
    
    if ((bytes[4] & 0x1f) == 8) {
        _videoPictureParameterSet = data;
    }

    if (!_videoFormatDescription && (_videoSequenceParameterSet && _videoPictureParameterSet)) {
        CMFormatDescriptionRef videoFormatDescription = NULL;

        unsigned char const *parameterSets[2] = {
            [_videoSequenceParameterSet bytes] + 4,
            [_videoPictureParameterSet bytes] + 4
        };

        size_t parameterSetSizes[2] = {
            [_videoSequenceParameterSet length] - 4,
            [_videoPictureParameterSet length] - 4
        };

        if (CMVideoFormatDescriptionCreateFromH264ParameterSets(NULL, 2, parameterSets, parameterSetSizes, 4, &videoFormatDescription) == 0) {
//            CMVideoDimensions dimensions = CMVideoFormatDescriptionGetDimensions(videoFormatDescription);
//            CGSizeMake(dimensions.width, dimensions.height)];

            _videoFormatDescription = videoFormatDescription;
        } else {
            CFRelease(videoFormatDescription);
        }
    }

    if (!_videoLayer) {
        _videoLayer = [[AVSampleBufferDisplayLayer alloc] init];
    
        [_videoLayer setBackgroundColor:[[UIColor clearColor] CGColor]];
        [_videoLayer setVideoGravity:AVLayerVideoGravityResizeAspect];
    }

    if (![_videoLayer superlayer]) {
        [[[_webView scrollView] layer] insertSublayer:_videoLayer atIndex:0];
    }

    if (_videoFormatDescription && ((bytes[4] & 0x1f) == 5 || (bytes[4] & 0x1f) == 1)) {
        CMBlockBufferRef blockBuffer = NULL;

        if (CMBlockBufferCreateWithMemoryBlock(NULL, NULL, bytes_size, NULL, NULL, 0, bytes_size, 0, &blockBuffer) == 0) {
            if (CMBlockBufferReplaceDataBytes(bytes, blockBuffer, 0, bytes_size) == 0) {
                size_t buffer_size_minus4 = bytes_size - 4;
                const uint8_t avccHeader[] = { buffer_size_minus4 >> 24, buffer_size_minus4 >> 16, buffer_size_minus4 >> 8, buffer_size_minus4 };

                if (CMBlockBufferReplaceDataBytes(avccHeader, blockBuffer, 0, 4) == 0) {
                    CMSampleBufferRef sampleBuffer = NULL;
                    size_t sampleSizeArray[1] = { CMBlockBufferGetDataLength(blockBuffer) };

                    CMFormatDescriptionRef videoFormatDescription = _videoFormatDescription;

                    if (CMSampleBufferCreate(NULL, blockBuffer, YES, NULL, NULL, videoFormatDescription, 1, 0, NULL, 1, sampleSizeArray, &sampleBuffer) == 0) {
                        CFArrayRef attachments = CMSampleBufferGetSampleAttachmentsArray(sampleBuffer, YES);
                        CFMutableDictionaryRef dict = (CFMutableDictionaryRef)CFArrayGetValueAtIndex(attachments, 0);
                        CFDictionarySetValue(dict, kCMSampleAttachmentKey_DisplayImmediately, kCFBooleanTrue);
            
                        if ([_videoLayer respondsToSelector:@selector(status)] && [_videoLayer status] == AVQueuedSampleBufferRenderingStatusFailed) {
                            [self stop];
                        } else {
                            [_videoLayer enqueueSampleBuffer:sampleBuffer];
                            [_videoLayer setNeedsDisplay];
                        }
                    } else {
                        CFRelease(sampleBuffer);
                    }
                }
            } else {
                CFRelease(blockBuffer);
            }
        } else {
            CFRelease(blockBuffer);
        }
    }
}

void audio_property(void *inClientData, AudioFileStreamID inAudioFileStream, AudioFileStreamPropertyID inPropertyID, UInt32 *ioFlags)
{
    MNStream *self = (__bridge MNStream *)inClientData;

    if (inPropertyID == kAudioFileStreamProperty_ReadyToProducePackets) {
        UInt32 audioStreamDescriptionSize = sizeof(AudioStreamBasicDescription);
        AudioStreamBasicDescription *audioStreamDescription = malloc(audioStreamDescriptionSize);

        if (AudioFileStreamGetProperty([self audioParser], kAudioFileStreamProperty_DataFormat, &audioStreamDescriptionSize, audioStreamDescription) == 0) {
            if ([self audioStreamDescription]) {
                free([self audioStreamDescription]);
            }

            [self setAudioStreamDescription:audioStreamDescription];
        }
        
        if ([self audioStreamDescription]) {
            if ([self audioQueue]) {
                AudioQueueDispose([self audioQueue], YES);

                [self setAudioQueue:NULL];
            }
            
            AudioQueueRef audioQueue = NULL;
            if (AudioQueueNewOutput([self audioStreamDescription], audio_output, (__bridge void *)self, NULL, NULL, 0, &audioQueue) == 0) {
                AudioQueueSetParameter(audioQueue, kAudioQueueParam_Volume, 5.5);
                AudioQueueSetParameter(audioQueue, kAudioQueueParam_VolumeRampTime, 1.0);

                [self setAudioQueue:audioQueue];
            }
        }
    }
}

void audio_packet(void *inClientData, UInt32 inNumberBytes, UInt32 inNumberPackets, const void *inInputData, AudioStreamPacketDescription *inPacketDescriptions)
{
    MNStream *self = (__bridge MNStream *)inClientData;

    if (![self audioIsUnmuted]) {
        [self setAudioIsRunning:NO];

        AudioQueueStop([self audioQueue], YES);
    }

    AudioQueueBufferRef audioQueueBuffer = NULL;
    if (AudioQueueAllocateBufferWithPacketDescriptions([self audioQueue], inNumberBytes, inNumberPackets, &audioQueueBuffer) == 0) {
        audioQueueBuffer->mPacketDescriptionCount = inNumberPackets;
        memcpy(audioQueueBuffer->mPacketDescriptions, inPacketDescriptions, sizeof(AudioStreamPacketDescription) * inNumberPackets);

        audioQueueBuffer->mAudioDataByteSize = inNumberBytes;
        memcpy(audioQueueBuffer->mAudioData, inInputData, inNumberBytes);

        AudioTimeStamp currentTimestamp;
        AudioQueueGetCurrentTime([self audioQueue], NULL, &currentTimestamp, NULL);

        double currentSampleTime = round(currentTimestamp.mSampleTime + 0.5);

        AudioTimeStamp targetTimestamp;
        if (AudioQueueEnqueueBufferWithParameters([self audioQueue], audioQueueBuffer, 0, NULL, 0, 0, 0, NULL, NULL, &targetTimestamp) == 0) {
            if (![self audioIsRunning]) {
                [self setAudioIsRunning:YES];

                AudioQueueStart([self audioQueue], NULL);
            }

            double targetSampleTime = round(targetTimestamp.mSampleTime + 0.5);

            if (targetSampleTime < currentSampleTime || (targetSampleTime - currentSampleTime) > ([self audioMaxLagMilliseconds] / 1000.0 * [self audioStreamDescription]->mSampleRate)) {
                if ([self audioIsRunning]) {
                    [self setAudioIsRunning:NO];

                    AudioQueueStop([self audioQueue], YES);
                }
            }
        } else {
            AudioQueueFreeBuffer([self audioQueue], audioQueueBuffer);
        }
    }
}

void audio_output(void *inClientData, AudioQueueRef inAQ, AudioQueueBufferRef inBuffer) {
    AudioQueueFreeBuffer(inAQ, inBuffer);
}

- (void)renderAudio:(NSData *)data
{
    if (!_audioIsUnmuted) {
        return;
    }

    uint8_t *bytes = (uint8_t *)[data bytes];
    size_t size = [data length];

    if (!_audioBuffer) {
        _audioBuffer = CFArrayCreateMutable(NULL, 0, NULL);
    }

    if (!_audioParser) {
        AudioFileStreamOpen((__bridge void *)self, audio_property, audio_packet, kAudioFileAAC_ADTSType, &_audioParser);
    }

    AudioFileStreamParseBytes(_audioParser, (UInt32)size, bytes, 0);
}

- (void)webSocketStartConnectTimer
{
    if (_webSocketConnectTimer) {
        [_webSocketConnectTimer invalidate];
        
        _webSocketConnectTimer = nil;
    }

    if (_webSocketConnectTimeout > 0) {
        _webSocketConnectTimer = [NSTimer scheduledTimerWithTimeInterval:(_webSocketConnectTimeout / 1000.0) target:self selector:@selector(webSocketDidConnectTimeout:) userInfo:nil repeats:NO];
    }
}

- (void)webSocketStartIdleTimer
{
    _webSocketIsIdle = NO;

    if (_webSocketIdleTimeout) {
        [_webSocketIdleTimer invalidate];
        
        _webSocketIdleTimer = nil;
    }

    if (_webSocketIdleTimeout > 0) {
        _webSocketIdleTimer = [NSTimer scheduledTimerWithTimeInterval:(_webSocketIdleTimeout / 1000.0) target:self selector:@selector(webSocketDidIdleTimeout:) userInfo:nil repeats:NO];
    }
}

#pragma mark - WebSocket Additions

- (void)webSocketDidConnectTimeout:(NSTimer *)timer
{
    [self stop];
}

- (void)webSocketDidIdleTimeout:(NSTimer *)timer
{
    _webSocketIsIdle = YES;

    [self stop];
}

#pragma mark - SRWebSocketDelegate

- (void)webSocketDidOpen:(MNSRWebSocket *)webSocket
{
    if (_webSocketConnectTimer) {
        [_webSocketConnectTimer invalidate];
        
        _webSocketConnectTimer = nil;
    }

    [self webSocketStartIdleTimer];
}

- (void)webSocket:(MNSRWebSocket *)webSocket didReceiveMessage:(id)message
{
    [self render:(NSData *)message];
    
    [self webSocketStartIdleTimer];
}

- (void)webSocket:(MNSRWebSocket *)webSocket didFailWithError:(NSError *)error
{
    [self stop];
}

- (void)webSocket:(MNSRWebSocket *)webSocket didCloseWithCode:(NSInteger)code reason:(NSString *)reason wasClean:(BOOL)wasClean
{
    [self stop];
}

@end
