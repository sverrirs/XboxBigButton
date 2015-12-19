window.outdiv = document.getElementById("device");
window.DEVICE_INFO = { "vendorId": 1118, "productId": 672, "interfaceId": 0 };
window.usbConnection = null;

function arrayBufferToString(array) 
{
    if( !array )
      return "";
    
    return String.fromCharCode.apply(null, new Uint8Array(array));
}

function myDevicePoll() 
{
  if( !window.usbConnection )
    return;
    
  var transferInfo = {
    "direction": "in"
    ,"endpoint": 129 // the endpoint 'address'
    ,"length": 5  // length in bytes
  };
  
  // Claim the necessary interface
  chrome.usb.claimInterface(window.usbConnection, 0, function()
  { 
    chrome.usb.interruptTransfer(window.usbConnection, transferInfo, function(usbEvent) 
    {
      try
      {
        console.log(usbEvent);
        
        if (usbEvent != null) 
        {
          console.log("Data length: "+usbEvent.data.byteLength);
          
          if( usbEvent.resultCode == 0 )
          {
            if( usbEvent.data.byteLength > 0 )
            {
              var dataView = new Uint8Array(usbEvent.data);
              var decoded = dataView[0]+":"+dataView[1]+":"+dataView[2]+":"+dataView[3]+":"+dataView[4];
              //var decoded = arrayBufferToString(usbEvent.data);
              window.outdiv.innerHTML = window.outdiv.innerHTML + "<br/>"+ decoded;
              console.log('Raw Data: ');
              console.log(usbEvent.data);
              console.log('Data (decoded): ' + decoded);
            }
          }
          /*else
          {
            console.warn(chrome.runtime.lastError);
          }*/
          
          // Next data retrieval
          setTimeout(myDevicePoll, 0);
        }
      }
      catch(ex)
      {
        console.error("error in interruptTransfer");
        console.error(ex);
      }
      finally
      {
        // Release the interface
        chrome.usb.releaseInterface(window.usbConnection, 0, function(){} );
      }
    });
  });
}

function initializeUsbDevice() 
{
    // Try to open the USB device
    chrome.usb.findDevices(window.DEVICE_INFO, function(devices) 
    {
      window.devices=devices;
      if (devices) 
      {
        if (devices.length > 0) {
          console.log("Device(s) found: "+devices.length);
        } else {
          console.log("Device could not be found");
          return;
        }
      } 
      else 
      {
        console.log("Permission denied, error enumerating devices.");
        return;
      }

      console.log('First device: ');
      console.log(window.devices[0]);
      
      window.usbConnection = window.devices[0];
      
      // List the interfaces
      /*chrome.usb.listInterfaces(window.usbConnection, function( interfaces )
        {
          console.log("interfaces.length="+interfaces.length);
          
          for(i=0; i<interfaces.length;i++)
          {
            var interf = interfaces[i]
            console.log("Interface id="+interf.interfaceNumber+" Class="+interf.interfaceClass+" Subclass="+interf.interfaceSubclass+" Protocol="+interf.interfaceProtocol+" Description="+interf.description + " Endpoints="+interf.endpoints);
            
            for(e=0; e<interf.endpoints.length;e++)
            {
              var endp = interf.endpoints[e];
              console.log("  => Endpoint "+e+": Address="+endp.address+" type="+endp.type+" direction="+endp.direction+" maximumPacketSize="+endp.maximumPacketSize+" usage="+endp.usage+" pollingInterval="+endp.pollingInterval+" extra="+arrayBufferToString(endp.extra_data));
            }
          }
        }
      );   */

      // Start polling the device for data
      console.log("Device opened, starting poll");
      window.outdiv.innerHTML = "Device open, interface 0 claimed, listening";
      myDevicePoll();      
    });
}


var requestButton = document.getElementById("requestPermission");

requestButton.addEventListener('click', function()
{
  // Must use the request permission as a direct result of an action initiated by the user
  // required by Chrome this is!
  chrome.permissions.request( 
    {permissions: [{'usbDevices': [window.DEVICE_INFO] }]}, 
    function(result) 
    {
      if (result) 
      { 
        console.log('App was granted the "usbDevices" permission.');
        initializeUsbDevice();
      } else {
        console.log('App was NOT granted the "usbDevices" permission.');
      }
    });
});