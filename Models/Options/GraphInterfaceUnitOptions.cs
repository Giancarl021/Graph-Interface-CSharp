using GraphInterface.Models.Abstract;

namespace GraphInterface.Options;
public class GraphInterfaceUnitOptions : GraphInterfaceRequestOptions
{
    public bool UseCache { get; set; } = false;
}