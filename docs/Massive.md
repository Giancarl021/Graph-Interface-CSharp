# Massive

This method allows you to make a lot of similar requests, which does not have a properly paginated endpoint. For example:

* You need to get all the [`licenseDetails`](https://docs.microsoft.com/pt-br/graph/api/user-list-licensedetails) from every user in the tenant.
* As you need to make one request for each user, you cannot simply use the [`List`](List.md) method (considering that, when this documentation was written, you could not use `$select` or `$expand` to get the `licenseDetails` from a normal paginated request).
* The best approach would be use the `$batch` endpoint on Graph API, but it would need a lot of work to package the needed requests in packages of 20.
* The [`Massive`](Massive.md) method handle all the above problems, requiring only a string pattern of the resource and the values to interpolate.

## Signature

```csharp
public async Task<Dictionary<string, T>> Massive<T>(string format, GraphInterfaceMassiveOptions options) where T : class
```

## Parameters

### `string format`

The format of the resource to be interpolated, like `users/{0}/licenseDetails`.

This parameter is required, and will concatenate with the full endpoint in the Graph API, allowing you to write query parameters like `$select` and `$filter`.

The format will be interpolated using the native [`string.Format`](https://docs.microsoft.com/pt-br/dotnet/api/system.string.format) method.

### `GraphInterfaceMassiveOptions options`

Dictates the behavior of the massive request. Such as Headers, Body, Method, if the response should be cached and the general behavior of the batch request cycle, such as attempts.

```csharp

```