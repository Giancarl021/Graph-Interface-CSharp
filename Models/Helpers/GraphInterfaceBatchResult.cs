using System.Collections.Generic;

namespace GraphInterface.Models.Helpers
{
    internal class GraphInterfaceBatchResult
    {
        public List<GraphInterfaceBatchResponseItem> Resolved { get; set; } = new List<GraphInterfaceBatchResponseItem>();
        public List<string> Rejected { get; set; } = new List<string>();
    }
}