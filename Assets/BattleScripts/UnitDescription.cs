public class UnitDescription
{
    private readonly string _name;
    public string Name { get { return _name; } }

    private readonly string _longDescription;
    public string LongDescription { get { return _longDescription; } }

    public UnitDescription(string name, string longDescription)
    {
        _name = name;
        _longDescription = longDescription;
    }
}
