using System;

namespace GraphInterface.Models.Helpers
{
    internal class GraphInterfaceMemoryCacheItem
    {
        public DateTime Expiration { get; set; }
        public object Value { get; set; }
    }
}