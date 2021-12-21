using GraphInterface.Models;
using Microsoft.Extensions.Logging;

namespace GraphInterface
{
    public class GraphInterface
    {
        public GraphInterface()
        {
            new GraphInterfaceOptions {
                // Logger = new LoggerFactory().CreateLogger("GraphInterface")
            };
        }
    }
}