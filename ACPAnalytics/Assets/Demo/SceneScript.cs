﻿using System.Collections;
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

    //Analytics callbacks
    [MonoPInvokeCallback(typeof(AdobeStartCallback))]
    public static void HandleStartAdobeCallback()
    {   
        if (Application.platform == RuntimePlatform.Android) {
            ACPCore.ConfigureWithAppID("94f571f308d5/7376e8bb5591/launch-15ec923b1cfa-development");    
        } else if (Application.platform == RuntimePlatform.IPhonePlayer) {
            Debug.Log("HandleStartAdobeCallback iphone");
        }
    }

    [MonoPInvokeCallback(typeof(AdobeGetQueueSizeCallback))]
    public static void HandleAdobeGetQueueSizeCallback(long queueSize)
    {
        print("Queue size is : " + queueSize);
    }

    [MonoPInvokeCallback(typeof(AdobeGetTrackingIdentifierCallback))]
    public static void HandleAdobeGetTrackingIdentifierCallback(string trackingIdentifier)
    {
        print("Tracking identifier is : " + trackingIdentifier);
    }

    [MonoPInvokeCallback(typeof(AdobeGetVisitorIdentifierCallback))]
    public static void HandleAdobeGetVisitorIdentifierCallback(string visitorIdentifier)
    {
        print("Visitor identifier is : " + visitorIdentifier);
    }


    void Start()
    {
        ACPCore.SetApplication();
        ACPCore.SetLogLevel(ACPCore.ACPMobileLogLevel.VERBOSE);
        ACPIdentity.registerExtension();
        ACPAnalytics.RegisterExtension();
        ACPCore.Start(HandleStartAdobeCallback);

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
		string analyticsExtensionVersion = ACPAnalytics.AnalyticsExtensionVersion();
        Debug.Log("Analytics extension version : " + analyticsExtensionVersion);
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
