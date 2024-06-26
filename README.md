# Graph-Interface-CSharp

![](assets/icon.png)

Simple Microsoft [Graph API](https://docs.microsoft.com/pt-br/graph/api/overview) client.

## Similar projects

* [Microsoft.Graph](https://www.nuget.org/packages/Microsoft.Graph/) - The official C# client;
* [graph-interface](https://www.npmjs.com/package/graph-interface) - The NodeJS version of this package.

## Why?

As you may noticed, there is already an [official package](https://www.nuget.org/packages/Microsoft.Graph/) to deal with the Graph API requests, and it's very well done. It have all the abstractions and provides a nice API to work with.

However, some things are not available in the official package, like the ability to make [massive requests](docs/Massive.md) and a simplified way to authenticate with the API.

In general, I would say that you probably should use the official package, but if you really need to make massive requests, you can use this package.

## Installation

You can get this package on the [Nuget](https://www.nuget.org/packages/GraphInterface/).

## Usage

### Importing

First you need to import the package using the `using` keyword:

```csharp
using GraphInterface;
```

Then you will have access to the `GraphInterfaceClient` class, which is the main class of this package.

### Initialization

This class have two constructors:

```csharp
public GraphInterfaceClient(GraphInterfaceCredentials credentials);
public GraphInterfaceClient(GraphInterfaceCredentials credentials, GraphInterfaceOptions options);
```

Both of them needs the `GraphInterfaceCredentials` parameter, which is used to properly authenticate with the Graph API.

Here is how this class looks like:

```csharp
public class GraphInterfaceCredentials
{
    public string TenantId { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }

    public GraphInterfaceCredentials(string tenantId, string clientId, string clientSecret);
}
```

* **TenantId** - The ID of the Tenant where the application is registered.
* **ClientId** - The ID of the application itself registered in the Tenant.
* **ClientSecret** - The secret of the application, generated by the developer.

In addition, this class also have a `GraphInterfaceOptions` parameter, which is used to configure the certain behaviors of the client, like the cache service.

The class looks like this:

```csharp
public class GraphInterfaceOptions
{
    public string Version { get; set; } = "v1.0";
    public Func<GraphInterfaceCredentials, Task<GraphInterfaceAccessTokenResponse>>? AuthenticationProvider { get; set; } = null;
    public ILogger Logger { get; set; } = NullLogger.Instance;
    public HttpClient HttpClient { get; set; } = new HttpClient();
    public IGraphInterfaceCacheService CacheService { get; set; } = new GraphInterfaceMemoryCacheService();
    public bool CacheAccessTokenByDefault { get; set; } = true;
}
```

* **Version** - The version of the Graph API to use. Default `v1.0`.
* **AuthenticationProvider** - A function that will be called to get the access token. If not provided, the client will use the default `GetAccessToken` behavior.
* **Logger** - An `ILogger` instance to log the internal processing of requests in the library. All logs have a level of `LogLevel.Debug`. Default `NullLogger.Instance`.
* **HttpClient** - An `HttpClient` instance to use to make the requests. Default `new HttpClient()`.
* **CacheService** - An `IGraphInterfaceCacheService` instance to use to cache access tokens and responses if needed. Default `new GraphInterfaceMemoryCacheService()`.
* **CacheAccessTokenByDefault** - If `true`, the client will cache the access token by default. If CacheService is set to `null`, this option will be ignored. Default `true`.

## Methods

* [GetAccessToken](docs/GetAccessToken.md) - Get the access token to use the Graph API, used most internally, but if you need for a custom request, you can get it from here;
* [Raw](docs/Raw.md) - Makes requests that returns non-JSON responses, returning a `StreamContent` instead;
* [Unit](docs/Unit.md) - Makes requests that returns only a single entity;
* [List](docs/List.md) - Makes requests that returns a list of entities, paginated using the `@odata.nextLink` property;
* [Massive](docs/Massive.md) - Makes batch requests based on a template URL and a list of values to interpolate, generating a lot of similar requests.
* [GetListGenerator](docs/GetListGenerator.md) - Creates a `AsyncEnumerable` that will make requests to the Graph API, paginating the results using the `@odata.nextLink` property, allowing the caller to control how the pages are threated.