using System.Collections.Generic;
using System.Linq;

namespace GraphInterface.Services;
internal class GraphInterfaceMassiveResourcesBuilder(string format, IEnumerable<IEnumerable<string>> values)
{
    private readonly IEnumerable<IEnumerable<string>> _values = values;
    private readonly string _format = format;

    public List<string> Build()
    {
        var resources = new List<string>();
        var formatParams = new List<List<string>>();

        var values = _values.ToList();
        int l = values.Count;

        for (int i = 0; i < l; i++)
        {
            var items = values[i].ToList();
            var s = items.Count;

            for (int j = 0; j < s; j++)
            {
                if (formatParams.ElementAtOrDefault(j) == null)
                    formatParams.Add([items[j]]);
                else
                    formatParams[j].Add(items[j]);
            }
        }

        foreach (var param in formatParams)
            resources.Add(string.Format(_format, [.. param]));

        return resources;
    }
}