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
using UnityEngine.UI;
using com.adobe.marketing.mobile;
using System;
using AOT;

public class SceneScript : MonoBehaviour
{
    // Analytics Buttons
    public Button btnExtensionVersion;
    public Button btnSendQueuedHits;
    public Button btnClearQueue;
    public Button btnGetQueueSize;
    public Button btnGetTrackingIdentifier;
    public Button btnSetVisitorIdentifier;
    public Button btnGetVisitorIdentifier;
    public InputField visitorIdentifier;
    public static Text callbackResults;

    // Analytics callbacks
    [MonoPInvokeCallback(typeof(AdobeStartCallback))]
    public static void HandleStartAdobeCallback()
    {
        ACPCore.ConfigureWithAppID("94f571f308d5/7376e8bb5591/launch-15ec923b1cfa-development");    
    }

    [MonoPInvokeCallback(typeof(AdobeGetQueueSizeCallback))]
    public static void HandleAdobeGetQueueSizeCallback(long queueSize)
    {
        Debug.Log("Queue size is : " + queueSize);
    }

    [MonoPInvokeCallback(typeof(AdobeGetTrackingIdentifierCallback))]
    public static void HandleAdobeGetTrackingIdentifierCallback(string trackingIdentifier)
    {
        Debug.Log("Tracking identifier is : " + trackingIdentifier);
        
    }

    [MonoPInvokeCallback(typeof(AdobeGetVisitorIdentifierCallback))]
    public static void HandleAdobeGetVisitorIdentifierCallback(string visitorIdentifier)
    {
        Debug.Log("Visitor identifier is : " + visitorIdentifier);
        callbackResults.text = visitorIdentifier;
    }


    void Start()
    {
        if (Application.platform == RuntimePlatform.Android) {
            ACPCore.SetApplication();
        }
        ACPCore.SetLogLevel(ACPCore.ACPMobileLogLevel.VERBOSE);
        ACPIdentity.registerExtension();
        ACPAnalytics.RegisterExtension();
        ACPCore.Start(HandleStartAdobeCallback);

        var callbackResultsGameObject = GameObject.Find("CallbackResults");
        callbackResults = callbackResultsGameObject.GetComponent<Text>();

        btnExtensionVersion.onClick.AddListener(analyticsExtensionVersion);
        btnSendQueuedHits.onClick.AddListener(sendQueuedHits);
        btnClearQueue.onClick.AddListener(clearQueue);
        btnGetQueueSize.onClick.AddListener(getQueueSize);
        btnGetTrackingIdentifier.onClick.AddListener(getTrackingIdentifier);
        btnGetVisitorIdentifier.onClick.AddListener(getVisitorIdentifier);
        btnSetVisitorIdentifier.onClick.AddListener(setVisitorIdentifier);
    }

    void analyticsExtensionVersion()
	{
        Debug.Log("Calling Analytics extensionVersion");
		string analyticsExtensionVersion = ACPAnalytics.ExtensionVersion();
        Debug.Log("Analytics extension version : " + analyticsExtensionVersion);
        callbackResults.text = analyticsExtensionVersion;
    }

    void sendQueuedHits()
    {
        Debug.Log("Calling sendQueuedHits");
        ACPAnalytics.SendQueuedHits();
    }

    void clearQueue()
    {
        Debug.Log("Calling clearQueue");
        ACPAnalytics.ClearQueue();
    }

    void getQueueSize()
    {
        Debug.Log("Calling getQueueSize");
        ACPAnalytics.GetQueueSize(HandleAdobeGetQueueSizeCallback);
    }

    void getTrackingIdentifier()
    {
        Debug.Log("Calling getTrackingIdentifier");
        ACPAnalytics.GetTrackingIdentifier(HandleAdobeGetTrackingIdentifierCallback);
    }

    void setVisitorIdentifier()
    {
        Debug.Log("Calling setVisitorIdentifier");
        ACPAnalytics.SetVisitorIdentifier(visitorIdentifier.text);
    }

    void getVisitorIdentifier()
    {
        Debug.Log("Calling getVisitorIdentifier");
        ACPAnalytics.GetVisitorIdentifier(HandleAdobeGetVisitorIdentifierCallback);
    }
}
