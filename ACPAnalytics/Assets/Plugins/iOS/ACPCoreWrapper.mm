#import "ACPCoreWrapper.h"
#import "ACPCore.h"
#import "ACPExtensionEvent.h"

NSDictionary *_getDictionaryFromJsonString(const char *jsonString);
ACPExtensionEvent *_getACPExtensionEventFromData (const char *eventName, const char *eventType, const char *eventSource, const char *cData);
NSString *_jsonStringWithPrettyPrint (NSDictionary *dict, NSError *error);

const char *acp_ExtensionVersion() {
    return [[ACPCore extensionVersion] cStringUsingEncoding:NSUTF8StringEncoding];
}

void acp_SetLogLevel(int logLevel) {
    [ACPCore setLogLevel:ACPMobileLogLevel(logLevel)];
}
int acp_GetLogLevel() {
    return (int)[ACPCore logLevel];
}

void acp_Start(void (*callback)()) {
    [ACPCore start:^{
        callback();
    }];
}

 void acp_ConfigureWithAppID(const char *appId) {
     NSString *appIdString = appId ? [NSString stringWithCString:appId encoding:NSUTF8StringEncoding] : nil;
     [ACPCore configureWithAppId:appIdString];
 }

void acp_DispatchEvent(const char *eventName, const char *eventType, const char *eventSource, const char *cData, void (*errorCallback)(const char *errorName, int errorCode)) {
    ACPExtensionEvent *event = _getACPExtensionEventFromData(eventName, eventType, eventSource, cData);
    NSError* error = nil;
    if ([ACPCore dispatchEvent:event error:&error]) {
        errorCallback("", 0);
    } else {
        errorCallback([error.localizedDescription cStringUsingEncoding:NSUTF8StringEncoding], (int)error.code);
    }
}

void acp_DispatchEventWithResponseCallback(const char *eventName, const char *eventType, const char *eventSource, const char *cData, void (*responseCallback)(const char *resEventName, const char *resEventType, const char *resEventSource, const char *resEventData), void (*errorCallback)(const char *errorName, int errorCode)) {
    ACPExtensionEvent *event = _getACPExtensionEventFromData(eventName, eventType, eventSource, cData);
    NSError* error = nil;
    if ([ACPCore dispatchEventWithResponseCallback:event responseCallback:^(ACPExtensionEvent * _Nonnull responseEvent) {
        
        responseCallback([responseEvent.eventName cStringUsingEncoding:NSUTF8StringEncoding],
                         [responseEvent.eventType cStringUsingEncoding:NSUTF8StringEncoding],
                         [responseEvent.eventSource cStringUsingEncoding:NSUTF8StringEncoding],
                         [_jsonStringWithPrettyPrint(responseEvent.eventData, error) cStringUsingEncoding:NSUTF8StringEncoding]);
    } error:&error]) {
        // nothing
        errorCallback("", 0);
    } else {
        errorCallback([error.localizedDescription cStringUsingEncoding:NSUTF8StringEncoding], (int)error.code);
    }
}

void acp_DispatchResponseEvent(const char *responseEventName, const char *responseEventType, const char *responseEventSource, const char *responseCData, const char *requestEventName, const char *requestEventType, const char *requestEventSource, const char *requestCData, void (*errorCallback)(const char *errorName, int errorCode)) {
    ACPExtensionEvent *responseEvent = _getACPExtensionEventFromData(responseEventName, responseEventType, responseEventSource, responseCData);
    ACPExtensionEvent *requestEvent = _getACPExtensionEventFromData(requestEventName, requestEventType, requestEventSource, requestCData);
    NSError* error = nil;
    if ([ACPCore dispatchResponseEvent:responseEvent requestEvent:requestEvent error:&error]) {
        errorCallback("", 0);
    } else {
        errorCallback([error.localizedDescription cStringUsingEncoding:NSUTF8StringEncoding], (int)error.code);
    }
}

void acp_SetPrivacyStatus(int privacyStatus) {
    [ACPCore setPrivacyStatus:ACPMobilePrivacyStatus(privacyStatus)];
}

void acp_SetAdvertisingIdentifier(const char *adId) {
    NSString *adIdString = adId ? [NSString stringWithCString:adId encoding:NSUTF8StringEncoding] : nil;
    [ACPCore setAdvertisingIdentifier:adIdString];
}
void acp_GetSdkIdentities(void (*callback)(const char *ids)) {
    [ACPCore getSdkIdentities:^(NSString * _Nullable content) {
        callback([content cStringUsingEncoding:NSUTF8StringEncoding]);
    }];
}

void acp_GetPrivacyStatus(void (*callback)(int status)) {
    [ACPCore getPrivacyStatus:^(ACPMobilePrivacyStatus status) {
        callback((int)status);
    }];
}

void acp_DownloadRules() {
    [ACPCore downloadRules];
}

void acp_UpdateConfiguration(const char *cdataString) {
    [ACPCore updateConfiguration:_getDictionaryFromJsonString(cdataString)];
}

void acp_TrackState(const char *name, const char *cdataString) {
    NSString *nameString = cdataString ? [NSString stringWithCString:cdataString encoding:NSUTF8StringEncoding] : nil;
    [ACPCore trackState:nameString data:_getDictionaryFromJsonString(cdataString)];
}

void acp_TrackAction(const char *name, const char *cdataString) {
    NSString *nameString = cdataString ? [NSString stringWithCString:cdataString encoding:NSUTF8StringEncoding] : nil;
    [ACPCore trackAction:nameString data:_getDictionaryFromJsonString(cdataString)];
}

void acp_LifecycleStart(const char *cdataString) {
    [ACPCore lifecycleStart:_getDictionaryFromJsonString(cdataString)];
}

void acp_LifecyclePause() {
    [ACPCore lifecyclePause];
}

// Helper functions
NSDictionary *_getDictionaryFromJsonString(const char *jsonString) {
    if (!jsonString) {
        return nil;
    }
    
    NSError *error = nil;
    NSString *tempString = [NSString stringWithCString:jsonString encoding:NSUTF8StringEncoding];
    NSData *data = [tempString dataUsingEncoding:NSUTF8StringEncoding];
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data
                                                         options:NSJSONReadingMutableContainers
                                                           error:&error];
    
    return (dict && !error) ? dict : nil;
}

ACPExtensionEvent *_getACPExtensionEventFromData (const char *eventName, const char *eventType, const char *eventSource, const char *cData) {
    NSString *name = [NSString stringWithCString:eventName encoding:NSUTF8StringEncoding];
    NSString *type = [NSString stringWithCString:eventType encoding:NSUTF8StringEncoding];
    NSString *source = [NSString stringWithCString:eventSource encoding:NSUTF8StringEncoding];
    NSDictionary *dict = _getDictionaryFromJsonString(cData);

    if (name && type && source && (dict == nil || [dict isKindOfClass:[NSDictionary class]])) {
        return [ACPExtensionEvent extensionEventWithName:name type:type source:source data:dict error:nil];
    }

    return nil;
}

NSString *_jsonStringWithPrettyPrint (NSDictionary *dict, NSError *error) {
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict
                                                       options:NSJSONWritingPrettyPrinted
                                                         error:&error];
    
    if (! jsonData) {
        NSLog(@"%s: error: %@", __func__, error.localizedDescription);
        return @"{}";
    } else {
        return [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    }
}