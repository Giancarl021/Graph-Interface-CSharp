using GraphInterface.Models.Abstract;

namespace GraphInterface.Options
{
    public class GraphInterfaceListOptions : GraphInterfaceRequestOptions
    {
        public int? Limit { get; set; } = null;
        public int? Offset { get; set; } = null;
        public bool UseCache { get; set; } = false;

        public GraphInterfaceUnitOptions ToUnitOptions()
        {
            return new GraphInterfaceUnitOptions
            {
                Body = Body,
                Headers = Headers,
                Method = Method,
                UseCache = false,
                CustomAccessToken = CustomAccessToken
            };
        }
    }
}