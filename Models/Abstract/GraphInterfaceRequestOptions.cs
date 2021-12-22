using System.Collections.Generic;
using System.Net.Http;

namespace GraphInterface.Models.Abstract
{
    public abstract class GraphInterfaceRequestOptions
    {
        public bool UseCache { get; set; } = false;
        public HttpMethod Method { get; set; } = HttpMethod.Get;
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public object Body { get; set; } = null;
    }
}