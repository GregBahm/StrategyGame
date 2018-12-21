using System;
using UnityEngine;

public class MapmakerTile
{
    public bool IsMasterTile { get; }
    public Material TileMat { get; set; }
    public int Ring { get; }
    public int Row { get; }
    public int Column { get; }
    public string Key { get { return Row + " " + Column; } }
    public MapmakerTileBehavior TileBehavior { get; private set; }
    public MapmakerTile(bool isMaster,
        int ring,
        int row,
        int column)
    {
        IsMasterTile = isMaster;
        Ring = ring;
        Row = row;
        Column = column;
    }

    internal void CreateGameobject(GameObject tilePrefab)
    {
        GameObject obj = GameObject.Instantiate(tilePrefab);
        obj.name = "Ring " + Ring + " row:" + Row + " column:" + Column;
        obj.transform.position = GetProvincePosition(Row, Column, Ring);
        TileMat = obj.GetComponent<MeshRenderer>().material;
        if (IsMasterTile)
        {
            TileBehavior = obj.AddComponent<MapmakerTileBehavior>();
        }
    }
    private Vector3 GetProvincePosition(int row, int ascendingColumn, int ring)
    {
        ring = ring + 2;
        float y = -Mathf.RoundToInt(ring / 5);
        Vector2 ascendingOffset = MapDisplay.AscendingTileOffset * ascendingColumn;
        Vector2 offset = ascendingOffset + new Vector2(row, 0);
        offset *= 2;
        return new Vector3(offset.x, y, offset.y);
    }

    internal string GetSaveLine()
    {
        return Key + "," + TileBehavior.IsImpassable + " " + TileBehavior.IsStartPosition;
    }

    internal void LoadFromSave(string saveContents)
    {
        string[] split = saveContents.Split(' ');
        string isImpassableString = split[0];
        string isStartPositionString = split[1];
        bool isImpassable = Convert.ToBoolean(isImpassableString);
        bool isStartPosition = Convert.ToBoolean(isStartPositionString);
        TileBehavior.IsImpassable = isImpassable;
        TileBehavior.IsStartPosition = isStartPosition;
    }
}
