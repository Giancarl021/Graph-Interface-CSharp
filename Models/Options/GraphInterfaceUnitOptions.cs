using GraphInterface.Models.Abstract;

namespace GraphInterface.Options;
public class GraphInterfaceUnitOptions : GraphInterfaceRequestOptions
{
    public bool UseCache { get; set; } = false;
    public GraphInterfaceRawOptions ToRawOptions()
    {
        return new GraphInterfaceRawOptions
        {
            Body = Body,
            CustomAccessToken = CustomAccessToken,
            Headers = Headers,
            Method = Method
        };
    }
}