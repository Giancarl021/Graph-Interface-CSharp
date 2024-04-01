using System.Collections.Generic;

namespace GraphInterface.Models.Helpers;
internal class GraphInterfaceBatchResult
{
    public List<GraphInterfaceBatchResponseItem> Resolved { get; set; } = [];
    public List<string> Rejected { get; set; } = [];
}