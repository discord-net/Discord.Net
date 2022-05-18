---
uid: Guides.IntFw.AutoCompletion
title: Command Autocompletion
---

# AutocompleteHandlers

[Autocompleters] provide a similar pattern to TypeConverters.
[Autocompleters] are cached, singleton services and they are used by the
Interaction Service to handle Autocomplete Interations targeted to a specific Slash Command parameter.

To start using AutocompleteHandlers, use the `[AutocompleteAttribute(Type type)]` overload of the [AutocompleteAttribute].
This will dynamically link the parameter to the [AutocompleteHandler] type.

AutocompleteHandlers raise the `AutocompleteHandlerExecuted` event on execution. This event can be also used to create a post-execution logic, just like the `*CommandExecuted` events.

## Creating AutocompleteHandlers

A valid AutocompleteHandlers must inherit [AutocompleteHandler] base type and implement all of its abstract methods.

[!code-csharp[Autocomplete Command Example](samples/autocompletion/autocomplete-example.cs)]

### GenerateSuggestionsAsync()

The Interactions Service uses this method to generate a response of an Autocomplete Interaction.
This method should return `AutocompletionResult.FromSuccess(IEnumerable<AutocompleteResult>)` to
display parameter suggestions to the user. If there are no suggestions to be presented to the user, you have two results:

1. Returning the parameterless `AutocompletionResult.FromSuccess()` will display a "No options match your search." message to the user.
2. Returning `AutocompleteResult.FromError()` will make the Interaction Service **not** respond to the interaction,
consequently displaying the user a "Loading options failed." message. `AutocompletionResult.FromError()` is solely used for error handling purposes. Discord currently doesn't allow
you to display custom error messages. This result type will be directly returned to the `AutocompleteHandlerExecuted` method.

## Resolving AutocompleteHandler Dependencies

AutocompleteHandler dependencies are resolved using the same dependency injection
pattern as the Interaction Modules.
Property injection and constructor injection are both valid ways to get service dependencies.

Because [AutocompleterHandlers] are constructed at service startup,
class dependencies are resolved only once.

> [!NOTE]
> If you need to access per-request dependencies you can use the
> IServiceProvider parameter of the `GenerateSuggestionsAsync()` method.

[AutoCompleteHandlers]: xref:Discord.Interactions.AutocompleteHandler
[AutoCompleteHandler]: xref:Discord.Interactions.AutocompleteHandler
[AutoCompleteAttribute]: 
