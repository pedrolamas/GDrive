# GDrive

### [Official Site/Blog][1] - [@pedrolamas][2]

This is my personal approach for a Google Drive application for Windows Phone!

It is not intended to be a full featured Google Drive client, but rather a way for me to show how I normally code my Windows Phone apps with an MVVM architecture, and also how you can easily create a Google Drive client for Windows Phone (since Google didn't build one themselves...)

Please feel free to fork, download, copy, change, alter, comment, trash...

## Project status

The application is using Google Drive API version 2.

Currently, this is what you can do:

* Multiple accounts support
* OAuth 2 account login
* List files and folders with their metadata
* Navigate the folder tree
* Create new folders
* Upload files (just pictures, that's what Windows Phone SDK allows us to do!)
* Star/Unstar files
* Rename files
* Trash files
* ...

On my backlog, this is what you can find:

* Copy/Move files
* Improve caching
* Implement some sort of service call stack
* Use Google API RPC calls (in order to group requests)
* Fix memory leaks and other issues!
* Grab some [cimbalino][4] expresso coffee...

## Setup

The project requires the following external components to work:

* [MVVMLight Toolkit][6]
* [Cimbalino Windows Phone Toolkit][7]
* [Windows Phone Toolkit][8]
* [HttpClient][9]
* [Json.net][10]

To make things easier, the project is configured with [NuGet][5] ability to restore missing packages (available since version 1.6). You can read [here][11] about this feature and how you can activate it in your Visual Studio.

Also, before running the code you'll need to get access to the Google Drive API:
 - Enter the Google APIs Console [here][12].
 - Go to Services and activate the Drive API. Read the presented Terms of Service, check the *"I agree"* checkbox and click *"Accept"*.
 - Go to API Access, and click on *"Create an OAuth 2.0 client ID..."*.
 - Enter a product name, click *"Next"*.
 - Check *"Installed application"* and *"Other"* on the *"Application type"* and *"Installed application type"* option groups respectively. Click on *"Create client ID"*.
 - You can now copy the **Client ID** and **Client secret** values back to the **ViewModelLocator.cs**, updating the two constants on top of the file, with these values.
 - Build the code, run the app, test it now!

## Reference

 - [Google Drive SDK API Reference][13]
 - [Google APIs Explorer][14]
 - [Google Accounts Authentication and Authorization: Using OAuth 2.0 for Login][15]

## License

See the [LICENSE.txt][3] file for details.

[1]: http://www.pedrolamas.com
[2]: http://twitter.com/pedrolamas
[3]: https://github.com/pedrolamas/GDrive/raw/master/LICENSE.txt "License"
[4]: https://github.com/Cimbalino/Cimbalino-Phone-Toolkit/wiki/Cimbalino%3F%3F
[5]: http://nuget.org/
[6]: http://mvvmlight.codeplex.com/
[7]: http://cimbalino.org
[8]: http://phone.codeplex.com/
[9]: http://blogs.msdn.com/b/bclteam/p/httpclient.aspx
[10]: http://james.newtonking.com/projects/json-net.aspx
[11]: http://docs.nuget.org/docs/workflows/using-nuget-without-committing-packages
[12]: http://code.google.com/apis/console
[13]: https://developers.google.com/drive/v2/reference/
[14]: https://developers.google.com/apis-explorer/#p/drive/v2/
[15]: https://developers.google.com/accounts/docs/OAuth2Login/