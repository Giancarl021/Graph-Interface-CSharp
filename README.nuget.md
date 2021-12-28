# Graph-Interface-CSharp

![](assets/icon.png)

Simple Microsoft [Graph API](https://docs.microsoft.com/pt-br/graph/api/overview) client.

## Similar projects

* [Microsoft.Graph](https://www.nuget.org/packages/Microsoft.Graph/) - The official C# client;
* [graph-interface](https://www.npmjs.com/package/graph-interface) - The NodeJS version of this package.

## Why?

As you may noticed, there is already an [official package](https://www.nuget.org/packages/Microsoft.Graph/) to deal with the Graph API requests, and it's very well done. It have all the abstractions and provides a nice API to work with.

However, some things are not available in the official package, like the ability to make [massive requests](docs/Massive.md) and a simplified way to authenticate with the API.

In general, I would say that you probably should use the official package, but if you really need to make massive requests, you can use this package.

## Documentation and Source-Code

You can find the full documentation and the source-code of this library [here](https://github.com/Giancarl021/Graph-Interface-CSharp).