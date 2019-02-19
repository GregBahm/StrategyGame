using System.Collections;
using System.Linq;
using UnityEngine;

public class MapAssetSetup : MonoBehaviour
{
    public TextAsset MapDefinitionFile;
    public TextAsset MapTileSetup;
    public Texture2D MapBase;
    public Texture2D MapBorders;

    public MapAssetSet GetMapAssetSet()
    {
        MapTilesBasis mapBasis = new MapTilesBasis(MapDefinitionFile);
        MapTilesSetup mapSetup = new MapTilesSetup(mapBasis, MapTileSetup.text);
        return new MapAssetSet(mapSetup, MapBase, MapBorders);
    }
}
