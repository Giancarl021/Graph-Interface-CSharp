# Unit

This method allows you to make requests that returns only a single entity, like in [`users/<id | userPrincipalName>`](https://docs.microsoft.com/pt-br/graph/api/user-get).

## Signatures

```csharp
public async Task<T> Unit<T>(string resource) where T : class;
public async Task<T> Unit<T>(string resource, GraphInterfaceUnitOptions options) where T : class;
```

## Parameters

### `string resource`

The resource on the Graph API to retrieve, like `users/<id | userPrincipalName>`.

This parameter is required, and will concatenate with the full endpoint in the Graph API, allowing you to write query parameters like `$select` and `$filter`.

### (Optional) `GraphInterfaceUnitOptions options`

Dictates the behavior of the unit request. Such as Headers, Body, Method and if the response should be cached.

This class extends the [`GraphInterfaceRequestOptions`](RequestOptions.md) class.

```csharp
public class GraphInterfaceUnitOptions : GraphInterfaceRequestOptions
{
    public bool UseCache { get; set; } = false;

    // (Internal methods hidden) ...
}
```

* **UseCache** - If `true`, the response will be cached using the `IGraphInterfaceCacheService` service initialized with the client. Default `false`;

## Returns

A Task, that when resolved will return an instance of `T` with the response from the Graph API. Note that under the hood, the response is parsed using the `System.Text.Json.JsonSerializer` `JsonConvert.DeserializeObject<T>` method, so, to get a better formed class, you can use the `[JsonPropertyName("propertyName")]` attribute on the properties you want to use.