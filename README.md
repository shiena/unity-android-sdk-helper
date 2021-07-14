# unity-android-sdk-helper
[![openupm](https://img.shields.io/npm/v/com.shiena.androidsdkhelper?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.shiena.androidsdkhelper/)

![AndroidSDKHelper](Documentaition~/AndroidSDKHelper.png)

## Installation

<details>
<summary>Add from OpenUPM <em>| via scoped registry, recommended</em></summary>

To add OpenUPM to your project:

- open `Edit/Project Settings/Package Manager`
- add a new Scoped Registry:
```
Name: OpenUPM
URL:  https://package.openupm.com/
Scope(s): com.shiena
```
- click <kbd>Save</kbd>
- open Package Manager
- Select ``My Registries`` in dropdown top left
- Select ``Android SDK Helper`` and click ``Install``
</details>

<details>
<summary>Add from GitHub | <em>not recommended, no updates through PackMan</em></summary>

You can also add it directly from GitHub on Unity 2019.4+. Note that you won't be able to receive updates through Package Manager this way, you'll have to update manually.

- open Package Manager
- click <kbd>+</kbd>
- select <kbd>Add from Git URL</kbd>
- paste `https://github.com/shiena/unity-android-sdk-helper.git#1.0.2`
- click <kbd>Add</kbd>
</details>
<https://docs.unity3d.com/Packages/com.unity.package-manager-ui@2.1/manual/index.html>

## Usage

Switch platform to Android.

### Open terminal with android platform tools

Start terminal (Windows: cmd, macOS: Terminal.app) with Android Platform-Tools added to your PATH.
This is useful when you want to use `adb`.

### Open terminal with android sdk tools

Start terminal (Windows: cmd, macOS: Terminal.app) with Android SDK Tools added to your PATH.
This is useful when you want to use `sdkmanager`.
Also, when using the embedded Android SDK on Windows, open cmd with administrator privileges.

## Environment

* OS: Windows or macOS
* Unity with Package Manager: 2018.1 or higher
* Unity without Package Manager: 2017.1 or higher

## LICENSE

[MIT License](LICENSE)
