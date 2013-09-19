WinRtHttpClientHandler
======================
An HttpMessageHandler that lets you use the Windows Runtime IHttpFilter types

This could be used in a platform-specific factory method that returns 
a concrete HttpClient for your app's use.

The main motivation for this is due to the need for supporting invalid SSL certificates.
While generally bad practice, there are some cases where you need to connect
to an endpoint without a valid SSL certificate. In .NET, you can either use the WebRequesHandler
or ServicePointManager to override SSL validation. Windows 8.1 does this with its new
Windows.Web.HttpClient. 

The problem occurs if you want to use Portable Class Libraries to share logic between platforms.
In that case, you'll need to use the System.Net.Http.HttpClient, but unfortunately on Windows 8 and 8.1,
there's no way to override SSL Validation.

Hence this WinRtHttpClientHandler. You'll still need a platform-specific factory libraries 
(see the Adaptation/Enlightentment patterns), but from there, you can return
a System.Net.Http.HttpClient to your PCL.

Internally, the WinRtHttpClientHandler uses the Windows 8.1 WinRT HttpClient instead, so 
you have more control over its pipeline.

