using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using System.Linq;
using System.Collections;
using System;

public class TileUnityObject : MonoBehaviour
{
    public Tile Tile;

    public Collider Collider;
    public CollisionCluster ColliderCluster;
    
    public void SetCollisionCluster(UnityObjectManager manager, Map map)
    {
        ColliderCluster = new CollisionCluster(this, manager, map);
    }

    public class CollisionCluster : IEnumerable<TileUnityObject>
    {
        private readonly IEnumerable<TileUnityObject> _cluster;
        private readonly ReadOnlyDictionary<Collider, TileUnityObject> _dictionary;

        public TileUnityObject this[Collider mesh] { get { return _dictionary[mesh]; } }

        public CollisionCluster(TileUnityObject source, UnityObjectManager manager, Map map)
        {
            _cluster = GetCluster(source, manager, map);
            _dictionary = new ReadOnlyDictionary<Collider, TileUnityObject>(
                _cluster.ToDictionary(item => item.Collider, item => item));
        }

        private static IEnumerable<TileUnityObject> GetCluster(TileUnityObject source, UnityObjectManager manager, Map map)
        {

            List<TileUnityObject> ret = new List<TileUnityObject>();
            ret.Add(source);
            ret.Add(TryGetObject(source, manager, map, -1, 0));
            ret.Add(TryGetObject(source, manager, map, 1, 0));
            ret.Add(TryGetObject(source, manager, map, -1, 1));
            ret.Add(TryGetObject(source, manager, map, 1, -1));
            ret.Add(TryGetObject(source, manager, map, 0, -1));
            ret.Add(TryGetObject(source, manager, map, 0, 1));
            return ret.Where(item => item != null).ToArray();
        }

        private static TileUnityObject TryGetObject(TileUnityObject source, UnityObjectManager manager, Map map, int rowOffset, int columnOffset)
        {
            bool withinBounds = map.GetIsWithinBounds(source.Tile, rowOffset, columnOffset);
            if(withinBounds)
            {
                Tile tile = source.Tile.GetOffset(rowOffset, columnOffset);
                return manager.GetUnityObject(tile);
            }
            return null;
        }

        public IEnumerator<TileUnityObject> GetEnumerator()
        {
            return _cluster.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}