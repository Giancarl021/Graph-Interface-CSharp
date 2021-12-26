using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphInterface.Options;
using Throttle;

namespace GraphInterface.Services
{
    internal class GraphInterfaceMassiveCycler<T>
    {
        private readonly GraphInterfaceMassiveCyclerOptions _options;

        public GraphInterfaceMassiveCycler(GraphInterfaceMassiveCyclerOptions options)
        {
            _options = options;
            _options.Assert();
        }

        public async Task<Dictionary<string, T>> Cycle()
        {
            var responses = await Throttler.Throttle(_options.Packages, _options.MaximumAttempts);

            throw new NotImplementedException();
        }
    }
}