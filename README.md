BTProgressHUD
=============

BTProgressHUD is a clean and easy-to-use HUD meant to display the progress of an ongoing task.

If you need a similar library for Android, [AndHUD is very similar component](https://github.com/redth-org/AndHUD)

## Installation

Install the NuGet package [BTProgressHUD][nuget]

## TODO

* The progress HUD is not Accessibility-aware, which would be very good to add.

## Usage

Have a look at the BTProgressHUDDemo project.

There are a few main static methods, however:

```csharp
BTProgressHUD.Show(); //shows the spinner
BTProgressHUD.Show(status: "Oh hai"); //show spinner + text
BTProgressHUD.ShowSuccessWithStatus("Wow, that worked"); //A big TICK with text
BTProgressHUD.ShowErrorWithStatus("Fail!"); //A big CROSS with text
BTProgressHUD.ShowInfoWithStatus("Info!"); //A big I with text
BTProgressHUD.ShowToast("Hello from Toast"); //show an Android-style toast
```
All of these can be dismissed with

```csharp
BTProgressHUD.Dismiss();
```

BTProgressHUD is aware of the thread you are calling from, and ensures that HUDs are always manipulated from the UI thread.

If you need to make your own instance of the HUD, you can make a new ProgressHUD.

### Other Show options

You can call Show with the following parameters
* status: <string> - show status text
* progress: <float> - show a progress circle with 0.0 - 1.0 of progress. Call again to change the progress.
* maskType: <MaskType> - show with the background (the whole window) clear, black or gradient. Default is none, which allows interaction with the underlying elements.
* imageStyle: <ImageStyle> - the type of image is used in the dialog, default, outline and outline full 

```csharp
public enum MaskType
{
	None = 1, // allow user interactions, don't dim background UI (default)
	Clear, // disable user interactions, don't dim background UI
	Black, // disable user interactions, dim background UI with 50% translucent black
	Gradient // disable user interactions, dim background UI with translucent radial gradient (a-la-alertView)
}
```

```csharp
public enum ImageStyle
{
    Default, // icon without an outline
    Outline, // icon with an outline
    OutlineFull // icon with a full outline
}
```

### ShowToast
The toast can be centered or at the top or bottom of the screen. This is controlled by the second parameter.

```csharp
BTProgressHUD.ShowToast("foo", toastPosition: ToastPosition.Center);
```

### ShowSuccess/ShowError/ShowInfo/ShowImage
This method dismisses the activity after 1 second. You can provide your own images if needed

```csharp
BTProgressHUD.ShowSuccessWithStatus("Wow, that worked"); //A big TICK with text
BTProgressHUD.ShowErrorWithStatus("Fail!"); //A big CROSS with text
BTProgressHUD.ShowInfoWithStatus("Info!"); //A big I with text
BTProgressHUD.ShowImage(UIImage.FromFile(…), "Nice one Stu!");
```

You can use the timeout parameter of ShowImage to control the time before it's dismissed.

## Customization
You can customize the appearance of the ProgressHUD globally through the `ProgressHUDAppearance` class, where you can
control the following options:

- Corner Radius
- Background Color
- Background Visual Effect (overrides background color)
- Border Thickness
- Border Color
- Image Tint Color
- Text Font
- Button Font
- Text Color
- Button Text Color
- Text Alignment
- Ring Radius
- Ring Stroke Thickness
- Ring Color
- Images and the various outlined/full versions for:
  - Error
  - Success
  - Info

## Credits

SVProgressHUD which this library is originally based on is brought to you by [Sam Vermette](http://samvermette.com) and [contributors to the project](https://github.com/samvermette/SVProgressHUD/contributors). 

The success, error and info icons are from [Human Interface Guidelines](https://developer.apple.com/design/human-interface-guidelines/sf-symbols/overview/) from Apple.
        
[nuget]: https://www.nuget.org/packages/BTProgressHUD/
