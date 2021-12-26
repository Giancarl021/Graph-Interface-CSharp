using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphInterface.Models.Helpers;
using Microsoft.Extensions.Logging;

namespace GraphInterface.Options
{
    internal class GraphInterfaceMassiveCyclerOptions
    {
        public IEnumerable<Func<Task<GraphInterfaceBatchResponse>>> Packages;
        public ILogger Logger { get; set; }
        public Func<Task<string>> GetAccessToken { get; set; }
        public bool Parallel { get; set; }
        public int Ticks { get; set; }
        public int MaximumAttempts { get; set; }

        public void Assert()
        {
            if (Packages == null)
            {
                throw new Exception("Packages cannot be null");
            }

            if (Logger == null)
            {
                throw new Exception("Logger cannot be null");
            }

            if (GetAccessToken == null)
            {
                throw new Exception("GetAccessToken cannot be null");
            }

            if (Ticks <= 0)
            {
                throw new ArgumentException("Ticks must be greater than 0");
            }

            if (MaximumAttempts <= 0)
            {
                throw new Exception("MaximumAttempts must be greater than 0");
            }
        }
    }
}