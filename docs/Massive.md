# Massive

This method allows you to make a lot of similar requests, which does not have a properly paginated endpoint. For example:

* You need to get all the [`licenseDetails`](https://docs.microsoft.com/pt-br/graph/api/user-list-licensedetails) from every user in the tenant;
* As you need to make one request for each user, you cannot simply use the [`List`](List.md) method (considering that, when this documentation was written, you could not use `$select` or `$expand` to get the `licenseDetails` from a normal paginated request);
* The best approach would be use the `$batch` endpoint on Graph API, but it would need a lot of work to package the needed requests in packages of 20;
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

This class extends the [`GraphInterfaceRequestOptions`](RequestOptions.md) class.

```csharp
public class GraphInterfaceMassiveOptions : GraphInterfaceRequestOptions
{
    public new Dictionary<string, string> Headers { get; set; } = null;
    public Dictionary<string, string> BatchRequestHeaders { get; set; } = new Dictionary<string, string>();
    public IEnumerable<IEnumerable<string>> Values { get; set; } = null;
    public uint BinderIndex = 0;
    public uint Attempts { get; set; } = 3;
    public uint RequestsPerAttempt { get; set; } = 50;
    public bool NullifyErrors = false;
    public bool UseCache { get; set; } = false;

    // (Internal methods hidden) ...
}
```

* **UseCache** - If `true`, the response will be cached using the `IGraphInterfaceCacheService` service initialized with the client. Default `false`;

## Data interpolation

On the property `Values` in the `options` parameter, you can pass a list of lists of strings, where
each list represents an index on the `format` parameter. All the lists should have the same length.

For example:

```csharp
var client = new GraphInterfaceClient(/* Credentials and options */);

var response = await client.Massive<LicenseDetails>("users/{1}/licenseDetails?$select={0}", new GraphInterfaceMassiveOptions
{
    Values = new string[][]
    {
        new string[] { "id", "skuId", "skuPartNumber" },
        new string[] { "id-1", "id-2", "id-3" }
    }
});
```

In the example above, the requests would be interpolated as:

* `users/id-1/licenseDetails?$select=id`;
* `users/id-2/licenseDetails?$select=skuId`;
* `users/id-3/licenseDetails?$select=skuPartNumber`.

Normally you will probably just one interpolated value, so, to make things easier, the `GraphInterfaceMassiveOptions` class also has two constructors, as listed below:

```csharp
public GraphInterfaceMassiveOptions() : base() {}
public GraphInterfaceMassiveOptions(IEnumerable<string> values) : base();
```

The first one is to allow this class to be used like the others, and the second one is to allow you to pass a single list of strings to be interpolated, avoiding creating a list of lists if you want to interpolate only one value.

## Request Cycle

While stability is an important factor, this method is not designed to not encounter errors, as the other methods of this package. Actually, it relies that errors will be thrown, and deal with them as a normal part of the cycle.

When you need to make a lot of requests, even when you shrink the number of requests, using the `$batch` endpoint, some of the requests will fail, normally caused by the `427 - Too Many Requests` status. To deal with this problem, the request cycle works like that:

* All the requests are packaged into batches of 20;
* All the batches are sent to the `$batch` endpoint, in blocks of `RequestsPerAttempt` requests, using throttling to limit the amount of requests sent by the same time;
* The responses are parsed and unpacked back into single results;
* If a response is not successful, it will be pushed back into the cycle, while the successful ones will be added to the result;
* The cycle repeats until it has no more requests to be sent or the maximum number of attempts is reached.

To avoid an infinite loop, the `Attempts` property in the `options` parameter can be used to set the maximum number of attempts. When the limit is reached, the cycle will stop and, depending on the `NullifyErrors` property, the errors will be ignored and set as `null` or the cycle will throw an exception.

> **Important note:** The number of attempts only increments when all the request sent are unsuccessful.

## Headers

This method in specific have a different behavior about the `Headers` of the `options` parameter. You may noticed that exist two properties regarding the headers, `BatchRequestHeaders` and `Headers`.

The `Headers` property will be used to set the headers of each **single** request, while the `BatchRequestHeaders` property will be used to set the headers of the **`$batch`** requests.

It is important to notice the difference, because each one of them are used to different purposes.

## Returns

A Task, that when resolved will return a `Dictionary` with `string` keys, which are the based on the `Values` on the index `BinderIndex`, and `T` values with the aggregation of all responses from Graph API. The shape of the responses will be exactly the same as the shape of the body got from the result of the batch requests. Note that under the hood, the response is parsed using the `System.Text.Json.JsonSerializer` `JsonConvert.DeserializeObject<T>` method, so, to get a better formed class, you can use the `[JsonPropertyName("propertyName")]` attribute on the properties you want to use.

It is important to note that, as the results will not be further parsed, paginated requests inside the batch will not be completed.
