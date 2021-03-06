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

- Download [ACPAnalytics-0.0.1-Unity.zip](https://github.com/adobe/unity-acpcore/tree/master/bin/ACPAnalytics-0.0.1-Unity.zip) 
- Unzip`ACPAnalytics-0.0.1-Unity.zip`
- Import `ACPAnalytics.unitypackage` via Assets->Import Package
## Usage

### [Analytics](https://aep-sdks.gitbook.io/docs/using-mobile-extensions/adobe-analytics)

#### Initialization
##### Initialize by registering the extensions and calling the start function for core
```
using com.adobe.marketing.mobile;
using AOT;

[MonoPInvokeCallback(typeof(AdobeStartCallback))]
public static void HandleStartAdobeCallback()
{
    ACPCore.ConfigureWithAppID("1423ae38-8385-8963-8693-28375403491d");    
}

public class MainScript : MonoBehaviour
{
    // Start is called before the first frame update
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
Sample App is located at *unity-acpanalytics/ACPAnalytics/Assets/Demo*.
To build demo app for specific platform follow the below instructions.

###### Add core plugin
- Download [ACPCore-0.0.1-Unity.zip](https://github.com/adobe/unity-acpcore/tree/master/bin/ACPCore-0.0.1-Unity.zip) 
- Unzip `ACPCore-0.0.1-Unity.zip`
- Import `ACPCore.unitypackage` via Assets->Import Package

###### Android
1. Make sure you have an Android device connected.
1. From the menu of the `Unity` app, select __File > Build Settings...__
1. Select `Android` from the __Platform__ window
1. If `Android` is not the active platform, hit the button that says __Switch Platform__ (it will only be available if you actually need to switch active platforms)
1. Press the __Build And Run__ button
2. You will be asked to provide a location to save the build. Make a new directory at *unity-acpanalytics/ACPAnalytics/Builds* (this folder is in the .gitignore file)
3. Name build whatever you want and press __Save__
4. `Unity` will build an `apk` file and automatically deploy it to the connected device

###### iOS
1. From the menu of the `Unity` app, select __File > Build Settings...__
1. Select `iOS` from the __Platform__ window
1. If `iOS` is not the active platform, hit the button that says __Switch Platform__ (it will only be available if you actually need to switch active platforms)
1. Press the __Build And Run__ button
1. You will be asked to provide a location to save the build. Make a new directory at *unity-acpanalytics/ACPAnalytics/Builds* (this folder is in the .gitignore file)
1. Name build whatever you want and press __Save__
1. `Unity` will create and open an `Xcode` project
1. From the Xcode project run the app on a simulator.
1. If you get an error `Symbol not found: _OBJC_CLASS_$_WKWebView`. Select the Unity-iPhone target -> Go to Build Phases tab -> Add `Webkit.Framework` to `Link Binary with Libraries`.

## Additional Cordova Plugins

Below is a list of additional Unity plugins from the AEP SDK suite:

| Extension | GitHub | Unity Package |
|-----------|--------|-----|
| Core SDK | https://github.com/adobe/unity-acpcore | [ACPCore](https://github.com/adobe/unity-acpcore/raw/master/bin/ACPCore-0.0.1-Unity.zip)
| Project Griffon (Beta) | https://github.com/adobe/unity-acpgriffon | [ACPGriffon](https://github.com/adobe/unity-acpgriffon/raw/master/bin/ACPGriffon-0.0.1-Unity.zip)

## Contributing
Looking to contribute to this project? Please review our [Contributing guidelines](.github/CONTRIBUTING.md) prior to opening a pull request.

We look forward to working with you!

## Licensing
This project is licensed under the Apache V2 License. See [LICENSE](LICENSE) for more information.
