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
    public CollisionCluster ColliderCluster { get; private set; }

    private readonly Worldmap _map;
    private float _highlighting;
    
    public TileDisplay(Tile tile, Worldmap map, GameObject gameObject)
    {
        Tile = tile;
        _map = map;
        GameObject = gameObject;
        Collider = gameObject.GetComponent<MeshCollider>();
        _tileMat = gameObject.GetComponent<MeshRenderer>().sharedMaterial;
    }

    public TileDisplay GetOffset(int rowOffset, int ascendingColumnOffset)
    {
        return _map.GetTile(Tile.Row + rowOffset, Tile.AscendingColumn + ascendingColumnOffset);
    }

    public void SetNeighbors(Worldmap worldMap)
    {
        Neighbors = new TileNeighbors(this, worldMap);
    }

    public void SetCollisionCluster()
    {
        ColliderCluster = new CollisionCluster(this);
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
        Faction preMergeOwner = preMergeGame.GetTilesProvince(Tile).Owner;
        Faction postMergeOwner = postMergeGame.GetTilesProvince(Tile).Owner;
        foreach (MaterialConnection connection in Neighbors.MaterialConnections)
        {
            float preMergeVal = GetConnectionVal(preMergeOwner, connection.Tile, preMergeGame);
            float postMergeVal = GetConnectionVal(postMergeOwner, connection.Tile, postMergeGame);
            float finalVal = Mathf.Lerp(preMergeVal, postMergeVal, mergerProgress);
            _tileMat.SetFloat(connection.MaterialName, finalVal);
        }
    }

    private float GetConnectionVal(Faction tileOwner, Tile neighborTile, GameState state)
    {
        Faction neighborFaction = state.GetTilesProvince(neighborTile).Owner;
        bool connected = tileOwner == neighborFaction;
        return connected ? 1 : 0;
    }

    public void UpdateHighlighting(bool isHighlit, float highlightDecaySpeed)
    {
        _highlighting = Mathf.Lerp(_highlighting, isHighlit ? 0 : 1, highlightDecaySpeed);
        _tileMat.SetFloat("_HighlightPower", _highlighting);
    }

    public class TileNeighbors : IEnumerable<TileDisplay>
    {
        public TileDisplay PositiveRow { get; }
        public TileDisplay NegativeRow { get; }
        public TileDisplay PositiveAscending { get; }
        public TileDisplay NegativeAscending { get; }
        public TileDisplay PositiveDescending { get; }
        public TileDisplay NegativeDescending { get; }

        private readonly IEnumerable<TileDisplay> _neighbors;
        public IEnumerable<MaterialConnection> MaterialConnections { get; }

        public ReadOnlyDictionary<Collider, TileDisplay> ColliderDictionary { get; }

        public TileNeighbors(TileDisplay owner, Worldmap worldMap)
        {
            PositiveRow = owner.GetOffset(1, 0);
            NegativeRow = owner.GetOffset(-1, 0);
            PositiveAscending = owner.GetOffset(0, 1);
            NegativeAscending = owner.GetOffset(0, -1);
            PositiveDescending = owner.GetOffset(-1, 1);
            NegativeDescending = owner.GetOffset(1, -1);
            _neighbors = new[]
            {
                PositiveRow,
                NegativeRow,
                PositiveAscending,
                NegativeAscending,
                PositiveDescending,
                NegativeDescending
            };
            MaterialConnections = new []
            {       
                new MaterialConnection("_PositiveRowConnected", PositiveRow.Tile),
                new MaterialConnection("_NegativeRowConnected", NegativeRow.Tile),
                new MaterialConnection("_PositiveAscendingConnected", PositiveAscending.Tile),
                new MaterialConnection("_NegativeAscendingConnected", NegativeAscending.Tile),
                new MaterialConnection("_PositiveDescendingConnected", PositiveDescending.Tile),
                new MaterialConnection("_NegativeDescendingConnected", NegativeDescending.Tile),
            };
        }

        public IEnumerator<TileDisplay> GetEnumerator()
        {
            return _neighbors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    public class MaterialConnection
    {
        public string MaterialName { get; }
        public Tile Tile { get; }
        public MaterialConnection(string materialName, Tile tile)
        {
            MaterialName = materialName;
            Tile = tile;
        }
    }

    public class CollisionCluster : IEnumerable<TileDisplay>
    {
        private readonly IEnumerable<TileDisplay> _cluster;
        private readonly ReadOnlyDictionary<Collider, TileDisplay> _dictionary;

        public TileDisplay this[Collider mesh] { get { return _dictionary[mesh]; } }

        public CollisionCluster(TileDisplay source)
        {
            _cluster = GetCluster(source);
            _dictionary = new ReadOnlyDictionary<Collider, TileDisplay>(
                _cluster.ToDictionary(item => item.Collider, item => item));
        }

        private static IEnumerable<TileDisplay> GetCluster(TileDisplay source)
        {
            List<TileDisplay> ret = new List<TileDisplay>();
            ret.Add(source);
            ret.AddRange(source.Neighbors);
            return ret;
        }

        public IEnumerator<TileDisplay> GetEnumerator()
        {
            return _cluster.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
