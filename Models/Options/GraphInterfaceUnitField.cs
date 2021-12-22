namespace GraphInterface.Models.Options
{
    public class GraphInterfaceUnitField
    {
        public string Name { get; set; }
        public string As { get; set; } = null;

        public GraphInterfaceUnitField(string name)
        {
            Name = name;
            As = null;
        }

        public GraphInterfaceUnitField(string name, string asName)
        {
            Name = name;
            As = asName;
        }
    }
}