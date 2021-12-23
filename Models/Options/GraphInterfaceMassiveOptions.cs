using System;
using System.Collections.Generic;
using System.Linq;
using GraphInterface.Models.Abstract;

namespace GraphInterface.Options
{
    public class GraphInterfaceMassiveOptions : GraphInterfaceRequestOptions
    {
        public Dictionary<string, string> BatchRequestHeaders { get; set; } = new Dictionary<string, string>();
        public IEnumerable<IEnumerable<string>> Values { get; set; } = null;
        public uint BinderIndex = 0;
        public uint Attempts { get; set; } = 3;
        public bool Parallel { get; set; } = true;
        public uint RequestsPerAttempt { get; set; } = 50;
        public GraphInterfaceParseMethod ParseMethod { get; set; } = GraphInterfaceParseMethod.Unit;
        public GraphInterfaceMassiveOptions() : base() {}
        public GraphInterfaceMassiveOptions(IEnumerable<string> values) : base() {
            Values = new List<IEnumerable<string>>() { values };
        }
        public void Assert() {
            if (Values == null)
            {
                throw new Exception("Values cannot be null");
            }

            int? size = null;

            foreach (var item in Values)
            {
                int count = item.ToList().Count();
                if (size == null) size = count;

                if (size != count) throw new Exception("All IEnumerable<string> assigned to Values must have the same size");
            }

            if (size.GetValueOrDefault(0) == 0)
            {
                throw new Exception("Values cannot have empty IEnumerable<string>");
            }

            if (BinderIndex >= Values.Count())
            {
                throw new Exception("BinderIndex must be less than the size of Values");
            }
        }
    }
}