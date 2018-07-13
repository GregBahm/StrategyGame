using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool Highlit;
    private bool _provincesNeedUpdate;
    private Province _province;
    public Province Province
    {
        get { return _province; }
        set
        {
            if(value != _province)
            {
                SetProvince(value);
            }
        }
    }

    private void SetProvince(Province newProvince)
    {
        if(_province != null)
        {
            _province.Tiles.Remove(this);
        }
        newProvince.Tiles.Add(this);
        _province = newProvince;
        _provincesNeedUpdate = true;
        foreach (Tile tile in Neighbors)
        {
            tile._provincesNeedUpdate = true;
        }
    }

    public Worldmap Map;

    public int Row;
    public int AscendingColumn;

    public Tile PositiveRow;
    public Tile NegativeRow;
    public Tile PositiveAscending;
    public Tile NegativeAscending;
    public Tile PositiveDescending;
    public Tile NegativeDescending;

    public IEnumerable<Tile> Neighbors
    {
        get
        {
            yield return PositiveRow;
            yield return NegativeRow;
            yield return PositiveAscending;
            yield return NegativeAscending;
            yield return PositiveDescending;
            yield return NegativeDescending;
        }
    }
    
    private Material _mat;

    private void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        _mat = renderer.material;
    }

    private void Update()
    {
        if(_provincesNeedUpdate)
        {
            _provincesNeedUpdate = false;
            UpdateConnections();
        }
        _mat.SetColor("_FactionColor", Highlit ? Color.green : Province.Owner.Color);
    }

    public Tile GetOffset(int rowOffset, int ascendingColumnOffset)
    {
        return Map.GetTile(Row + rowOffset, AscendingColumn + ascendingColumnOffset);
    }

    public void EstablishNeighbors()
    {
        PositiveRow = GetOffset(1, 0);
        NegativeRow = GetOffset(-1, 0);
        PositiveAscending = GetOffset(0, 1);
        NegativeAscending = GetOffset(0, -1);
        PositiveDescending = GetOffset(-1, 1);
        NegativeDescending = GetOffset(1, -1);
    }

    public void UpdateConnections()
    {
        _mat.SetFloat("_PositiveRowConnected", PositiveRow.Province == Province ? 1 : 0);
        _mat.SetFloat("_NegativeRowConnected", NegativeRow.Province == Province ? 1 : 0);
        _mat.SetFloat("_PositiveAscendingConnected", PositiveAscending.Province == Province ? 1 : 0);
        _mat.SetFloat("_NegativeAscendingConnected", NegativeAscending.Province == Province ? 1 : 0);
        _mat.SetFloat("_PositiveDescendingConnected", PositiveDescending.Province == Province ? 1 : 0);
        _mat.SetFloat("_NegativeDescendingConnected", NegativeDescending.Province == Province ? 1 : 0);
    }
}