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
> To learn more about deployment, visit [.NET Core application deployment]
> by Microsoft.

By default, .NET Core compiles all projects as a DLL file, so that any
.NET Core runtime can execute the application.

You may execute the application via `dotnet myprogram.dll` assuming you
have the dotnet CLI installed.

When redistributing the application, you may want to publish the
application, or in other words, create a self-contained package
for use on another machine without installing the dependencies first.

This can be achieved by using the dotnet CLI too on the development
machine:

* `dotnet publish -c Release`

Additionally, you may want to target a specific platform when
publishing the application so you may use the application without
having to install the Core runtime on the target machine. To do this,
you may specify an [Runtime ID] upon build/publish with the `-r`
option.

For example, when targeting a Windows 10 machine, you may want to use
the following to create the application in Windows executable
format (.exe):

* `dotnet publish -c Release -r win10-x64`

[.NET Core application deployment]: https://docs.microsoft.com/en-us/dotnet/core/deploying/
[Runtime ID]: https://docs.microsoft.com/en-us/dotnet/core/rid-catalog