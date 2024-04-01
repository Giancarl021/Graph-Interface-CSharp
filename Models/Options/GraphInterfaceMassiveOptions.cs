using System;
using System.Collections.Generic;
using System.Linq;
using GraphInterface.Models.Abstract;

namespace GraphInterface.Options
{
    public class GraphInterfaceMassiveOptions(IEnumerable<IEnumerable<string>> values) : GraphInterfaceRequestOptions
    {
        public new Dictionary<string, string>? Headers { get; set; } = null;
        public Dictionary<string, string> BatchRequestHeaders { get; set; } = [];
        public IEnumerable<IEnumerable<string>> Values { get; set; } = values;
        public uint BinderIndex = 0;
        public uint Attempts { get; set; } = 3;
        public uint RequestsPerAttempt { get; set; } = 50;
        public bool NullifyErrors = false;
        public GraphInterfaceMassiveOptions(IEnumerable<string> values) : this([values]) {}
        public void Assert() {
            if (Values == null)
                throw new ArgumentNullException(nameof(Values));

            int? size = null;

            foreach (var item in Values)
            {
                int count = item.ToList().Count;
                if (size == null) size = count;

                if (size != count) throw new Exception("All collections assigned to Values must have the same size");
            }

            if (size.GetValueOrDefault(0) == 0)
                throw new Exception("Values cannot have empty collections inside Values");

            if (BinderIndex >= Values.Count())
                throw new Exception("BinderIndex must be less than the size of Values");
        }
    }
}