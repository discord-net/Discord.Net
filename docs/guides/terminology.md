# Terminology

## Preface

Most terms for objects remain the same between 0.9 and 1.0. The major difference is that the ``Server`` is now called ``Guild``, to stay in line with Discord internally

## Introduction to Interfaces

Discord.Net 1.0 is built strictly around Interfaces. There are no methods that return a concrete object, only an interface. 

Many of the interfaces in Discord.Net are linked through inheritance. For example, @Discord.IChannel represents any channel in Discord. @Discord.IGuildChannel inherits from IChannel, and represents all channels belonging to a Guild. As a result, @Discord.IChannel can sometimes be cast to @Discord.IGuildChannel, and you may find yourself doing this frequently in order to properly utilize the library.

### The Inheritance Tree

You may want to familiarize yourself with the inheritance in Discord.Net. An inheritance tree is provided below.

![](https://i.lithi.io/kpgd.png)
![](https://i.lithi.io/kNrr.png)
![](https://i.lithi.io/gs8d.png)
![](https://i.lithi.io/LAJr.png)