# RequestOptions

This abstract class is used as a base to all options classes used in the [Unit](Unit.md), [List](List.md) and [Massive](Massive.md) methods. It provides all the basic properties to configure an Graph API request.

## Properties

The shape of this class is:

```csharp
public abstract class GraphInterfaceRequestOptions
{
    public bool UseCache { get; set; } = false;
    public HttpMethod Method { get; set; } = HttpMethod.Get;
    public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
    public object Body { get; set; } = null;
}
```

* **UseCache** - If `true`, the access token will be cached using the `IGraphInterfaceCacheService` service initialized with the client. Default `false`;
* **Method** - The HTTP method to use in the request. Default `HttpMethod.Get`;
* **Headers** - A dictionary of custom headers to be added to the request. Default `new Dictionary<string, string>()`. *Important: The Access Token will be automatically put in the headers, so it is not necessary to put it manually*;
* **Body** - The body of the request. Default `null`.