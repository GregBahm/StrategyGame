using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Worldmap : MonoBehaviour
{
    public Tile HighlitTile
    {
        get
        {
            return _highlitTile;
        }
        set
        {
            if(_highlitTile != value)
            {
                if(_highlitTile != null)
                {
                    _highlitTile.Highlit = false;
                }
                if(value != null)
                {
                    value.Highlit = true;
                }
                _highlitTile = value;
            }
        }
    }
    public Tile[] Tiles;
    public GameObject TilePrefab;
    public Transform MapUvs;
    [Range(0, 1)]
    public float TileMargin;
    public Color BackgroundColor;
    public Material SkyMat;

    
    private Faction _factionA;
    private Faction _factionB;
    private Faction _unclaimed;
    private Province _startingProvince;
    private Tile _highlitTile;

    private TileManager _tileManager;
    public int Rows;
    public int Columns;
    public int TilesCount { get { return Rows * Columns; } }
    public float HighlightDecaySpeed;


    private void Start()
    {
        _tileManager = new TileManager(this);
        Tiles = MakeTiles();
        foreach (Tile tile in Tiles)
        {
            tile.EstablishNeighbors();
        }
        _factionA = new Faction(Color.red);
        _factionB = new Faction(Color.blue);
        _unclaimed = new Faction(Color.white);
        _startingProvince = new Province(_unclaimed);
        SetInitialProvince();
    }

    private void SetInitialProvince()
    {
        foreach (Tile tile in Tiles)
        {
            tile.Province = _startingProvince;
        }
        GetTile(0, 0).Province = new Province(_factionA);
        GetTile(Rows / 2, Columns / 2).Province = new Province(_factionB);
    }

    private void Update()
    {
        HighlitTile = _tileManager.GetTileUnderMouse();
        Shader.SetGlobalFloat("_TileMargin", TileMargin);
        Shader.SetGlobalMatrix("_MapUvs", MapUvs.worldToLocalMatrix);
        Shader.SetGlobalColor("_SideColor", BackgroundColor);
        SkyMat.SetColor("_Tint", BackgroundColor);
    }

    private Tile[] MakeTiles()
    {
        Tile[] ret = new Tile[TilesCount];
        for (int row = 0; row < Rows; row++)
        {
            for (int ascendingColumn = 0; ascendingColumn < Columns; ascendingColumn++)
            {
                int index = (row * Columns) + ascendingColumn;
                ret[index] = CreateTile(row, ascendingColumn);
            }
        }
        return ret;
    }

    public Tile GetTile(int row, int ascendingColumn)
    {
        int modRow = MathMod(row, Rows);
        int modColumn = MathMod(ascendingColumn, Columns);
        int index = (modRow * Columns) + modColumn;
        if (index < 0 || index >= Tiles.Length)
        {
            throw new Exception("Bad index (" + index + ")");
        }
        return Tiles[index];
    }

    private static int MathMod(int value, int modolus)
    {
        return (Math.Abs(value * modolus) + value) % modolus;
    }

    private Tile CreateTile(int row, int ascendingColumn)
    {
        int descendingColumn = row + ascendingColumn;
        string providenceName = string.Format("Providence {0} {1} {2}", row, ascendingColumn, descendingColumn);
        GameObject obj = Instantiate(TilePrefab);
        obj.name = providenceName;
        Tile ret = obj.AddComponent<Tile>();
        ret.Map = this;
        ret.Row = row;
        ret.AscendingColumn = ascendingColumn;
        obj.transform.position = _tileManager.GetProvincePosition(row, ascendingColumn);
        return ret;
    }
}
