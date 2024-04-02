# RequestOptions

This abstract class is used as a base to all options classes used in the [Unit](Unit.md), [List](List.md) and [Massive](Massive.md) methods. It provides all the basic properties to configure an Graph API request.

## Properties

The shape of this class is:

```csharp
public abstract class GraphInterfaceRequestOptions
{
    public HttpMethod Method { get; set; } = HttpMethod.Get;
    public Dictionary<string, string> Headers { get; set; } = [];
    public object? Body { get; set; } = null;
    public string? CustomAccessToken = null;
}
```

* **Method** - The HTTP method to use in the request. Default `HttpMethod.Get`;
* **Headers** - A dictionary of custom headers to be added to the request. Default `new Dictionary<string, string>()`. *Important: The Access Token will be automatically put in the headers, so it is not necessary to put it manually*;
* **Body** - The body of the request. Default `null`.
* **CustomAccessToken** - A custom access token to use in the request. If set, the client will not get a new access token from the cache or the authentication provider. Default `null`.