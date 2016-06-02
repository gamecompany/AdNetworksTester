// MNAFNetworkReachabilityManager.m
// Copyright (c) 2011–2015 Alamofire Software Foundation (http://alamofire.org/)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#import "MNAFNetworkReachabilityManager.h"
#if !TARGET_OS_WATCH

#import <netinet/in.h>
#import <netinet6/in6.h>
#import <arpa/inet.h>
#import <ifaddrs.h>
#import <netdb.h>

NSString * const MNAFNetworkingReachabilityDidChangeNotification = @"com.alamofire.networking.reachability.change";
NSString * const MNAFNetworkingReachabilityNotificationStatusItem = @"MNAFNetworkingReachabilityNotificationStatusItem";

typedef void (^MNAFNetworkReachabilityStatusBlock)(MNAFNetworkReachabilityStatus status);

typedef NS_ENUM(NSUInteger, MNAFNetworkReachabilityAssociation) {
    MNAFNetworkReachabilityForAddress = 1,
    MNAFNetworkReachabilityForAddressPair = 2,
    MNAFNetworkReachabilityForName = 3,
};

NSString * MNAFStringFromNetworkReachabilityStatus(MNAFNetworkReachabilityStatus status) {
    switch (status) {
        case MNAFNetworkReachabilityStatusNotReachable:
            return NSLocalizedStringFromTable(@"Not Reachable", @"MNAFNetworking", nil);
        case MNAFNetworkReachabilityStatusReachableViaWWAN:
            return NSLocalizedStringFromTable(@"Reachable via WWAN", @"MNAFNetworking", nil);
        case MNAFNetworkReachabilityStatusReachableViaWiFi:
            return NSLocalizedStringFromTable(@"Reachable via WiFi", @"MNAFNetworking", nil);
        case MNAFNetworkReachabilityStatusUnknown:
        default:
            return NSLocalizedStringFromTable(@"Unknown", @"MNAFNetworking", nil);
    }
}

static MNAFNetworkReachabilityStatus MNAFNetworkReachabilityStatusForFlags(SCNetworkReachabilityFlags flags) {
    BOOL isReachable = ((flags & kSCNetworkReachabilityFlagsReachable) != 0);
    BOOL needsConnection = ((flags & kSCNetworkReachabilityFlagsConnectionRequired) != 0);
    BOOL canConnectionAutomatically = (((flags & kSCNetworkReachabilityFlagsConnectionOnDemand ) != 0) || ((flags & kSCNetworkReachabilityFlagsConnectionOnTraffic) != 0));
    BOOL canConnectWithoutUserInteraction = (canConnectionAutomatically && (flags & kSCNetworkReachabilityFlagsInterventionRequired) == 0);
    BOOL isNetworkReachable = (isReachable && (!needsConnection || canConnectWithoutUserInteraction));

    MNAFNetworkReachabilityStatus status = MNAFNetworkReachabilityStatusUnknown;
    if (isNetworkReachable == NO) {
        status = MNAFNetworkReachabilityStatusNotReachable;
    }
#if	TARGET_OS_IPHONE
    else if ((flags & kSCNetworkReachabilityFlagsIsWWAN) != 0) {
        status = MNAFNetworkReachabilityStatusReachableViaWWAN;
    }
#endif
    else {
        status = MNAFNetworkReachabilityStatusReachableViaWiFi;
    }

    return status;
}

static void MNAFNetworkReachabilityCallback(SCNetworkReachabilityRef __unused target, SCNetworkReachabilityFlags flags, void *info) {
    MNAFNetworkReachabilityStatus status = MNAFNetworkReachabilityStatusForFlags(flags);
    MNAFNetworkReachabilityStatusBlock block = (__bridge MNAFNetworkReachabilityStatusBlock)info;
    if (block) {
        block(status);
    }


    dispatch_async(dispatch_get_main_queue(), ^{
        NSNotificationCenter *notificationCenter = [NSNotificationCenter defaultCenter];
        NSDictionary *userInfo = @{ MNAFNetworkingReachabilityNotificationStatusItem: @(status) };
        [notificationCenter postNotificationName:MNAFNetworkingReachabilityDidChangeNotification object:nil userInfo:userInfo];
    });

}

static const void * MNAFNetworkReachabilityRetainCallback(const void *info) {
    return Block_copy(info);
}

static void MNAFNetworkReachabilityReleaseCallback(const void *info) {
    if (info) {
        Block_release(info);
    }
}

@interface MNAFNetworkReachabilityManager ()
@property (readwrite, nonatomic, strong) id networkReachability;
@property (readwrite, nonatomic, assign) MNAFNetworkReachabilityAssociation networkReachabilityAssociation;
@property (readwrite, nonatomic, assign) MNAFNetworkReachabilityStatus networkReachabilityStatus;
@property (readwrite, nonatomic, copy) MNAFNetworkReachabilityStatusBlock networkReachabilityStatusBlock;
@end

@implementation MNAFNetworkReachabilityManager

+ (instancetype)sharedManager {
    static MNAFNetworkReachabilityManager *_sharedManager = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        struct sockaddr_in address;
        bzero(&address, sizeof(address));
        address.sin_len = sizeof(address);
        address.sin_family = AF_INET;

        _sharedManager = [self managerForAddress:&address];
    });

    return _sharedManager;
}

