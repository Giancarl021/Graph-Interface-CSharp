# Unit

This method allows you to make requests that returns only a single entity, like an [User](https://docs.microsoft.com/pt-br/graph/api/resources/user).

## Signatures

```csharp
public async Task<T> Unit<T>(string resource) where T : class;
public async Task<T> Unit<T>(string resource, GraphInterfaceUnitOptions options) where T : class;
```

## Parameters

### `string resource`

The resource on the Graph API to retrieve, like `users/<id>`.

This parameter is required, and will concatenate with the full endpoint in the Graph API, allowing you to write query parameters like `$select` and `$filter`.

> Work In Progress