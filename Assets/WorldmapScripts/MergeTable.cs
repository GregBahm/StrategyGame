using System.Collections.Generic;
using System.Collections.ObjectModel;

public class MergeTable
{
    private readonly ReadOnlyDictionary<Province, Province> _dictionary;

    public MergeTable(Dictionary<Province, Province> dictionary)
    {
        _dictionary = new ReadOnlyDictionary<Province, Province>(dictionary);
    }

    /// <summary>
    /// If given a province that is merged into another province during a turn, returns the merged province.
    /// </summary>
    /// <returns>Itself, or the province it merges with.</returns>
    public Province GetPostMerged(Province province)
    {
        return _dictionary[province];
    }
}