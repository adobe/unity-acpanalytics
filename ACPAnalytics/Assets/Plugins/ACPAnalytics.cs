using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.adobe.marketing.mobile
{
    public delegate void AdobeGetQueueSizeCallback(long queueSize);
    public delegate void AdobeGetTrackingIdentifierCallback(string trackingIdentifier);
    public delegate void AdobeGetVisitorIdentifierCallback(string visitorIdentifier);

    #if UNITY_ANDROID
	class GetQueueSizeCallback: AndroidJavaProxy
	{
		AdobeGetQueueSizeCallback redirectedDelegate;
		public GetQueueSizeCallback (AdobeGetQueueSizeCallback callback): base("com.adobe.marketing.mobile.AdobeCallback") {
			redirectedDelegate = callback;
		}

		void call(long status)
		{
			redirectedDelegate (status);
		}
	}

    class GetTrackingIdentifierCallback: AndroidJavaProxy
	{
		AdobeGetTrackingIdentifierCallback redirectedDelegate;
		public GetTrackingIdentifierCallback (AdobeGetTrackingIdentifierCallback callback): base("com.adobe.marketing.mobile.AdobeCallback") {
			redirectedDelegate = callback;
		}

		void call(string trackingIdentifier)
		{
			redirectedDelegate (trackingIdentifier);
		}
	}

    class GetVisitorIdentifierCallback: AndroidJavaProxy
	{
		AdobeGetVisitorIdentifierCallback redirectedDelegate;
		public GetVisitorIdentifierCallback (AdobeGetVisitorIdentifierCallback callback): base("com.adobe.marketing.mobile.AdobeCallback") {
			redirectedDelegate = callback;
		}

		void call(string visitorIdentifier)
		{
			redirectedDelegate (visitorIdentifier);
		}
	}
    #endif

	public class ACPAnalytics
	{
    #if UNITY_IPHONE
	/* ===================================================================
		* extern declarations for iOS Methods
		* =================================================================== */

	/*---------------------------------------------------------------------
	* Core
	*----------------------------------------------------------------------*/
	//TODO
    #endif

    #if UNITY_ANDROID && !UNITY_EDITOR
    /* ===================================================================
    * Static Helper objects for our JNI access
    * =================================================================== */
    static AndroidJavaClass analytics = new AndroidJavaClass("com.adobe.marketing.mobile.Analytics");
    #endif

		/*---------------------------------------------------------------------
		* Analytics Methods
		*----------------------------------------------------------------------*/
        public static void RegisterExtension()
        {
            #if UNITY_IPHONE && !UNITY_EDITOR
            //todo
            #elif UNITY_ANDROID && !UNITY_EDITOR
			analytics.CallStatic("registerExtension");
            #endif
        }

		public static string AnalyticsExtensionVersion()
		{
            #if UNITY_IPHONE && !UNITY_EDITOR
            //todo
            #elif UNITY_ANDROID && !UNITY_EDITOR
			return analytics.CallStatic<string> ("extensionVersion");
            #else
			return "";
            #endif
		}

        public static void SendQueuedHits()
        {
            #if UNITY_IPHONE && !UNITY_EDITOR
            //todo
            #elif UNITY_ANDROID && !UNITY_EDITOR
			analytics.CallStatic("sendQueuedHits");
            #endif
        }

        public static void ClearQueue()
        {
            #if UNITY_IPHONE && !UNITY_EDITOR
            //todo
            #elif UNITY_ANDROID && !UNITY_EDITOR
			analytics.CallStatic("clearQueue");
            #endif
        }

        public static void GetQueueSize(AdobeGetQueueSizeCallback callback)
        {
            #if UNITY_IPHONE && !UNITY_EDITOR
            //todo
            #elif UNITY_ANDROID && !UNITY_EDITOR
			analytics.CallStatic("getQueueSize", new GetQueueSizeCallback(callback));
            #endif
        }

        public static void GetTrackingIdentifier(AdobeGetTrackingIdentifierCallback callback)
        {
            #if UNITY_IPHONE && !UNITY_EDITOR
            //todo
            #elif UNITY_ANDROID && !UNITY_EDITOR
			analytics.CallStatic("getTrackingIdentifier", new GetTrackingIdentifierCallback(callback));
            #endif
        }

        public static void SetVisitorIdentifier(string visitorId)
        {
            #if UNITY_IPHONE && !UNITY_EDITOR
            //todo
            #elif UNITY_ANDROID && !UNITY_EDITOR
			analytics.CallStatic("setVisitorIdentifier", visitorId);
            #endif
        }

        public static void GetVisitorIdentifier(AdobeGetVisitorIdentifierCallback callback)
        {
            #if UNITY_IPHONE && !UNITY_EDITOR
            //todo
            #elif UNITY_ANDROID && !UNITY_EDITOR
			analytics.CallStatic("getVisitorIdentifier", new GetVisitorIdentifierCallback(callback));
            #endif
        }
	}
}