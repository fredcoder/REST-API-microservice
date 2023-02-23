# InfoTrack .NET Core MidLevel Tech Test

- [Description](#description)
- [Your Mission](#your-mission)
- [Getting Started](#getting-started)
- [What We are Looking For](#what-we-are-looking-for)
- [Going Above and Beyond](#going-above-and-beyond)

## Description

This web application is a REST API microservice responsible for CRUD (Create, Read, Update, Delete) operations regarding the `User` entity, including data that is directly related (one-to-one) such as a user's `ContactDetails`.

## Your Mission

You have received the API in an incomplete state. There are several `TODO` comments as well as `NotImplementedExceptions` located throughout the solution. You must finish implementing this API so that the acceptance tests in `WebApplication.IntegrationTests` pass when they are executed.

The files that you must complete are:

- WebApplication.csproj:
  - UsersController.cs
- WebApplication.Core.csproj:
  - UpdateUserCommand.cs
  - ListUsersQuery.cs
  - RequestValidationBehaviour.cs
- WebApplication.Infrastructure.csproj:
  - UserService.cs

## Getting Started

It is highly recommended for you to **download this test** over using the online IDE provided by HackerRank. Please follow the instructions provided by HackerRank to download and complete the test.

## What We are Looking For

We are generally lookng to see that:

- WebApplication:
  - You know how a REST API works, and know when to use the different request types.
- WebApplication.Core:
  - You know / can learn how to write request handlers using the [MediatR](https://github.com/jbogard/MediatR) library
  - You know / can learn how to write [MediatR](https://github.com/jbogard/MediatR) pipeline behaviours that perform actions before executing a request handler
  - You know / can learn how to write validation logic using [FluentValidation](https://fluentvalidation.net/)
  - You know / can learn how to map entities to DTOs using [AutoMapper](https://docs.automapper.org/en/stable/Getting-started.html)
- WebApplication.Infrastructure:
  - You know / can learn how to perform CRUD operations using [EF Core](https://docs.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli) and [LINQ](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/)
- WebApplication.IntegrationTests
  - You understand how to read tests and can code to satisfy the testing scenarios

On the more subjective side, we will also be assessing you on your coding style, such as:

- How easy is it to understand, e.g. [Cognitive Complexity](https://docs.codeclimate.com/docs/cognitive-complexity)
- Are there any [Code Smells](https://refactoring.guru/refactoring/smells)
- Have you written unnecessary code / could your code have been simplified

## Going Above and Beyond

The product owner of this API has gotten in touch with you and asked that `logging` be added throughout the API so that there is increased visiblity on what the API is doing. Using any logging library of your choice, or no library at all, implement logging for the API that does the following:

- Logs the duration it takes for any MediatR command / query handler to execute
- Logs the exceptions that are thrown by the API
