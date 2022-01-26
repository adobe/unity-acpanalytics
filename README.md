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

The `Unity Hub` application is required for development and testing. Inside of `Unity Hub`, you will be required to download the `Unity` app. The ACPAnalytics Unity package is built using Unity version 2019.4.

[Download the Unity Hub](http://unity3d.com/unity/download). The free version works for development and testing, but a Unity Pro license is required for distribution. See [Distribution](#distribution) below for details.

#### FOLDER STRUCTURE
Plugins for a Unity project use the following folder structure:

`{Project}/Assets/Plugins/{Platform}`

## Installation

#### Installing the ACPCore Unity Package
- Download [ACPCore-1.0.1-Unity.zip](https://github.com/adobe/unity-acpcore/tree/master/bin/ACPCore-1.0.1-Unity.zip) 
- Unzip `ACPCore-1.0.1-Unity.zip`
- Import `ACPCore.unitypackage` via Assets->Import Package

#### Installing the ACPAnalytics Unity Package
- Download [ACPAnalytics-1.0.0-Unity.zip](./bin/ACPAnalytics-1.0.0-Unity.zip) 
- Unzip`ACPAnalytics-1.0.0-Unity.zip`
- Import `ACPAnalytics.unitypackage` via Assets->Import Package

#### Android installation
No additional steps are required for Android installation.

#### iOS installation
ACPCore 1.0.0 and above is shipped with XCFrameworks. Follow these steps to add them to the Xcode project generated when building and running for iOS platform in Unity.
1. Go to File -> Project Settings -> Build System and select `New Build System`.
2. [Download](https://github.com/Adobe-Marketing-Cloud/acp-sdks/tree/master/iOS/ACPCore) `ACPCore.xcframework`, `ACPIdentity.xcframework`, `ACPLifecycle.xcframework` and `ACPSignal.xcframework`.
3. [Download](https://github.com/Adobe-Marketing-Cloud/acp-sdks/tree/master/iOS/ACPAnalytics) `ACPAnalytics.xcframework`.
4. Select the UnityFramework target -> Go to Build Phases tab -> Add the XCFrameworks downloaded in Steps 2 and 3 to `Link Binary with Libraries`.
5. Select the Unity-iPhone target -> Go to Build Phases tab -> Add the XCFrameworks downloaded in Steps 2 and 3 to `Link Binary with Libraries` and `Embed Frameworks`. Alternatively, select `Unity-iPhone` target -> Go to `General` tab -> Add the XCFrameworks downloaded in Steps 2 and 3 to `Frameworks, Libraries, and Embedded Content` -> Select `Embed and sign` option.

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
- Download [ACPCore-1.0.0-Unity.zip](https://github.com/adobe/unity-acpcore/tree/master/bin/ACPCore-1.0.0-Unity.zip) 
- Unzip `ACPCore-1.0.0-Unity.zip`
- Import `ACPCore.unitypackage` via Assets->Import Package

###### Android
1. Make sure you have an Android device connected.
2. From the menu of the `Unity` app, select __File > Build Settings...__
3. Select `Android` from the __Platform__ window
4. If `Android` is not the active platform, hit the button that says __Switch Platform__ (it will only be available if you actually need to switch active platforms)
5. Press the __Build And Run__ button
6. You will be asked to provide a location to save the build. Make a new directory at *unity-acpanalytics/ACPAnalytics/Builds* (this folder is in the .gitignore file)
7. Name build whatever you want and press __Save__
8. `Unity` will build an `apk` file and automatically deploy it to the connected device

###### iOS
1. From the menu of the `Unity` app, select __File > Build Settings...__
2. Select `iOS` from the __Platform__ window
3. If `iOS` is not the active platform, hit the button that says __Switch Platform__ (it will only be available if you actually need to switch active platforms)
4. Press the __Build And Run__ button
5. You will be asked to provide a location to save the build. Make a new directory at *unity-acpanalytics/ACPAnalytics/Builds* (this folder is in the .gitignore file)
6. Name build whatever you want and press __Save__
7. `Unity` will create and open an `Xcode` project
8. [Add XCFrameworks to the Xcode project](#ios-installation).
9. From the Xcode project run the app on a simulator.

## Additional Cordova Plugins

Below is a list of additional Unity plugins from the AEP SDK suite:

| Extension | GitHub | Unity Package |
|-----------|--------|-----|
| ACPCore | https://github.com/adobe/unity-acpcore | [ACPCore](https://github.com/adobe/unity-acpcore/blob/master/bin/ACPCore-1.0.1-Unity.zip)
| AEPAssurance | https://github.com/adobe/unity-aepassurance | [AEPAssurance](https://github.com/adobe/unity-aepassurance/blob/master/bin/AEPAssurance-1.0.0-Unity.zip)
| ACPUserProfile | https://github.com/adobe/unity_acpuserprofile | [ACPUserProfile](https://github.com/adobe/unity_acpuserprofile/blob/master/bin/ACPUserProfile-1.0.0-Unity.zip)

## Contributing
Looking to contribute to this project? Please review our [Contributing guidelines](.github/CONTRIBUTING.md) prior to opening a pull request.

We look forward to working with you!

## Licensing
This project is licensed under the Apache V2 License. See [LICENSE](LICENSE) for more information.
