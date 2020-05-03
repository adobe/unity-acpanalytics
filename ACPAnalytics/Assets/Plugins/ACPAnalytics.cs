/*
Copyright 2020 Adobe. All rights reserved.
This file is licensed to you under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License. You may obtain a copy
of the License at http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under
the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR REPRESENTATIONS
OF ANY KIND, either express or implied. See the License for the specific language
governing permissions and limitations under the License.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

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

		void call(long size)
		{
			redirectedDelegate (size);
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
        [DllImport ("__Internal")]
		private static extern System.IntPtr acp_Analytics_ExtensionVersion();

        [DllImport ("__Internal")]
		private static extern void acp_Analytics_RegisterExtension();

        [DllImport ("__Internal")]
		private static extern void acp_SendQueuedHits();

        [DllImport ("__Internal")]
		private static extern void acp_ClearQueue();

        [DllImport ("__Internal")]
		private static extern void acp_GetQueueSize(AdobeGetQueueSizeCallback callback);

        [DllImport ("__Internal")]
		private static extern void acp_GetTrackingIdentifier(AdobeGetTrackingIdentifierCallback callback);

        [DllImport ("__Internal")]
		private static extern void acp_SetVisitorIdentifier(string visitorIdentifier);

        [DllImport ("__Internal")]
		private static extern void acp_GetVisitorIdentifier(AdobeGetVisitorIdentifierCallback callback);
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
        public static string AnalyticsExtensionVersion()
	    {
            #if UNITY_IPHONE && !UNITY_EDITOR
            return Marshal.PtrToStringAnsi(acp_Analytics_ExtensionVersion());		
            #elif UNITY_ANDROID && !UNITY_EDITOR
		    return analytics.CallStatic<string> ("extensionVersion");
            #else
		    return "";
            #endif
	    }

        public static void AnalyticsRegisterExtension()
        {
            #if UNITY_IPHONE && !UNITY_EDITOR
            acp_Analytics_RegisterExtension();
            #elif UNITY_ANDROID && !UNITY_EDITOR
            analytics.CallStatic("registerExtension");
            #endif
        }

        public static void SendQueuedHits()
        {
            #if UNITY_IPHONE && !UNITY_EDITOR
            acp_SendQueuedHits();
            #elif UNITY_ANDROID && !UNITY_EDITOR
		    analytics.CallStatic("sendQueuedHits");
            #endif
        }

        public static void ClearQueue()
        {
            #if UNITY_IPHONE && !UNITY_EDITOR
            acp_ClearQueue();
            #elif UNITY_ANDROID && !UNITY_EDITOR
		    analytics.CallStatic("clearQueue");
            #endif
        }

        public static void GetQueueSize(AdobeGetQueueSizeCallback callback)
        {
            if (callback == null) {
				Debug.Log("Unable to perform GetQueueSize, callback is null");
				return;
			}
            #if UNITY_IPHONE && !UNITY_EDITOR
            acp_GetQueueSize(callback);
            #elif UNITY_ANDROID && !UNITY_EDITOR
		    analytics.CallStatic("getQueueSize", new GetQueueSizeCallback(callback));
            #endif
        }

        public static void GetTrackingIdentifier(AdobeGetTrackingIdentifierCallback callback)
        {
            if (callback == null) {
				Debug.Log("Unable to perform GetTrackingIdentifier, callback is null");
				return;
			}
            #if UNITY_IPHONE && !UNITY_EDITOR
            acp_GetTrackingIdentifier(callback);
            #elif UNITY_ANDROID && !UNITY_EDITOR
		    analytics.CallStatic("getTrackingIdentifier", new GetTrackingIdentifierCallback(callback));
            #endif
        }

        public static void SetVisitorIdentifier(string visitorId)
        {
            if(visitorId == null)
            {
                Debug.Log("Unable to perform SetVisitorIdentifier, visitorId is null");
            }
            #if UNITY_IPHONE && !UNITY_EDITOR
            acp_SetVisitorIdentifier(visitorId == null ? "" : visitorId);
            #elif UNITY_ANDROID && !UNITY_EDITOR
		    analytics.CallStatic("setVisitorIdentifier", visitorId);
            #endif
        }

        public static void GetVisitorIdentifier(AdobeGetVisitorIdentifierCallback callback)
        {
			if (callback == null) {
				Debug.Log("Unable to perform GetVisitorIdentifier, callback is null");
				return;
			}
            #if UNITY_IPHONE && !UNITY_EDITOR
            acp_GetVisitorIdentifier(callback);
            #elif UNITY_ANDROID && !UNITY_EDITOR
		    analytics.CallStatic("getVisitorIdentifier", new GetVisitorIdentifierCallback(callback));
            #endif
        }
	}
}