---
uid: Guides.InteractionsFramework.Autocompleters
title: Autocompleters
---

# Autocompleters

Autocompleters provide a similar pattern to TypeConverters. Autocompleters are cached, singleton services and they are used by the Interaction Service to handle Autocomplete Interations targeted to a specific Slash Command parameter.

To start using Autocompleters, use the `[AutocompleteAttribute(Type autocompleterType)]` overload of the `[AutocompleteAttribute]`. This will dynamically link the parameter to the Autocompleter type.

## Creating Autocompleters

A valid Autocompleter must inherit `Autocompleter` base type and implement all of its abstract methods.

### GenerateSuggestionsAsync()

Interactions Service uses this method to generate a response to a Autocomplete Interaction. This method should return `AutocompletionResult.FromSuccess(IEnumerable<AutocompleteResult>)` to display parameter sugesstions to the user. If there are no suggestions to be presented to the user, you have two options:

1. Returning the parameterless `AutocompletionResult.FromSuccess()` will display "No options match your search." message to the user.
2. Returning `AutocompleteResult.FromError()` will make the Interaction Service not respond to the interation, consequently displaying the user "Loading options failed." message.

## Resolving Autocompleter Dependencies

Autocompleter dependencies are resolved using the same dependency injection pattern as the Interaction Modules. Property injection and constructor injection are both valid ways to get service dependencies.

Because Autocompleters are constructed at service startup, class dependencies are resolved only once. If you need to access per-request dependencies you can use the IServiceProvider parameter of the `GenerateSuggestionsAsync()` method.
