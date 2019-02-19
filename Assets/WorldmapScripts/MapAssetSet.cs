using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class MapAssetSet
{
    public MapTilesSetup MapSetup { get; }

    public Texture2D BaseMap { get; }
    public Texture2D BorderMap { get; }

    public ReadOnlyCollection<NeighborIndices> NeighborsTable { get; }
    
    public MapAssetSet(MapTilesSetup mapSetup, Texture2D baseMap, Texture2D borderMap)
    {
        MapSetup = mapSetup;
        BaseMap = baseMap;
        BorderMap = borderMap;
        Map map = new Map(mapSetup);
        NeighborsTable = LoadNeighborsData(map);
    }
    private ReadOnlyCollection<NeighborIndices> LoadNeighborsData(IEnumerable<Tile> tiles)
    {
        NeighborIndices[] ret = new NeighborIndices[tiles.Max(item => item.BufferIndex) + 1];
        foreach (Tile tile in tiles)
        {
            ret[tile.BufferIndex] = LoadNeighborIndicie(tile);
        }
        return ret.ToList().AsReadOnly();
    }

    private NeighborIndices LoadNeighborIndicie(Tile tile)
    {

        return new NeighborIndices()
        {
            NeighborA = GetBufferIndex(tile.Neighbors.NeighborA),
            NeighborB = GetBufferIndex(tile.Neighbors.NeighborB),
            NeighborC = GetBufferIndex(tile.Neighbors.NeighborC),
            NeighborD = GetBufferIndex(tile.Neighbors.NeighborD),
            NeighborE = GetBufferIndex(tile.Neighbors.NeighborE),
            NeighborF = GetBufferIndex(tile.Neighbors.NeighborF),
        };
    }

    private uint GetBufferIndex(Tile tile)
    {
        if(tile == null)
        {
            return 0;
        }
        return (uint)tile.BufferIndex;
    }

    public struct NeighborIndices
    {
        public uint NeighborA;
        public uint NeighborB;
        public uint NeighborC;
        public uint NeighborD;
        public uint NeighborE;
        public uint NeighborF;
    }
}