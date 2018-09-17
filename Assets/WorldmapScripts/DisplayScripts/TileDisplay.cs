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

    public void UpdateConnections()
    {
        // Reimplement this when you get back to implementing the display bit 

        //_tileMat.SetFloat("_PositiveRowConnected", PositiveRow.Province == Province ? 1 : 0);
        //_tileMat.SetFloat("_NegativeRowConnected", NegativeRow.Province == Province ? 1 : 0);
        //_tileMat.SetFloat("_PositiveAscendingConnected", PositiveAscending.Province == Province ? 1 : 0);
        //_tileMat.SetFloat("_NegativeAscendingConnected", NegativeAscending.Province == Province ? 1 : 0);
        //_tileMat.SetFloat("_PositiveDescendingConnected", PositiveDescending.Province == Province ? 1 : 0);
        //_tileMat.SetFloat("_NegativeDescendingConnected", NegativeDescending.Province == Province ? 1 : 0);
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
