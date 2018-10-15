---
uid: Guides.Deployment
title: Deploying the Bot
---

# Deploying a Discord.Net Bot

After finishing your application, you may want to deploy your bot to a
remote location such as a Virtual Private Server (VPS) or another
computer so you can keep the bot up and running 24/7.

## Recommended VPS

For small-medium scaled bots, a cheap VPS (~$5) might be sufficient
enough. Here is a list of recommended VPS provider.

* [DigitalOcean](https://www.digitalocean.com/)
  * Description: American cloud infrastructure provider headquartered
    in New York City with data centers worldwide.
  * Location(s):
    * Asia: Singapore, India
    * America: Canada, United States
    * Europe: Netherlands, Germany, United Kingdom
  * Based in: United States
* [Vultr](https://www.vultr.com/)
  * Description: DigitalOcean-like
  * Location(s):
    * Asia: Japan, Australia, Singapore
    * America: United States
    * Europe: United Kingdom, France, Netherlands, Germany
  * Based in: United States
* [OVH](https://www.ovh.com/)
  * Description: French cloud computing company that offers VPS,
    dedicated servers and other web services.
  * Location(s):
    * Asia: Australia, Singapore
    * America: United States, Canada
    * Europe: United Kingdom, Poland, Germany
  * Based in: Europe
* [Scaleway](https://www.scaleway.com/)
  * Description: Cheap but powerful VPS owned by [Online.net](https://online.net/).
  * Location(s):
    * Europe: France, Netherlands
  * Based in: Europe
* [Time4VPS](https://www.time4vps.eu/)
  * Description: Affordable and powerful VPS Hosting in Europe.
  * Location(s):
    * Europe: Lithuania
  * Based in: Europe

## .NET Core Deployment

> [!NOTE]
> This section only covers the very basics of .NET Core deployment.
> To learn more about .NET Core deployment, 
> visit [.NET Core application deployment] by Microsoft.

When redistributing the application - whether for deployment on a
remote machine or for sharing with another user - you may want to
publish the application; in other words, to create a
self-contained package without installing the dependencies
and the runtime on the target machine.

### Framework-dependent Deployment

To deploy a framework-dependent package (i.e. files to be used on a
remote machine with the `dotnet` command), simply publish
the package with:

* `dotnet publish -c Release`

This will create a package with the **least dependencies**
included with the application; however, the remote machine
must have `dotnet` runtime installed before the remote could run the
program.

> [!TIP]
> Do not know how to run a .NET Core application with 
> the `dotnet` runtime? Navigate to the folder of the program and 
> enter `dotnet program.dll` where `program.dll` is your compiled
> binaries.

### Self-contained Deployment

To deploy a self-contained package (i.e. files to be used on a remote
machine without the `dotnet` runtime), publish with a specific
[Runtime ID] with the `-r` switch.

This will create a package with dependencies compiled for the target
platform, meaning that all the required dependencies will be included
with the program. This will result in **larger package size**; 
however, not only is the portabilitiy greatly increased, but also the
it will include a copy of the executable that can be run
natively on the target runtime.

For example, the following command will create a Windows 
executable (`.exe`) that is ready to be executed on any
Windows 10 x64 based machine:

* `dotnet publish -c Release -r win10-x64`

[.NET Core application deployment]: https://docs.microsoft.com/en-us/dotnet/core/deploying/
[Runtime ID]: https://docs.microsoft.com/en-us/dotnet/core/rid-catalog