+ (instancetype)managerForDomain:(NSString *)domain {
    SCNetworkReachabilityRef reachability = SCNetworkReachabilityCreateWithName(kCFAllocatorDefault, [domain UTF8String]);

    MNAFNetworkReachabilityManager *manager = [[self alloc] initWithReachability:reachability];
    manager.networkReachabilityAssociation = MNAFNetworkReachabilityForName;

    return manager;
}

+ (instancetype)managerForAddress:(const void *)address {
    SCNetworkReachabilityRef reachability = SCNetworkReachabilityCreateWithAddress(kCFAllocatorDefault, (const struct sockaddr *)address);

    MNAFNetworkReachabilityManager *manager = [[self alloc] initWithReachability:reachability];
    manager.networkReachabilityAssociation = MNAFNetworkReachabilityForAddress;

    return manager;
}

- (instancetype)initWithReachability:(SCNetworkReachabilityRef)reachability {
    self = [super init];
    if (!self) {
        return nil;
    }

    self.networkReachability = CFBridgingRelease(reachability);
    self.networkReachabilityStatus = MNAFNetworkReachabilityStatusUnknown;

    return self;
}

- (instancetype)init NS_UNAVAILABLE
{
    return nil;
}

- (void)dealloc {
    [self stopMonitoring];
}

#pragma mark -

- (BOOL)isReachable {
    return [self isReachableViaWWAN] || [self isReachableViaWiFi];
}

- (BOOL)isReachableViaWWAN {
    return self.networkReachabilityStatus == MNAFNetworkReachabilityStatusReachableViaWWAN;
}

- (BOOL)isReachableViaWiFi {
    return self.networkReachabilityStatus == MNAFNetworkReachabilityStatusReachableViaWiFi;
}

#pragma mark -

- (void)startMonitoring {
    [self stopMonitoring];

    if (!self.networkReachability) {
        return;
    }

    __weak __typeof(self)weakSelf = self;
    MNAFNetworkReachabilityStatusBlock callback = ^(MNAFNetworkReachabilityStatus status) {
        __strong __typeof(weakSelf)strongSelf = weakSelf;

        strongSelf.networkReachabilityStatus = status;
        if (strongSelf.networkReachabilityStatusBlock) {
            strongSelf.networkReachabilityStatusBlock(status);
        }

    };

    id networkReachability = self.networkReachability;
    SCNetworkReachabilityContext context = {0, (__bridge void *)callback, MNAFNetworkReachabilityRetainCallback, MNAFNetworkReachabilityReleaseCallback, NULL};
    SCNetworkReachabilitySetCallback((__bridge SCNetworkReachabilityRef)networkReachability, MNAFNetworkReachabilityCallback, &context);
    SCNetworkReachabilityScheduleWithRunLoop((__bridge SCNetworkReachabilityRef)networkReachability, CFRunLoopGetMain(), kCFRunLoopCommonModes);

    switch (self.networkReachabilityAssociation) {
        case MNAFNetworkReachabilityForName:
            break;
        case MNAFNetworkReachabilityForAddress:
        case MNAFNetworkReachabilityForAddressPair:
        default: {
            dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_BACKGROUND, 0),^{
                SCNetworkReachabilityFlags flags;
                SCNetworkReachabilityGetFlags((__bridge SCNetworkReachabilityRef)networkReachability, &flags);
                MNAFNetworkReachabilityStatus status = MNAFNetworkReachabilityStatusForFlags(flags);
                dispatch_async(dispatch_get_main_queue(), ^{
                    callback(status);

                    NSNotificationCenter *notificationCenter = [NSNotificationCenter defaultCenter];
                    [notificationCenter postNotificationName:MNAFNetworkingReachabilityDidChangeNotification object:nil userInfo:@{ MNAFNetworkingReachabilityNotificationStatusItem: @(status) }];


                });
            });
        }
            break;
    }
}

- (void)stopMonitoring {
    if (!self.networkReachability) {
        return;
    }

    SCNetworkReachabilityUnscheduleFromRunLoop((__bridge SCNetworkReachabilityRef)self.networkReachability, CFRunLoopGetMain(), kCFRunLoopCommonModes);
}

#pragma mark -

- (NSString *)localizedNetworkReachabilityStatusString {
    return MNAFStringFromNetworkReachabilityStatus(self.networkReachabilityStatus);
}

#pragma mark -

- (void)setReachabilityStatusChangeBlock:(void (^)(MNAFNetworkReachabilityStatus status))block {
    self.networkReachabilityStatusBlock = block;
}

#pragma mark - NSKeyValueObserving

+ (NSSet *)keyPathsForValuesAffectingValueForKey:(NSString *)key {
    if ([key isEqualToString:@"reachable"] || [key isEqualToString:@"reachableViaWWAN"] || [key isEqualToString:@"reachableViaWiFi"]) {
        return [NSSet setWithObject:@"networkReachabilityStatus"];
    }

    return [super keyPathsForValuesAffectingValueForKey:key];
}

@end
#endif
