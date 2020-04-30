using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using AOT;

namespace com.adobe.marketing.mobile {
    public class ACPHelpers {
        protected internal static string JsonStringFromStringDictionary(Dictionary<string, string> dict) 
        {
            if (dict == null || dict.Count <= 0) 
            {
                return null;
            }
            
            var entries = dict.Select(d => string.Format("\"{0}\": \"{1}\"", d.Key, d.Value));
            string jsonString = "{" + string.Join (",", entries.ToArray()) + "}";
            
            return jsonString;
        }

        protected internal static string JsonStringFromDictionary(Dictionary<string, object> dict) 
        {
            if (dict == null || dict.Count <= 0) 
            {
                return null;
            }
            
            var entries = dict.Select(d => string.Format("\"{0}\": \"{1}\"", d.Key, d.Value));
            string jsonString = "{" + string.Join (",", entries.ToArray()) + "}";
            
            return jsonString;
        }

        #if UNITY_ANDROID
        protected internal static AndroidJavaObject GetHashMapFromDictionary(Dictionary<string, object> dict)
        {
            // quick out if nothing in the dict param
            if (dict == null || dict.Count <= 0) 
            {
                return null;
            }
            
            AndroidJavaObject hashMap = new AndroidJavaObject ("java.util.HashMap");
            IntPtr putMethod = AndroidJNIHelper.GetMethodID(hashMap.GetRawClass(), "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
            object[] args = new object[2];
            foreach (KeyValuePair<string, object> kvp in dict)
            {
                using (var key = new AndroidJavaObject("java.lang.String", kvp.Key))
                {
                    using (var value = new AndroidJavaObject("java.lang.String", kvp.Value))
                    {
                        args[0] = key;
                        args[1] = value;
                        AndroidJNI.CallObjectMethod(hashMap.GetRawObject(), putMethod, AndroidJNIHelper.CreateJNIArgArray(args));
                    }
                }
            }
            
            return hashMap;
        }

        protected internal static AndroidJavaObject GetStringHashMapFromDictionary(Dictionary<string, string> dict)
        {
            // quick out if nothing in the dict param
            if (dict == null || dict.Count <= 0) 
            {
                return null;
            }
            
            AndroidJavaObject hashMap = new AndroidJavaObject ("java.util.HashMap");
            IntPtr putMethod = AndroidJNIHelper.GetMethodID(hashMap.GetRawClass(), "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
            object[] args = new object[2];
            foreach (KeyValuePair<string, string> kvp in dict)
            {
                using (var key = new AndroidJavaObject("java.lang.String", kvp.Key))
                {
                    using (var value = new AndroidJavaObject("java.lang.String", kvp.Value))
                    {
                        args[0] = key;
                        args[1] = value;
                        AndroidJNI.CallObjectMethod(hashMap.GetRawObject(), putMethod, AndroidJNIHelper.CreateJNIArgArray(args));
                    }
                }
            }
            
            return hashMap;
        }

        protected internal static Dictionary<string, object> GetDictionaryFromHashMap(AndroidJavaObject hashmap)
        {
            Dictionary<string, object> dict = new Dictionary<string,object> ();
            AndroidJavaObject entrySet = hashmap.Call<AndroidJavaObject>("entrySet");
            AndroidJavaObject[] array = entrySet.Call<AndroidJavaObject[]> ("toArray");
            foreach (AndroidJavaObject keyValuepair in array) 
            {
                string key = keyValuepair.Call<string> ("getKey");
                string value = keyValuepair.Call<string> ("getValue");
                dict.Add (key, value);
            }

            if (dict == null || dict.Count <= 0) 
            {
                return null;
            }
            return dict;
        }

        protected internal static AndroidJavaObject GetAdobeEventFromACPExtensionEvent(ACPExtensionEvent acpExtensionEvent) {
            if (acpExtensionEvent == null) 
            {
                return null;
            }

            using (AndroidJavaObject eventBuilder = new AndroidJavaObject("com.adobe.marketing.mobile.Event$Builder", acpExtensionEvent.eventName, acpExtensionEvent.eventType, acpExtensionEvent.eventSource)) 
            {
                if (acpExtensionEvent.eventData != null) {
                    using (var hashmap = GetHashMapFromDictionary(acpExtensionEvent.eventData)) {
                        eventBuilder.Call<AndroidJavaObject>("setEventData", hashmap);
                    }
                }
                
                AndroidJavaObject eventObj = eventBuilder.Call<AndroidJavaObject>("build");
                return eventObj;
            }
        }
        #endif
    }
}