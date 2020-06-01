/*
Copyright 2020 Adobe
All Rights Reserved.
NOTICE: Adobe permits you to use, modify, and distribute this file in
accordance with the terms of the Adobe license agreement accompanying
it. If you have received this file from a source other than Adobe,
then your use, modification, or distribution of it requires the prior
<<<<<<< HEAD
written permission of Adobe.
=======
written permission of Adobe. (See LICENSE-MIT for details)
>>>>>>> eda3c41... Adding MIT license in demo dir and adding notices
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using com.adobe.marketing.mobile;
using System;
using AOT;
using System.Threading;

public class SceneScript : MonoBehaviour
{
    public static String results;
    public Text callbackResultsText;

    // Analytics Buttons
    public Button btnExtensionVersion;
    public Button btnSendQueuedHits;
    public Button btnClearQueue;
    public Button btnGetQueueSize;
    public Button btnGetTrackingIdentifier;
    public Button btnSetVisitorIdentifier;
    public Button btnGetVisitorIdentifier;
    public Button btnBatchAnalyticsHits;
    public InputField visitorIdentifier;
    public static Text callbackResults;
    static CountdownEvent latch;

    // Analytics callbacks
    [MonoPInvokeCallback(typeof(AdobeStartCallback))]
    public static void HandleStartAdobeCallback()
    {
        ACPCore.ConfigureWithAppID("94f571f308d5/00fc543a60e1/launch-c861fab912f7-development");    
    }

    [MonoPInvokeCallback(typeof(AdobeGetQueueSizeCallback))]
    public static void HandleAdobeGetQueueSizeCallback(long queueSize)
    {
        Debug.Log("Queue size is : " + queueSize);
        results = "Queue size is : " + queueSize;
    }

    [MonoPInvokeCallback(typeof(AdobeGetTrackingIdentifierCallback))]
    public static void HandleAdobeGetTrackingIdentifierCallback(string trackingIdentifier)
    {
        Debug.Log("Tracking identifier is : " + trackingIdentifier);
        results = "Tracking identifier is : " + trackingIdentifier;
    }

    [MonoPInvokeCallback(typeof(AdobeGetVisitorIdentifierCallback))]
    public static void HandleAdobeGetVisitorIdentifierCallback(string visitorIdentifier)
    {
        Debug.Log("Visitor identifier is : " + visitorIdentifier);
        results = "Visitor identifier is : " + visitorIdentifier;
    }

    [MonoPInvokeCallback(typeof(AdobeGetQueueSizeCallback))]
    public static void HandleAdobeBatchedGetQueueSizeCallback(long queueSize)
    {
        Debug.Log("Queue size is : " + queueSize);
        results = "Queue size is : " + queueSize;
        if (latch != null)
        {
            latch.Signal();
        }
    }

    private void Update()
    {
        callbackResultsText.text = results;
    }

    void Start()
    {
        if (Application.platform == RuntimePlatform.Android) {
            ACPCore.SetApplication();
        }
        ACPCore.SetLogLevel(ACPCore.ACPMobileLogLevel.VERBOSE);
        ACPCore.SetWrapperType();
        ACPIdentity.RegisterExtension();
        ACPAnalytics.RegisterExtension();
        ACPCore.Start(HandleStartAdobeCallback);

        var callbackResultsGameObject = GameObject.Find("CallbackResults");
        callbackResultsText = callbackResultsGameObject.GetComponent<Text>();

        btnExtensionVersion.onClick.AddListener(AnalyticsExtensionVersion);
        btnSendQueuedHits.onClick.AddListener(SendQueuedHits);
        btnClearQueue.onClick.AddListener(ClearQueue);
        btnGetQueueSize.onClick.AddListener(GetQueueSize);
        btnGetTrackingIdentifier.onClick.AddListener(GetTrackingIdentifier);
        btnGetVisitorIdentifier.onClick.AddListener(GetVisitorIdentifier);
        btnSetVisitorIdentifier.onClick.AddListener(SetVisitorIdentifier);
        btnBatchAnalyticsHits.onClick.AddListener(BatchAnalyticsHits);
    }

    void AnalyticsExtensionVersion()
	{
        Debug.Log("Calling Analytics extensionVersion");
		string analyticsExtensionVersion = ACPAnalytics.ExtensionVersion();
        Debug.Log("Analytics extension version : " + analyticsExtensionVersion);
        results = "Analytics extension version : " + analyticsExtensionVersion;
    }

    void SendQueuedHits()
    {
        Debug.Log("Calling sendQueuedHits");
        ACPAnalytics.SendQueuedHits();
    }

    void ClearQueue()
    {
        Debug.Log("Calling clearQueue");
        ACPAnalytics.ClearQueue();
    }

    void GetQueueSize()
    {
        Debug.Log("Calling getQueueSize");
        ACPAnalytics.GetQueueSize(HandleAdobeGetQueueSizeCallback);
    }

    void GetTrackingIdentifier()
    {
        Debug.Log("Calling getTrackingIdentifier");
        ACPAnalytics.GetTrackingIdentifier(HandleAdobeGetTrackingIdentifierCallback);
    }

    void SetVisitorIdentifier()
    {
        Debug.Log("Calling setVisitorIdentifier");
        ACPAnalytics.SetVisitorIdentifier(visitorIdentifier.text);
    }

    void GetVisitorIdentifier()
    {
        Debug.Log("Calling getVisitorIdentifier");
        ACPAnalytics.GetVisitorIdentifier(HandleAdobeGetVisitorIdentifierCallback);
    }

    //used for testing
    void BatchAnalyticsHits()
    {
        //setup
        latch = new CountdownEvent(1);

        //set batch limit to 5
        Dictionary<string,object> config = new Dictionary<string, object>();
        config.Add("analytics.batchLimit", "5");
        ACPCore.UpdateConfiguration(config);

        Thread.Sleep(1000);

        Dictionary<string,string> contextData = new Dictionary<string, string>();
        contextData.Add("contextdata", "data");
        ACPCore.TrackAction("action", contextData);
        ACPCore.TrackAction("action", contextData);
        ACPCore.TrackAction("action", contextData);

        //get queue size for batches hits
        ACPAnalytics.GetQueueSize(HandleAdobeBatchedGetQueueSizeCallback);
        latch.Wait();

        //cleanup
        latch.Dispose();
        latch = null;
    }
}
