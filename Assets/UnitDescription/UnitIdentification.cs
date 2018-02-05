using UnityEngine;

public class UnitIdentification
{
    private readonly string _name;
    public string Name { get { return _name; } }

    private readonly string _longDescription;
    public string LongDescription { get { return _longDescription; } }

    private readonly GameObject _artPrefab;
    public GameObject ArtPrefab { get{ return _artPrefab; } }

    public UnitIdentification(string name, string longDescription, GameObject artPrefab)
    {
        _name = name;
        _longDescription = longDescription;
        _artPrefab = artPrefab;
    }

    public override string ToString()
    {
        return Name;
    }
}
