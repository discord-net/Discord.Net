---
uid: Terminology
title: Terminology
---

# Terminology

## Preface

Most terms for objects remain the same between 0.9 and 1.0. The major difference is that the ``Server`` is now called ``Guild``, to stay in line with Discord internally

## Implementation Specific Entities

Discord.Net 1.0 is split into a core library, and three different 
implementations - Discord.Net.Core, Discord.Net.Rest, Discord.Net.Rpc,
and Discord.Net.WebSockets.

As a bot developer, you will only need to use Discord.Net.WebSockets, 
but you should be aware of the differences between them.

`Discord.Net.Core` provides a set of interfaces that model Discord's 
API. These interfaces are consistent throughout all implementations of 
Discord.Net, and if you are writing an implementation-agnostic library 
or addon, you can rely on the core interfaces to ensure that your 
addon will run on all platforms.

`Discord.Net.Rest` provides a set of concrete classes to be used 
**strictly** with the REST portion of Discord's API. Entities in 
this implementation are prefixed with `Rest`, e.g. `RestChannel`.

`Discord.Net.Rpc` provides a set of concrete classes that are used with 
Discord's RPC API. Entities in this implementation are prefixed with 
`Rpc`, e.g. `RpcChannel`.

`Discord.Net.WebSocket` provides a set of concrete classes that are used 
primarily with Discord's WebSocket API, or entities that are kept in 
cache. When developing bots, you will be using this implementation. All 
entities are prefixed with `Socket`, e.g. `SocketChannel`.