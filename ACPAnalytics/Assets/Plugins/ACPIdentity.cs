using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace com.adobe.marketing.mobile
{
	public delegate void AdobeIdentityAppendToUrlCallback(string url);
	public delegate void AdobeGetIdentifiersCallback(string visitorIds);
	public delegate void AdobeGetExperienceCloudIdCallback(string cloudId);
	public delegate void AdobeGetUrlVariables(string urlVariables);
	

	#if UNITY_ANDROID
	class IdentityAppendToUrlCallback: AndroidJavaProxy
	{
		AdobeIdentityAppendToUrlCallback redirectedDelegate;
		public IdentityAppendToUrlCallback (AdobeIdentityAppendToUrlCallback callback): base("com.adobe.marketing.mobile.AdobeCallback") {
			redirectedDelegate = callback;
		}

		void call(string url)
		{
			redirectedDelegate (url);
		}
	}

	class GetIdentifiersCallback: AndroidJavaProxy
	{
		AdobeGetIdentifiersCallback redirectedDelegate;
		public GetIdentifiersCallback (AdobeGetIdentifiersCallback callback): base("com.adobe.marketing.mobile.AdobeCallback") {
			redirectedDelegate = callback;
		}

		void call(AndroidJavaObject visitorIds)
		{
			if (visitorIds == null) {
				redirectedDelegate ("");
				return;
			}

			int length = visitorIds.Call<int>("size");
			List<string> visIdsList = new List<string>();
			for (int i = 0; i < length; i++)
			{
				AndroidJavaObject visitorId = visitorIds.Call<AndroidJavaObject>("get", i);
				Dictionary<string, string> vistiorIdDict = ACPIdentity.dictFromVisitorIdentifier(visitorId);
				visIdsList.Add(ACPHelpers.JsonStringFromStringDictionary(vistiorIdDict));
			}
			string result = string.Join(",", visIdsList.ToArray());
			redirectedDelegate (result);
		}
	}

	class GetExperienceCloudIdCallback: AndroidJavaProxy
	{
		AdobeGetExperienceCloudIdCallback redirectedDelegate;
		public GetExperienceCloudIdCallback (AdobeGetExperienceCloudIdCallback callback): base("com.adobe.marketing.mobile.AdobeCallback") {
			redirectedDelegate = callback;
		}

		void call(string cloudId)
		{
			redirectedDelegate (cloudId);
		}
	}

	class GetUrlVariables: AndroidJavaProxy
	{
		AdobeGetUrlVariables redirectedDelegate;
		public GetUrlVariables (AdobeGetUrlVariables callback): base("com.adobe.marketing.mobile.AdobeCallback") {
			redirectedDelegate = callback;
		}

		void call(string urlVariables)
		{
			redirectedDelegate (urlVariables);
		}
	}
	#endif

    public class ACPIdentity 
    {
        #if UNITY_IPHONE 
		/* ===================================================================
		 * extern declarations for iOS Methods
		 * =================================================================== */
		[DllImport ("__Internal")]
		private static extern System.IntPtr acp_ExtensionVersion();

		[DllImport ("__Internal")]
		private static extern void acp_AppendToUrl(AdobeIdentityAppendToUrlCallback callback);

		[DllImport ("__Internal")]
		private static extern void acp_GetIdentifiers(AdobeGetIdentifiersCallback callback);

		[DllImport ("__Internal")]
		private static extern void acp_GetExperienceCloudIdCallback(AdobeGetExperienceCloudIdCallback callback);

		[DllImport ("__Internal")]
		private static extern void acp_SyncIdentifier(string identifierType, string identifier, int authState);

		[DllImport ("__Internal")]
		private static extern void acp_SyncIdentifiers(string identifiers);

		[DllImport ("__Internal")]
		private static extern void acp_SyncIdentifiers(string identifiers, int authState);

		[DllImport ("__Internal")]
		private static extern void acp_GetUrlVariables(AdobeGetUrlVariables callback);

        #endif
        
        #if UNITY_ANDROID && !UNITY_EDITOR
		/* ===================================================================
		* Static Helper objects for our JNI access
		* =================================================================== */
        static AndroidJavaClass identity = new AndroidJavaClass("com.adobe.marketing.mobile.Identity");
        #endif

		public enum ACPAuthenticationState {
			UNKNOWN = 0,
			AUTHENTICATED = 1,
			LOGGED_OUT = 2
		};

        /*---------------------------------------------------------------------
		* Methods
		*----------------------------------------------------------------------*/
        public static string ExtensionVersion() 
		{
			#if UNITY_IPHONE && !UNITY_EDITOR		
			return Marshal.PtrToStringAnsi(acp_ExtensionVersion());		
			#elif UNITY_ANDROID && !UNITY_EDITOR 
			return identity.CallStatic<string> ("extensionVersion");
			#else
			return "";
			#endif
		}

		public static void registerExtension() {
			#if UNITY_IPHONE && !UNITY_EDITOR		
			#elif UNITY_ANDROID && !UNITY_EDITOR 
			identity.CallStatic("registerExtension");
			#endif
		}

		public static void AppendToUrl(string url, AdobeIdentityAppendToUrlCallback callback) 
		{
			#if UNITY_ANDROID && !UNITY_EDITOR		
			identity.CallStatic("appendVisitorInfoForURL", url, new IdentityAppendToUrlCallback(callback));
			#elif UNITY_IPHONE && !UNITY_EDITOR	
			#endif
		}

		public static void GetIdentifiers(AdobeGetIdentifiersCallback callback) 
		{
			#if UNITY_ANDROID && !UNITY_EDITOR		
			identity.CallStatic("getIdentifiers", new GetIdentifiersCallback(callback));
			#elif UNITY_IPHONE && !UNITY_EDITOR	
			#endif
		}

		public static void GetExperienceCloudIdCallback(AdobeGetExperienceCloudIdCallback callback) 
		{
			#if UNITY_ANDROID && !UNITY_EDITOR		
			identity.CallStatic("getExperienceCloudId", new GetExperienceCloudIdCallback(callback));
			#elif UNITY_IPHONE && !UNITY_EDITOR	
			#endif
		}

		public static void SyncIdentifier(string identifierType, string identifier, ACPAuthenticationState authState) 
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			using (var authStateClass = new AndroidJavaClass("com.adobe.marketing.mobile.VisitorID.AuthenticationState")) 
			{
				var authStateObj = authStateClass.GetStatic<AndroidJavaObject>(authState.ToString());
				identity.CallStatic("syncIdentifier", identifierType, identifier, authStateObj);
			}		
			#elif UNITY_IPHONE && !UNITY_EDITOR	
			#endif
		}

		public static void SyncIdentifiers(Dictionary<string, string> ids) 
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			//AndroidJavaObject idMap = ACPHelpers.GetHashMapFromDictionary(ids);
			// identity.CallStatic("syncIdentifiers", idMap);
			#elif UNITY_IPHONE && !UNITY_EDITOR	
			#endif
		}

		public static void SyncIdentifiers(Dictionary<string, string> ids, ACPAuthenticationState authenticationState) 
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			using (var authStateClass = new AndroidJavaClass("com.adobe.marketing.mobile.VisitorID.AuthenticationState")) 
			{
				var authStateObj = authStateClass.GetStatic<AndroidJavaObject>(authenticationState.ToString());
				//AndroidJavaObject idMap = ACPHelpers.GetHashMapFromDictionary(ids);
				//identity.CallStatic("syncIdentifiers", idMap, authStateObj);
			}
			#elif UNITY_IPHONE && !UNITY_EDITOR	
			#endif
		}

		public static void GetUrlVariables(AdobeGetUrlVariables callback) 
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			identity.CallStatic("getUrlVariables", new GetUrlVariables(callback));
			#elif UNITY_IPHONE && !UNITY_EDITOR	
			#endif
		}

		/* ===================================================================
		 * Helper Methods
		 * =================================================================== */	
		 #if UNITY_ANDROID
		internal static Dictionary<string, string> dictFromVisitorIdentifier(AndroidJavaObject visitorId) {
			if (visitorId == null) {
				 return null;
			}
			Dictionary<string, string> dict = new Dictionary<string,string> ();
			dict.Add("idOrigin", visitorId.Call<string>("getIdOrigin"));
			dict.Add("idType", visitorId.Call<string>("getIdType"));
			dict.Add("identifier", visitorId.Call<string>("getId"));
			dict.Add("authenticationState", ACPIdentity.stringFromAuthState(visitorId.Call<AndroidJavaObject>("authenticationState")));
			return dict;
		 }

		internal static string stringFromAuthState(AndroidJavaObject authState) {
			if (authState == null) {
				return "ACP_VISITOR_AUTH_STATE_UNKNOWN";
			}
			using (var authStateObj = new AndroidJavaClass("com.adobe.marketing.mobile.VisitorID.AuthenticationState"))
			{
				if (authState.Call<int>("ordinal") == (authStateObj.CallStatic<AndroidJavaObject>("AUTHENTICATED")).Call<int>("ordinal")) {
					return "ACP_VISITOR_AUTH_STATE_AUTHENTICATED";
				} else if(authState.Call<int>("ordinal") == (authStateObj.CallStatic<AndroidJavaObject>("LOGGED_OUT")).Call<int>("ordinal")) {
					return "ACP_VISITOR_AUTH_STATE_LOGGED_OUT";
				}
			}

			return "ACP_VISITOR_AUTH_STATE_UNKNOWN";
		 }

		 #endif

    }
}



