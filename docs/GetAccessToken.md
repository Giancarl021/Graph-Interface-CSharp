# GetAccessToken

> This method is already used internally, but if you need the access token for a custom request, you can get it from here.

This method allow you to get the access token to use in the Graph API requests on the `Authorization` header.

## Signatures

```csharp
public async Task<string> GetAccessToken();
public async Task<string> GetAccessToken(GraphInterfaceAccessTokenOptions options);
```

## Parameters

### (Optional) `GraphInterfaceAccessTokenOptions options`

Dictates the behavior of the method, allowing you to decide if the response should be cached or not.

```csharp
public class GraphInterfaceAccessTokenOptions
{
    public bool UseCache { get; set; } = true;
}
```

* **UseCache** - If `true`, the access token will be cached using the `IGraphInterfaceCacheService` service initialized with the client. Default `true`.

## Returns

A string with the access token as value. In order to use it, you need to add it to the `Authorization` header of your request, prefixed with `Bearer `.
If the `AuthenticationProvider` is set in the `GraphInterfaceOptions` in the instance, the set method will call it to get the access token.