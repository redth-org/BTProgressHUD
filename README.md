BTProgressHUD
=============

__If you have an issue, file it here, but also email me__ - nicw@fastchicken.co.nz - or @fastchicken - as GitHub doesn't notify me! Thanks!.

Port to [MonoTouch](http://xamar.in/r/nicwise/store.xamarin.com) of the [SVProgressHUD](https://github.com/samvermette/SVProgressHUD). 

SVProgressHUD (and BTProgressHUD) is a clean and easy-to-use HUD meant to display the progress of an ongoing task.

![SVProgressHUD](http://f.cl.ly/items/3r2x0b1E1O2F0V422a3R/screenshots2.png)

(Image from SVProgressHUD)

This is also available in the [Xamarin Component Store](https://components.xamarin.com/view/btprogresshud/). 

If you need this for Xamarin.Android, [redth has a very similar component](https://github.com/Redth/AndHUD)

__Now includes iOS7-style HUD if the app is running on iOS7__

You can force iOS6 or iOS7 style in the IsiOS7 property in ProgressHUD.cs



##Installation

* Grab the .cs files, and the images, and add them to your project.
* Use it :)

##TODO

* The progress HUD is not Accessability-aware, which would be very good to add. The ObjC code for this is still in the project, just commented out.

##Usage

Have a look at the BTProgressHUDDemo project.

There are a few main static methods, however:

```csharp
BTProgressHUD.Show(); //shows the spinner
BTProgressHUD.Show(status: "Oh hai"); //show spinner + text
BTProgressHUD.ShowSuccessWithStatus("Wow, that worked"); //A big TICK with text
BTProgressHUD.ShowErrorWithStatus("Fail!"); //A big CROSS with text
BTProgressHUD.ShowToast("Hello from Toast"); //show an Android-style toast
```
All of these can be dismissed with

```csharp
BTProgressHUD.Dismiss();
```

BTProgressHUD is aware of the thread you are calling from, and ensures that HUDs are always manipulated from the UI thread.

If you need to make your own instance of the HUD, you can make a new ProgressHUD.

###Other Show options

You can call Show with the following parameters
* status: <string> - show status text
* progress: <float> - show a progress circle with 0.0 - 1.0 of progress. Call again to change the progress.
* maskType: <ProgressHUD.MaskType> - show with the background (the whole window) clear, black or gradient. Default is none, which allows interaction with the underlying elements.

```csharp
public enum MaskType
{
	None = 1, // allow user interactions, don't dim background UI (default)
	Clear, // disable user interactions, don't dim background UI
	Black, // disable user interactions, dim background UI with 50% translucent black
	Gradient // disable user interactions, dim background UI with translucent radial gradient (a-la-alertView)
}
```

###ShowToast
The toast can be centered or at the top or bottom of the screen. This is controlled by the second parameter.

```csharp
BTProgressHUD.ShowToast("foo", toastPosition: ProgressHUD.ToastPosition.Center);
```

###ShowSuccess/Error/ShowImage
This method dismisses the activity after 1 second. You can provide your own images if needed - make them 28x28 white PNGs.

```csharp
BTProgressHUD.ShowSuccessWithStatus("Wow, that worked"); //A big TICK with text
BTProgressHUD.ShowErrorWithStatus("Fail!"); //A big CROSS with text
BTProgressHUD.ShowImage(UIImage.FromFile(…), "Nice one Stu!");
```

You can use the timeout parameter of ShowImage to control the time before it's dismissed.


## Credits

SVProgressHUD is brought to you by [Sam Vermette](http://samvermette.com) and [contributors to the project](https://github.com/samvermette/SVProgressHUD/contributors). The success and error icons are from [Glyphish](http://glyphish.com/). If you have feature suggestions or bug reports, feel free to help out by sending pull requests or by [creating new issues](https://github.com/samvermette/SVProgressHUD/issues/new). If you're using SVProgressHUD in your project, attribution would be nice.

BTProgressHUD is brought to you by [Nic Wise](http://www.fastchicken.co.nz/). I'm also happy to take bug reports and pull 
requests for the MonoTouch version. If you use BTProgressHUD in your project, attribution would also be 
nice - or [tweet](http://twitter.com/fastchicken) me a link to your project when it's live.

Thanks to 

* [mloenow](https://github.com/mloenow)
* [Ramesh Sringeri](https://github.com/idispose) 
* [Danny Cabrera](https://github.com/dannycabrera)
* [Greg Shackles](https://github.com/gshackles)

for the updates / fixes.
