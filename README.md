# XboxBigButton
A C# library and example project showing how to interface with the Xbox 360 Big Button Controllers ([Scene It?](https://en.wikipedia.org/wiki/Scene_It%3F) game controllers)

![The Scene It game kit](http://1.bp.blogspot.com/-lRn76FpzitM/Vh1W2TxJ7mI/AAAAAAAABaw/LVQ4r_PY4ZI/s1600/02.jpg "The Scene It game kit")

The project page can be found at [http://sverrirs.github.io/XboxBigButton/](http://sverrirs.github.io/XboxBigButton/)

<p align="center">
  <a href="https://www.paypal.me/sverrirs/1.5" target="_blank"><img border="0" src="https://www.paypalobjects.com/en_US/i/btn/x-click-but21.gif" alt="Like my work? You can donate to this project using PayPal" title="Like my work? You can donate to this project using PayPal"></a>
</p>

# Driver installer
A downloadable binary for the driver install (both 32 and 64 bit versions) can be found under *XboxBigButton_WinUsbDriver/binaries* folder. 
Note: These installers have not been tested with Win8 and newer so they might not work (due to the fact that Windows 8+ requires .inf files to be signed as well)

## Installing on Windows 10 or newer
When attempting to install this custom driver on a Windows 10 or newer system you can encounter an error similar to `Unknown return value of the DPINST utility` with a error code `0x80010000`. This is due to the new driver signing requirements first introduced in Windows 8 and now enforced in Windows 10 that block any drivers that haven't been signed with Microsofts $1000 hardware driver signature key.

To get around this issue you can follow the procedure below:

>> Installing Unsigned Drivers by Disabling Integrity Checks
>> You can also disable the integrity checks to install unsigned drivers. To disable integrity checks, open the Command Prompt as admin from the Power User menu and execute the below command:
>> 
>> `bcdedit /set nointegritychecks off`
>> 
>> After executing the command, just restart your system, and you can install the unsigned driver on your Windows 10 machine.
>> 
>> Just like with Testing Mode, it is important to enable the integrity checks. To re-enable integrity check, execute the below command as an admin in the command prompt:
>> 
>> `bcdedit /set nointegritychecks on`
>> 
>> Now restart the system and you are good to go.
More info can be found on [this page](https://www.maketecheasier.com/install-unsigned-drivers-windows10/) or [this page](http://forum.flirc.tv/index.php?/topic/141-windows-8/&tab=comments#comment-1117).

# Dependencies
* Uses the [WinUsb.NET](https://github.com/madwizard-thomas/winusbnet/) project 
* Requires the custom driver in the *XboxBigButton_WinUsbDriver* folder to be installed prior to running any of the code.

# Examples
```csharp
var _device = new XboxBigButtonDevice();
_device.ButtonStateChanged += _device_ButtonStateChanged;
_device.Connect();
```

After installing the included driver, the only think you need to do is to create an instance of the Device and connect it. Then your ButtonStateChanged event handler will receive a callback every time the user presses a button on any of the controllers attached.

Please refer to the included example App project for more usage examples.

# Further info
[My blog](http://hardkjarni.blogspot.co.uk/2015/10/how-to-use-xbox-360-big-button.html) goes into some details about the process of getting the custom driver to work.
