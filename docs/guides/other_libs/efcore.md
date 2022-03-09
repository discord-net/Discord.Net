---
uid: Guides.OtherLibs.EFCore
title: EFCore
---

# Entity Framework Core

In this guide we will set up EFCore with a PostgreSQL database. Information on other databases will be at the bottom of this page.

## Prerequisites

- A simple bot with dependency injection configured
- A running PostgreSQL instance
- [EFCore CLI tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet#installing-the-tools)

## Downloading the required packages

You can install the following packages through your IDE or go to the nuget link to grab the dotnet cli command.

|Name|Link|
|--|--|
| `Microsoft.EntityFrameworkCore` | [link](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore) |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | [link](https://www.nuget.org/packages/Npgsql.EntityFrameworkCore.PostgreSQL)|

## Configuring the DbContext

To use EFCore, you need a DbContext to access everything in your database. The DbContext will look like this. Here is an example entity to show you how you can add more entities yourself later on.

[!code-csharp[DBContext Sample](samples/DbContextSample.cs)]

> [!NOTE]
> To learn more about creating the EFCore model, visit the following [link](https://docs.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli#create-the-model)

## Adding the DbContext to your Dependency Injection container

To add your newly created DbContext to your Dependency Injection container, simply use the extension method provided by EFCore to add the context to your container. It should look something like this

[!code-csharp[DBContext Dependency Injection](samples/DbContextDepInjection.cs)]

> [!NOTE]
> You can find out how to get your connection string [here](https://www.connectionstrings.com/npgsql/standard/)

## Migrations

Before you can start using your DbContext, you have to migrate the changes you've made in your code to your actual database.
To learn more about migrations, visit the official Microsoft documentation [here](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli)

## Using the DbContext

You can now use the DbContext wherever you can inject it. Here's an example on injecting it into an interaction command module.

[!code-csharp[DBContext injected into interaction module](samples/InteractionModuleDISample.cs)]

## Using a different database provider

Here's a couple of popular database providers for EFCore and links to tutorials on how to set them up. The only thing that usually changes is the provider inside of your `DbContextOptions`

| Provider | Link |
|--|--|
| MySQL | [link](https://dev.mysql.com/doc/connector-net/en/connector-net-entityframework-core-example.html) |
| SQLite | [link](https://docs.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli) |
