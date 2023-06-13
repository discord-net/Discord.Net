# Ratelimits

Ratelimits are a core concept of any API - Discords API is no exception. Each verified library must follow the ratelimit guidelines.

### Using the ratelimit callback

There is a new property within `RequestOptions` called RatelimitCallback. This callback is called when a request is made via the rest api. The callback is called with a `IRateLimitInfo` parameter:

| Name       | Type            | Description                                                                                                                                        |
| ---------- | --------------- | -------------------------------------------------------------------------------------------------------------------------------------------------- |
| IsGlobal   | bool            | Whether or not this ratelimit info is global.                                                                                                      |
| Limit      | int?            | The number of requests that can be made.                                                                                                           |
| Remaining  | int?            | The number of remaining requests that can be made.                                                                                                 |
| RetryAfter | int?            | The total time (in seconds) of when the current rate limit bucket will reset. Can have decimals to match previous millisecond ratelimit precision. |
| Reset      | DateTimeOffset? | The time at which the rate limit resets.                                                                                                           |
| ResetAfter | TimeSpan?       | The absolute time when this ratelimit resets.                                                                                                      |
| Bucket     | string          | A unique string denoting the rate limit being encountered (non-inclusive of major parameters in the route path).                                   |
| Lag        | TimeSpan?       | The amount of lag for the request. This is used to denote the precise time of when the ratelimit expires.                                          |
| Endpoint   | string          | The endpoint that this ratelimit info came from.                                                                                                   |

Let's set up a ratelimit callback that will print out the ratelimit info to the console.

```cs
public async Task MyRatelimitCallback(IRateLimitInfo info)
{
    Console.WriteLine($"{info.IsGlobal} {info.Limit} {info.Remaining} {info.RetryAfter} {info.Reset} {info.ResetAfter} {info.Bucket} {info.Lag} {info.Endpoint}");
}
```

Let's use this callback in a send message function

```cs
[Command("ping")]
public async Task ping()
{
    var options = new RequestOptions()
    {
        RatelimitCallback = MyRatelimitCallback
    };

    await Context.Channel.SendMessageAsync("Pong!", options: options);
}
```

Running this produces the following output:

```
False 5 4  2021-09-09 3:48:14 AM +00:00 00:00:05 a06de0de4a08126315431cc0c55ee3dc 00:00:00.9891364 channels/848511736872828929/messages
```
