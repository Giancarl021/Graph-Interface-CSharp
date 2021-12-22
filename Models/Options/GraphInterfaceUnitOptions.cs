using System.Collections.Generic;
using GraphInterface.Models.Abstract;

namespace GraphInterface.Models.Options
{
    public class GraphInterfaceUnitOptions : GraphInterfaceRequestOptions
    {
        public IEnumerable<GraphInterfaceUnitField> Fields { get; set; } = null;
    }
}