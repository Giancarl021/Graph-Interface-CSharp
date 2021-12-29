# List

This method allows you to paginate between a list of entities, like in [`users`](https://docs.microsoft.com/pt-br/graph/api/user-list).

## Signatures

```csharp
public async Task<IEnumerable<T>> List<T>(string resource) where T : class;
public async Task<IEnumerable<T>> List<T>(string resource, GraphInterfaceListOptions options) where T : class;
```

## Parameters

### `string resource`

The resource on the Graph API to retrieve, like `users`.

This parameter is required, and will concatenate with the full endpoint in the Graph API, allowing you to write query parameters like `$select` and `$filter`.

### (Optional) `GraphInterfaceListOptions options`

Dictates the behavior of the list request. Such as Headers, Body, Method, if the response should be cached and the limit and offset of the pagination.

This class extends the [`GraphInterfaceRequestOptions`](RequestOptions.md) class.

```csharp
public class GraphInterfaceListOptions : GraphInterfaceRequestOptions
{
    public int? Limit { get; set; } = null;
    public int? Offset { get; set; } = null;

    // (Internal methods removed) ...
}
```

## How `Limit` and `Offset` works

Both of this properties works with the quantity of **requests** made, **NOT** the quantity of entities returned.

You can control the quantity of total entities by using the `$top` query parameter or combining it with this properties if the specified quantity is too big.

### Limit

This property limits the quantity of requests made to the Graph API. So, if you have a list of 1000 users, but you only want the first 100, you can set the `Limit` property to 1, as by default the Graph API returns 100 entities per page.

### Offset

This property skips the first `n` requests, allowing you to skip the first entities on the list. So, if you have a list of 1000 users, but you only want the users starting from the 100th position, you can set the `Offset` property to 1, as by default the Graph API returns 100 entities per page.

### Using both

When you set both `Limit` and `Offset`, the number of requests will now be `Limit` + `Offset`, starting to count the pages starting from the `Offset` position, until it reaches the `Limit`.
This is important because now the `Limit` does **NOT** represents the total of requests.
## Returns

A Task, that when resolved will return an `IEnumerable` of `T` with the response from the Graph API. Note that under the hood, the response is parsed using the [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/) `JsonConvert.DeserializeObject<T>` method, so, to get a better formed class, you can use the `[JsonProperty("propertyName")]` attribute on the properties you want to use.

In this case, the original response bodies will be shaped like this:

```jsonc
{
    "@odata.context": "https://graph.microsoft.com/v1.0/$metadata#<context>",
    "@data.nextLink": "<Graph API URL to next page>",
    "value": [
        // Multiple entities with the resource shape
    ]
}
```

This method will remove the `@odata.context` and `@data.nextLink` properties from the response, and will aggregate all the `value` properties from the multiple responses. This allows you to use the same class for an [`Unit`](Unit.md) and `List` request, avoiding the need to wrap the model class in a aggregation class.