# Two Factor Authenticator App in WPF

WPF desktop app of a 2 factor authenticator that scans a QR code one time and then generates a 6 digit code every 30 seconds.

----------

The extra libraries needed to run this app are included as an embedded resource so the app can run as a single .exe file in a folder without dll's.
These are loaded in the App.xaml.cs > Main() with AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>

This project also uses the 2 following libraries (https://www.nuget.org/packages/CredentialManagement & https://www.nuget.org/packages/StickyWindows.WPF/0.3.0-unstable0009) but are not installed with the package manager.
To include libraries as resource they need to be strongly named but are not when installing from NuGet.
In order to maintain transparency i did not download and recompile those libraries myself, but included them as code files so everyone can see that no changes were made.

More info: https://www.vanderwaal.eu/mini-projecten/two-factor-authenticator-app-wpf
