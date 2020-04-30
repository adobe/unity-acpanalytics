using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using AOT;

namespace com.adobe.marketing.mobile
{
	public delegate void AdobeExtensionErrorCallback(string errorName, int errorCode);
	public delegate void AdobeEventCallback(string eventName, string eventType, string eventSource, string jsonEventData);
	public delegate void AdobePrivacyStatusCallback(int privacyStatus);
	public delegate void AdobeStartCallback();
	public delegate void AdobeIdentitiesCallback(string value);

	#if UNITY_ANDROID
	class ExtensionErrorCallback: AndroidJavaProxy
	{
		AdobeExtensionErrorCallback redirectedDelegate;
		public ExtensionErrorCallback (AdobeExtensionErrorCallback callback): base("com.adobe.marketing.mobile.ExtensionErrorCallback"){
			redirectedDelegate = callback;
		}

		void error(AndroidJavaObject error)
		{
			string errorName = error.Call<string>("getErrorName");
			int errorCode = error.Call<int>("getErrorCode");

			redirectedDelegate (errorName, errorCode);
		}
	}

	class EventCallback: AndroidJavaProxy
	{
		AdobeEventCallback redirectedDelegate;
		public EventCallback (AdobeEventCallback callback): base("com.adobe.marketing.mobile.AdobeCallback") {
			redirectedDelegate = callback;
		}

		void call(AndroidJavaObject eventObj)
		{
			string eventName = eventObj.Call<string>("getName");
			string eventType = eventObj.Call<string>("getType");
			string eventSource = eventObj.Call<string>("getSource");
			AndroidJavaObject eventDataHashmap = eventObj.Call<AndroidJavaObject>("getEventData");
			Dictionary<string, object> eventData = ACPHelpers.GetDictionaryFromHashMap(eventDataHashmap);
			redirectedDelegate (eventName, eventType, eventSource, ACPHelpers.JsonStringFromDictionary(eventData));
		}
	}

	class PrivacyStatusCallback: AndroidJavaProxy
	{
		AdobePrivacyStatusCallback redirectedDelegate;
		public PrivacyStatusCallback (AdobePrivacyStatusCallback callback): base("com.adobe.marketing.mobile.AdobeCallback") {
			redirectedDelegate = callback;
		}

		void call(AndroidJavaObject status)
		{
			redirectedDelegate (status.Call<int>("ordinal"));
		}
	}

	class StartCallback: AndroidJavaProxy
	{
		AdobeStartCallback redirectedDelegate;
		public StartCallback (AdobeStartCallback callback): base("com.adobe.marketing.mobile.AdobeCallback") {
			redirectedDelegate = callback;
		}

		void call(object value)
		{
			redirectedDelegate ();
		}
	}

	class IdentitiesCallback: AndroidJavaProxy
	{
		AdobeIdentitiesCallback redirectedDelegate;
		public IdentitiesCallback (AdobeIdentitiesCallback callback): base("com.adobe.marketing.mobile.AdobeCallback") {
			redirectedDelegate = callback;
		}

		void call(string value)
		{
			redirectedDelegate (value);
		}
	}
	#endif

	public class ExtensionError {
		public string errorName;
		public int errorCode;
		public ExtensionError(string errorParam, int errorCodeParam) {
			errorName = errorParam;
			errorCode = errorCodeParam;
		}
	}
	
	public class ACPExtensionEvent
	{
		public string eventName;
		public string eventType;
		public string eventSource;
		public Dictionary<string, object> eventData;

		public ACPExtensionEvent(string acp_eventName, string acp_eventType, string acp_eventSource, Dictionary<string, object> acp_eventData) {
			eventName = acp_eventName;
			eventType = acp_eventType;
			eventSource = acp_eventSource;
			eventData = acp_eventData;
		}
	}

	public class ACPCore 
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		private static string CONST_JAVA_CLASS_UNITY_PLAYER = "com.unity3d.player.UnityPlayer";
		#endif

		public enum ACPMobilePrivacyStatus {
			OPT_IN = 0,
			OPT_OUT = 1,
			UNKNOWN = 2
		};

		public enum ACPMobileLogLevel {
			ERROR = 0,
			WARNING = 1,
			DEBUG = 2,
			VERBOSE = 3,
			UNKOWN = -1
		};

		#if UNITY_IPHONE 
		/* ===================================================================
		 * extern declarations for iOS Methods
		 * =================================================================== */
		[DllImport ("__Internal")]
		private static extern System.IntPtr acp_ExtensionVersion();
		
