# Raw

This method allows you to make requests that returns non-JSON responses, like in [`reports/getTeamsUserActivityUserDetail`](https://learn.microsoft.com/en-us/graph/api/reportroot-getteamsuseractivityuserdetail).

## Signatures

```csharp
public async Task<StreamContent> Raw(string resource);
public async Task<StreamContent> Raw(string resource, GraphInterfaceRawOptions options);
```

## Parameters

### `string resource`

The resource on the Graph API to retrieve, like `reports/getTeamsUserActivityUserDetail(period='{period}')`.

This parameter is required, and will concatenate with the full endpoint in the Graph API, allowing you to write query parameters like `$select` and `$filter`.

### (Optional) `GraphInterfaceRawOptions options`

Dictates the behavior of the unit request. Such as Headers, Body, Method and if the response should be cached.

This class extends the [`GraphInterfaceRequestOptions`](RequestOptions.md) class.

```csharp
public class GraphInterfaceRawOptions : GraphInterfaceRequestOptions {}
```

## Returns

A Task, that when resolved will return an `StreamContent` with the response from the Graph API. This will give you the raw response from the Graph API, allowing you to parse it as you wish.