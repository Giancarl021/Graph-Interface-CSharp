# GetListGenerator

This method is similar to the [List](List.md) method, but instead of returning a `IEnumerable` of `T`, it returns a `IAsyncEnumerable` of `IEnumerable` of `T`, allowing the caller to control how the pages are threated.

It allows you to paginate between a list of entities, like in [`users`](https://docs.microsoft.com/pt-br/graph/api/user-list).

## Signatures

```csharp
public IAsyncEnumerable<IEnumerable<T>> CreateListGenerator<T>(string resource) where T : class;
public async IAsyncEnumerable<IEnumerable<T>> CreateListGenerator<T>(string resource, GraphInterfaceListGeneratorOptions options) where T : class;
```

## Parameters

### `string resource`

The resource on the Graph API to retrieve, like `users`.

This parameter is required, and will concatenate with the full endpoint in the Graph API, allowing you to write query parameters like `$select` and `$filter`.

### (Optional) `GraphInterfaceListGeneratorOptions options`

Dictates the behavior of the list request. Such as Headers, Body, Method, if the response should be cached and the limit and offset of the pagination.

This class extends the [`GraphInterfaceRequestOptions`](RequestOptions.md) class.

```csharp
public class GraphInterfaceListGeneratorOptions : GraphInterfaceRequestOptions
{
    public int? Limit { get; set; } = null;
    public int? Offset { get; set; } = null;
    // (Internal methods hidden) ...
}
```

> **Important:** You may have noticed that the `UseCache` property is missing from this class. This is because of the nature of the method, the flow of the data is controlled by the caller, so managing the return data is in the responsibility of the caller by principal.

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

A `AsyncEnumerable` of an `IEnumerable` of `T` with each iteration coming from the Graph API. The items will be returned by page, and when a new page is needed, the caller can decide to continue or not, making the process transparent.

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