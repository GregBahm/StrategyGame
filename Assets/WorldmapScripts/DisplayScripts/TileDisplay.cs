using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class TileDisplay
{
    public GameObject GameObject { get; }

    private readonly Material _tileMat;

    public Tile Tile { get; }

    public Collider Collider { get; }

    public TileNeighbors Neighbors { get; private set; }
    
    private readonly MapDisplay _mapDisplay;
    private float _hoverPower;
    private float _selectPower;
    
    public TileDisplay(Tile tile, MapDisplay map, GameObject gameObject)
    {
        Tile = tile;
        _mapDisplay = map;
        GameObject = gameObject;
        Collider = gameObject.GetComponent<MeshCollider>();
        _tileMat = gameObject.GetComponent<MeshRenderer>().material;
    }

    public TileDisplay GetOffset(int rowOffset, int ascendingColumnOffset)
    {
        return _mapDisplay.GetTile(Tile.Row + rowOffset, Tile.AscendingColumn + ascendingColumnOffset);
    }

    public void SetNeighbors(MapDisplay worldMap)
    {
        Neighbors = new TileNeighbors(this, worldMap);
    }

    public void DisplayTile(GameTurnTransition gameTransition, DisplayTimings timings)
    {
        DisplayProvinceOwnershipChanges(gameTransition.InitialState, gameTransition.PostOwnershipChangesState, timings.ProvinceOwnershipChanges);
        DisplayProvinceMergers(gameTransition.InitialState, gameTransition.PostMergersState, timings.ProvinceMergers);
    }

    private void DisplayProvinceOwnershipChanges(GameState preOwnerGame, GameState postOwnerGame, float progression)
    {
        ProvinceState preOwnerProvince = preOwnerGame.GetTilesProvince(Tile);
        ProvinceState postOwnerProvince = postOwnerGame.GetTilesProvince(Tile);
        Color preColor = preOwnerProvince.Owner.Color;
        Color postColor = preOwnerProvince.Owner.Color;
        Color factionColor = Color.Lerp(preColor, postColor, progression);
        _tileMat.SetColor("_FactionColor", factionColor);
    }

    private void DisplayProvinceMergers(GameState preMergeGame, GameState postMergeGame, float mergerProgress)
    {
        Province preMergeOwner = preMergeGame.GetTilesProvince(Tile).Identifier;
        Province postMergeOwner = postMergeGame.GetTilesProvince(Tile).Identifier;
        foreach (MaterialConnection connection in Neighbors.MaterialConnections)
        {
            float preMergeVal = GetConnectionVal(preMergeOwner, connection.Tile.Tile, preMergeGame);
            float postMergeVal = GetConnectionVal(postMergeOwner, connection.Tile.Tile, postMergeGame);
            float finalVal = Mathf.Lerp(preMergeVal, postMergeVal, mergerProgress);
            _tileMat.SetFloat(connection.MaterialName, finalVal);
        }
    }

    private float GetConnectionVal(Province tileOwner, Tile neighborTile, GameState state)
    {
        Province neighborFaction = state.GetTilesProvince(neighborTile).Identifier;
        bool connected = tileOwner == neighborFaction;
        return connected ? 1 : 0;
    }

    public void UpdateHighlighting(MapInteraction mapInteraction, float highlightDecaySpeed, float timeDelta)
    {
        bool isHovered = mapInteraction.HoveredTile == Tile;
        bool isSelected = mapInteraction.SelectedTile == Tile;
        float speed = highlightDecaySpeed * timeDelta;
        _hoverPower = Mathf.Lerp(_hoverPower, isHovered ? 1 : 0, speed);
        _selectPower = Mathf.Lerp(_selectPower, isSelected ? 1 : 0, speed);
        _tileMat.SetFloat("_HighlightPower", _selectPower);
        _tileMat.SetFloat("_HoverPower", _hoverPower);
    }

    public class TileNeighbors
    {
        private static readonly ConnectionBlueprint[] _connectionBlueprints = new ConnectionBlueprint[]
        {
            new ConnectionBlueprint("_PositiveRowConnected", 1, 0),
            new ConnectionBlueprint("_NegativeRowConnected", -1, 0),
            new ConnectionBlueprint("_PositiveAscendingConnected", 0, 1),
            new ConnectionBlueprint("_NegativeAscendingConnected", 0, -1),
            new ConnectionBlueprint("_PositiveDescendingConnected", -1, 1),
            new ConnectionBlueprint("_NegativeDescendingConnected", 1, -1)
        };
        
        public IEnumerable<MaterialConnection> MaterialConnections { get; }

        public TileNeighbors(TileDisplay owner, MapDisplay worldMap)
        {
            MaterialConnections = GetMaterialConnections(owner, worldMap);
        }

        private IEnumerable<MaterialConnection> GetMaterialConnections(TileDisplay owner, MapDisplay worldMap)
        {
            List<MaterialConnection> ret = new List<MaterialConnection>();
            foreach (ConnectionBlueprint blueprint in _connectionBlueprints)
            {
                bool tileExists = GetDoesTileExist(worldMap.Map, blueprint, owner.Tile);
                if (tileExists)
                {
                    TileDisplay neighbor = GetNeighbor(worldMap, blueprint, owner.Tile);
                    MaterialConnection retItem = new MaterialConnection(blueprint.MaterialPropertyName, neighbor);
                    ret.Add(retItem);
                }
            }
            return ret;
        }

        private bool GetDoesTileExist(Map map, ConnectionBlueprint connectionMap, Tile tile)
        {
            return map.GetIsWithinBounds(tile, connectionMap.RowOffset, connectionMap.ColumnOffset);
        }

        private TileDisplay GetNeighbor(MapDisplay worldMap, ConnectionBlueprint connection, Tile tile)
        {
            int row = tile.Row + connection.RowOffset;
            int column = tile.AscendingColumn + connection.ColumnOffset;
            return worldMap.GetTile(row, column);
        }

        private struct ConnectionBlueprint
        {
            public string MaterialPropertyName { get; }
            public int RowOffset { get; }
            public int ColumnOffset { get; }
            public ConnectionBlueprint(string materialPropertyName, int rowOffset, int columnOffset)
            {
                MaterialPropertyName = materialPropertyName;
                RowOffset = rowOffset;
                ColumnOffset = columnOffset;
            }
        }
    }

    public class MaterialConnection
    {
        public string MaterialName { get; }
        public TileDisplay Tile { get; }
        public MaterialConnection(string materialName, TileDisplay tile)
        {
            MaterialName = materialName;
            Tile = tile;
        }
    }
}
