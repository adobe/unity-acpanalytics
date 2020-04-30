#ifndef Unity_iPhone_ACPCoreWrapper_h
#define Unity_iPhone_ACPCoreWrapper_h

extern "C" {
    const char *acp_ExtensionVersion();
    void acp_SetLogLevel(int logLevel);
    int acp_GetLogLevel();
    void acp_Start(void (*callback)());
    void acp_ConfigureWithAppID(const char *appId);
    void acp_DispatchEvent(const char *eventName, const char *eventType, const char *eventSource, const char *cData, void (*errorCallback)(const char *errorName, int errorCode));
    void acp_DispatchEventWithResponseCallback(const char *eventName, const char *eventType, const char *eventSource, const char *cData, void (*responseCallback)(const char *resEventName, const char *resEventType, const char *resEventSource, const char *resEventData), void (*errorCallback)(const char *errorName, int errorCode));
    void acp_DispatchResponseEvent(const char *responseEventName, const char *responseEventType, const char *responseEventSource, const char *responseCData, const char *requestEventName, const char *requestEventType, const char *requestEventSource, const char *requestCData, void (*errorCallback)(const char *errorName, int errorCode));
    void acp_SetPrivacyStatus(int privacyStatus);
    void acp_SetAdvertisingIdentifier(const char *adId);
    void acp_GetSdkIdentities(void (*callback)(const char *ids));
    void acp_GetPrivacyStatus(void (*callback)(int status));
    void acp_DownloadRules();
    void acp_UpdateConfiguration(const char *cdataString);
    void acp_TrackState(const char *name, const char *cdataString);
    void acp_TrackAction(const char *name, const char *cdataString);
    void acp_LifecycleStart(const char *cdataString);
    void acp_LifecyclePause();
}

#endif