#import <AppTrackingTransparency/ATTrackingManager.h>
#import "UnityInterface.h"

NSString* _gameObjectNameCallback;
NSString* _methodNameCallback;


NSString* GetResultString(ATTrackingManagerAuthorizationStatus status);
NSString* CreateNSString (const char* string);
void SendResultToUnity();

void RequestTrackingAuthorizationNative(const char* gameObjectNameCallback,
                                          const char* methodNameCallback,
                                          const BOOL forceShow)
{
    _gameObjectNameCallback = CreateNSString(gameObjectNameCallback);
    _methodNameCallback = CreateNSString(methodNameCallback);
    
    if (@available(iOS 14.5, *) || forceShow)
    {
        ATTrackingManagerAuthorizationStatus curStatus = ATTrackingManager.trackingAuthorizationStatus;
        
        if (curStatus == ATTrackingManagerAuthorizationStatusNotDetermined
            || curStatus == ATTrackingManagerAuthorizationStatusRestricted)
        {
            [ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status) {
                
                NSString* result = GetResultString(status);
                SendResultToUnity(result);
            }];
        }
        else
        {
            NSString* result = GetResultString(curStatus);
            SendResultToUnity(result);
        }
    }
    else
    {
        NSString* result = CreateNSString("Authorized");
        SendResultToUnity(result);
    }
}

NSString* CreateNSString(const char* string)
{
    if (string)
    {
        return [NSString stringWithUTF8String: string];
    }
    else
    {
        return [NSString stringWithUTF8String: ""];
    }
}

NSString* GetResultString(ATTrackingManagerAuthorizationStatus status)
{
    NSString* result = nil;

    switch(status)
    {
        case ATTrackingManagerAuthorizationStatusNotDetermined:
            result = CreateNSString("NotDetermined");
            break;
        case ATTrackingManagerAuthorizationStatusRestricted:
            result = CreateNSString("Restricted");
            break;
        case ATTrackingManagerAuthorizationStatusDenied:
            result = CreateNSString("Denied");
            break;
        case ATTrackingManagerAuthorizationStatusAuthorized:
            result = CreateNSString("Authorized");
            break;
        default:
            result = CreateNSString("Authorized");
    }
    
    return result;
}

void SendResultToUnity(NSString* result)
{
    UnitySendMessage(_gameObjectNameCallback.UTF8String,
                     _methodNameCallback.UTF8String,
                     result.UTF8String);
}
