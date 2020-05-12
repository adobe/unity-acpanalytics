# Adobe Experience Platform - Analytics plugin for Unity apps

- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Usage](#usage)
    - [Initialization](#initialization)
    - [Analytics methods](#Analytics-methods)
- [Running Tests](#running-tests)
- [Sample App](#sample-app)
- [Contributing](#contributing)
- [Licensing](#licensing)

## Prerequisites

The `Unity Hub` application is required for development and testing. Inside of `Unity Hub`, you will be required to download the current version of the `Unity` app.

[Download the Unity Hub](http://unity3d.com/unity/download). The free version works for development and testing, but a Unity Pro license is required for distribution. See [Distribution](#distribution) below for details.

#### FOLDER STRUCTURE
Plugins for a Unity project use the following folder structure:

`{Project}/Assets/Plugins/{Platform}`

## Installation
- Download [ACPCore-0.0.1-Unity.zip](https://github.com/adobe/unity-acpcore/tree/master/bin/ACPCore-0.0.1-Unity.zip) 
- Unzip `ACPCore-0.0.1-Unity.zip`
- Import `ACPCore.unitypackage` via Assets->Import Package

- Download [ACPAnalytics-0.0.1-Unity.zip](https://github.com/adobe/unity-acpcore/tree/master/ACPAnalytics/bin/ACPAnalytics-0.0.1-Unity.zip) 
- Unzip`ACPAnalytics-0.0.1-Unity.zip`
- Import `ACPAnalytics.unitypackage` via Assets->Import Package
## Usage

### [Analytics](https://aep-sdks.gitbook.io/docs/using-mobile-extensions/adobe-analytics)

#### Initialization
##### Initialize by registering the extensions and calling the start function for core
```
using com.adobe.marketing.mobile;
using using AOT;

public class MainScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {   
        if (Application.platform == RuntimePlatform.Android) {
            ACPCore.SetApplication();
        }
        
        ACPCore.SetLogLevel(ACPCore.ACPMobileLogLevel.VERBOSE);
        ACPIdentity.RegisterExtension();
        ACPLifecycle.RegisterExtension();
        ACPSignal.RegisterExtension();
        ACPAnalytics.RegisterExtension();
        ACPCore.Start(HandleStartAdobeCallback);
    }
}
```

#### Analytics methods

##### Getting Analytics version:
```cs
ACPAnalytics.ExtensionVersion();
```

#### Send queued hits:
```cs
ACPAnalytics.SendQueuedHits();
```

#### Clear queued hits:
```cs
ACPAnalytics.ClearQueue();
```

#### Get the queue size:
```cs
[MonoPInvokeCallback(typeof(AdobeGetQueueSizeCallback))]
public static void HandleAdobeGetQueueSizeCallback(long queueSize)
{
    Debug.Log("Queue size is : " + queueSize);
}
ACPAnalytics.GetQueueSize(HandleAdobeGetQueueSizeCallback);
```

#### Get the tracking identifier
```cs
[MonoPInvokeCallback(typeof(AdobeGetTrackingIdentifierCallback))]
public static void HandleAdobeGetTrackingIdentifierCallback(string trackingIdentifier)
{
    Debug.Log("Tracking identifier is : " + trackingIdentifier);
}
ACPAnalytics.GetTrackingIdentifier(HandleAdobeGetTrackingIdentifierCallback);
```

#### Set the custom visitor identifier
```cs
ACPAnalytics.SetVisitorIdentifier("VisitorIdentifier");
```

#### Get the custom visitor identifier
```cs
[MonoPInvokeCallback(typeof(AdobeGetVisitorIdentifierCallback))]
public static void HandleAdobeGetVisitorIdentifierCallback(string visitorIdentifier)
{
    Debug.Log("Visitor identifier is : " + visitorIdentifier);
}
ACPAnalytics.GetVisitorIdentifier(HandleAdobeGetVisitorIdentifierCallback);
```
## Running Tests
1. Open the demo app in unity.
2. Open the test runner from `Window -> General -> TestRunner`.
3. Click on the `PlayMode` tab.
4. Connect an Android or iOS device as we run the tests on a device in play mode.
5. Select the platform for which the tests need to be run from `File -> Build Settings -> Platform`. 
5. Click `Run all in player (platform)` to run the tests.

## Sample App

## Contributing
Looking to contribute to this project? Please review our [Contributing guidelines](.github/CONTRIBUTING.md) prior to opening a pull request.

We look forward to working with you!

## Licensing
This project is licensed under the Apache V2 License. See [LICENSE](LICENSE) for more information.