		[DllImport ("__Internal")]
		private static extern void acp_SetLogLevel(int logLevel);

		[DllImport ("__Internal")]
		private static extern int acp_GetLogLevel();
		
		[DllImport ("__Internal")]
		private static extern void acp_Start(AdobeStartCallback callback);

		[DllImport ("__Internal")]
		private static extern void acp_ConfigureWithAppID(string appId);

		[DllImport ("__Internal")]
		private static extern void acp_DispatchEvent(string eventName, 
		string eventType, 
		string eventSource, 
		string cData,
		AdobeExtensionErrorCallback errorCallback);
		
		[DllImport ("__Internal")]
		private static extern void acp_DispatchEventWithResponseCallback(string eventName, 
		string eventType, 
		string eventSource, 
		string cData,
		AdobeEventCallback responseCallback,
		AdobeExtensionErrorCallback errorCallback);

		[DllImport ("__Internal")]
		private static extern void acp_DispatchResponseEvent(string responseEventName, 
		string responseEventType, 
		string responseEventSource, 
		string responseCData,
		string requestEventName,
		string requestEventType,
		string requestEventSource,
		string requestCData,
		AdobeExtensionErrorCallback errorCallback);
		
		[DllImport ("__Internal")]
		private static extern void acp_SetPrivacyStatus(int privacyStatus);

		[DllImport ("__Internal")]
		private static extern void acp_SetAdvertisingIdentifier(string adId);

		[DllImport ("__Internal")]
		private static extern void acp_GetSdkIdentities(AdobeIdentitiesCallback callback);
		
		[DllImport ("__Internal")]
		private static extern void acp_GetPrivacyStatus(AdobePrivacyStatusCallback callback);

		[DllImport ("__Internal")]
		private static extern void acp_DownloadRules();
		
		[DllImport ("__Internal")]
		private static extern void acp_UpdateConfiguration(string cdataString);

		[DllImport ("__Internal")]
		private static extern void acp_TrackState(string name, string cdataString);

		[DllImport ("__Internal")]
		private static extern void acp_TrackAction(string name, string cdataString);

		[DllImport ("__Internal")]
		private static extern void acp_LifecycleStart(string cdataString);

		[DllImport ("__Internal")]
		private static extern void acp_LifecyclePause();

		#endif
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		/* ===================================================================
		* Static Helper objects for our JNI access
		* =================================================================== */
		static AndroidJavaClass mobileCore = new AndroidJavaClass("com.adobe.marketing.mobile.MobileCore");
		#endif

		/*---------------------------------------------------------------------
		* Core Methods
		*----------------------------------------------------------------------*/
		public static string ExtensionVersion() 
		{
			#if UNITY_IPHONE && !UNITY_EDITOR		
			return Marshal.PtrToStringAnsi(acp_ExtensionVersion());		
			#elif UNITY_ANDROID && !UNITY_EDITOR 
			return mobileCore.CallStatic<string> ("extensionVersion");
			#else
			return "";
			#endif
		}

		public static void SetApplication() 
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			using (var actClass = new AndroidJavaClass(CONST_JAVA_CLASS_UNITY_PLAYER))
			{
				// get activity
				var activity = actClass.GetStatic<AndroidJavaObject>("currentActivity");
				// get application
				var application = activity.Call<AndroidJavaObject>("getApplication");
				mobileCore.CallStatic("setApplication", application);
			}
			#endif
		}

		public static AndroidJavaObject GetApplication() 
		{
			#if UNITY_ANDROID && !UNITY_EDITOR			
			return mobileCore.CallStatic<AndroidJavaObject> ("getApplication");
			#endif

			return null;
		}

		public static void SetLogLevel(ACPMobileLogLevel logLevel) {
			#if UNITY_ANDROID && !UNITY_EDITOR
			using (var logLevelVar = new AndroidJavaClass("com.adobe.marketing.mobile.LoggingMode")) 
			{
				var logModelObject = logLevelVar.GetStatic<AndroidJavaObject>(logLevel.ToString());
				mobileCore.CallStatic("setLogLevel", logModelObject);
			}
			#elif UNITY_IPHONE && !UNITY_EDITOR
			acp_SetLogLevel((int)logLevel);
			#endif
		}

		public static ACPMobileLogLevel GetLogLevel() {
			#if UNITY_ANDROID && !UNITY_EDITOR
			using (AndroidJavaObject logLevel = mobileCore.CallStatic<AndroidJavaObject>("getLogLevel")) 
			{
				int level = logLevel.Call<int>("ordinal");
				return ACPMobileLogLevelFromInt(level);
			}
			#elif UNITY_IPHONE && !UNITY_EDITOR
			return ACPMobileLogLevelFromInt(acp_GetLogLevel());
			#endif

			return ACPMobileLogLevel.UNKOWN;
		}

		public static void Start(AdobeStartCallback callback) {
			#if UNITY_ANDROID && !UNITY_EDITOR
			mobileCore.CallStatic("start", new StartCallback(callback));
			#elif UNITY_IPHONE && !UNITY_EDITOR
			acp_Start(callback);
			#endif
		}

		public static void ConfigureWithAppID(string appId) {
			#if UNITY_ANDROID && !UNITY_EDITOR
			mobileCore.CallStatic("configureWithAppID", appId);
			#elif UNITY_IPHONE && !UNITY_EDITOR
			acp_ConfigureWithAppID(appId);
			#endif
		}

		public static void DispatchEvent(ACPExtensionEvent acpExtensionEvent, AdobeExtensionErrorCallback errorCallback) {
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaObject eventObj = ACPHelpers.GetAdobeEventFromACPExtensionEvent(acpExtensionEvent);
			mobileCore.CallStatic<Boolean>("dispatchEvent", eventObj, new ExtensionErrorCallback(errorCallback));
			#elif UNITY_IPHONE && !UNITY_EDITOR
			string jsonDataEvent = ACPHelpers.JsonStringFromDictionary(acpExtensionEvent.eventData);
			acp_DispatchEvent(acpExtensionEvent.eventName, acpExtensionEvent.eventType, acpExtensionEvent.eventSource, jsonDataEvent, errorCallback);
			#endif
		}

		public static void DispatchEventWithResponseCallback(ACPExtensionEvent acpExtensionEvent, AdobeEventCallback responseCallback, AdobeExtensionErrorCallback errorCallback) {
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaObject eventObj = ACPHelpers.GetAdobeEventFromACPExtensionEvent(acpExtensionEvent);
			mobileCore.CallStatic<Boolean>("dispatchEventWithResponseCallback", eventObj, new EventCallback(responseCallback), new ExtensionErrorCallback(errorCallback));
			#elif UNITY_IPHONE && !UNITY_EDITOR
			string jsonDataEvent = ACPHelpers.JsonStringFromDictionary(acpExtensionEvent.eventData);
			acp_DispatchEventWithResponseCallback(acpExtensionEvent.eventName, acpExtensionEvent.eventType, acpExtensionEvent.eventSource, jsonDataEvent, responseCallback, errorCallback);
			#endif
		}

		public static void DispatchResponseEvent(ACPExtensionEvent responseEvent, ACPExtensionEvent requestEvent, AdobeExtensionErrorCallback errorCallback) {
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaObject responseEventObject = ACPHelpers.GetAdobeEventFromACPExtensionEvent(responseEvent);
			AndroidJavaObject requestEventObject = ACPHelpers.GetAdobeEventFromACPExtensionEvent(requestEvent);
			mobileCore.CallStatic<Boolean>("dispatchResponseEvent", responseEventObject, requestEventObject, new ExtensionErrorCallback(errorCallback));
			#elif UNITY_IPHONE && !UNITY_EDITOR
			string responseJsonDataEvent = ACPHelpers.JsonStringFromDictionary(responseEvent.eventData);
			string requestJsonDataEvent = ACPHelpers.JsonStringFromDictionary(requestEvent.eventData);
			acp_DispatchResponseEvent(responseEvent.eventName, responseEvent.eventType, responseEvent.eventSource, responseJsonDataEvent,
			requestEvent.eventName, requestEvent.eventType, requestEvent.eventSource, requestJsonDataEvent, errorCallback);
			#endif
		}

		public static void SetPrivacyStatus(ACPMobilePrivacyStatus privacyStatus) {
			#if UNITY_ANDROID && !UNITY_EDITOR
			using (var privacyClass = new AndroidJavaClass("com.adobe.marketing.mobile.MobilePrivacyStatus"))
			{
				var privacyStatusObject = privacyClass.GetStatic<AndroidJavaObject>(privacyStatus.ToString());
				mobileCore.CallStatic("setPrivacyStatus", privacyStatusObject);
			}
			#elif UNITY_IPHONE && !UNITY_EDITOR
			acp_SetPrivacyStatus((int)privacyStatus);
			#endif
		}

		public static void SetAdvertisingIdentifier(string adId) {
			#if UNITY_ANDROID && !UNITY_EDITOR
			mobileCore.CallStatic("setAdvertisingIdentifier", adId);
			#elif UNITY_IPHONE && !UNITY_EDITOR
			acp_SetAdvertisingIdentifier(adId);
			#endif
		}

		public static void GetSdkIdentities(AdobeIdentitiesCallback callback) {
			#if UNITY_ANDROID && !UNITY_EDITOR
			mobileCore.CallStatic("getSdkIdentities", new IdentitiesCallback(callback));
			#elif UNITY_IPHONE && !UNITY_EDITOR
			acp_GetSdkIdentities(callback);
			#endif
		}

		public static void GetPrivacyStatus(AdobePrivacyStatusCallback callback) {
			#if UNITY_ANDROID && !UNITY_EDITOR
			mobileCore.CallStatic("getPrivacyStatus", new PrivacyStatusCallback(callback));
			#elif UNITY_IPHONE && !UNITY_EDITOR
			acp_GetPrivacyStatus(callback);
			#endif
		}

		public static void DownloadRules() {
			#if UNITY_ANDROID && !UNITY_EDITOR
			using (var logLevelVar = new AndroidJavaClass("com.adobe.marketing.mobile.LoggingMode")) 
			{
				AndroidJavaObject logDebug = logLevelVar.GetStatic<AndroidJavaObject>("DEBUG");
				mobileCore.CallStatic("log", logDebug, "ACPCore", "DownloadRules() cannot be invoked on Android");
			}
			#elif UNITY_IPHONE && !UNITY_EDITOR
			acp_DownloadRules();
			#endif
		}

		public static void UpdateConfiguration(Dictionary<string, object> config) {
			#if UNITY_ANDROID && !UNITY_EDITOR
			var map = ACPHelpers.GetHashMapFromDictionary(config);
			mobileCore.CallStatic("updateConfiguration", map);
			#elif UNITY_IPHONE && !UNITY_EDITOR
			acp_UpdateConfiguration(ACPHelpers.JsonStringFromDictionary(config));
			#endif
		}

		public static void TrackState(string name, Dictionary<string, string> contextDataDict) {
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaObject contextData = ACPHelpers.GetStringHashMapFromDictionary(contextDataDict);
			mobileCore.CallStatic("trackState", name, contextData);
			#elif UNITY_IPHONE && !UNITY_EDITOR
			acp_TrackState(name, ACPHelpers.JsonStringFromStringDictionary(contextDataDict));
			#endif
		}

		public static void TrackAction(string name, Dictionary<string, string> contextDataDict) {
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaObject contextData = ACPHelpers.GetStringHashMapFromDictionary(contextDataDict);
			mobileCore.CallStatic("trackAction", name, contextData);
			#elif UNITY_IPHONE && !UNITY_EDITOR
			acp_TrackAction(name, ACPHelpers.JsonStringFromStringDictionary(contextDataDict));
			#endif
		}

		public static void LifecycleStart(Dictionary<string, string> additionalContextData) {
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaObject contextData = ACPHelpers.GetStringHashMapFromDictionary(additionalContextData);
			mobileCore.CallStatic("lifecycleStart", contextData);
			#elif UNITY_IPHONE && !UNITY_EDITOR
			acp_LifecycleStart(ACPHelpers.JsonStringFromStringDictionary(additionalContextData));
			#endif
		}

		public static void LifecyclePause() {
			#if UNITY_ANDROID && !UNITY_EDITOR
			mobileCore.CallStatic("lifecyclePause");
			#elif UNITY_IPHONE && !UNITY_EDITOR
			acp_LifecyclePause();
			#endif
		}

		/* ===================================================================
		 * Helper Methods
		 * =================================================================== */	
		 private static ACPMobileLogLevel ACPMobileLogLevelFromInt(int logLevel)
		{
			switch (logLevel) 
			{
			case 0:
				return ACPMobileLogLevel.ERROR;				
			case 1:
				return ACPMobileLogLevel.WARNING;				
			case 2:
				return ACPMobileLogLevel.DEBUG;
			case 3:
				return ACPMobileLogLevel.VERBOSE;
			default:
				return ACPMobileLogLevel.UNKOWN;				
			}
		}

		internal static ACPMobilePrivacyStatus ACPMobilePrivacyStatusFromInt(int status)
		{
			switch (status) 
			{
			case 0:
				return ACPMobilePrivacyStatus.OPT_IN;				
			case 1:
				return ACPMobilePrivacyStatus.OPT_OUT;					
			default:
				return ACPMobilePrivacyStatus.UNKNOWN;			
			}
		}
	}
}

