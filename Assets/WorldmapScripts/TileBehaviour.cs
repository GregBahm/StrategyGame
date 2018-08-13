using System;
using System.Collections.Generic;
using UnityEngine;

public class TileBehaviour : MonoBehaviour
{
    public bool Highlit;

    public Worldmap Map;
    public Tile Tile;

    public TileBehaviour PositiveRow;
    public TileBehaviour NegativeRow;
    public TileBehaviour PositiveAscending;
    public TileBehaviour NegativeAscending;
    public TileBehaviour PositiveDescending;
    public TileBehaviour NegativeDescending;

    public IEnumerable<TileBehaviour> Neighbors { get; private set; }

    public Dictionary<Collider, TileBehaviour> ColliderDictionary { get; private set; }

    public Province Province
    {
        get { return _province; }
        set
        {
            if (value != _province)
            {
                SetProvince(value);
            }
        }
    }

    private Material _mat;
    private bool _provincesNeedUpdate;
    private Province _province;
    private MeshCollider _collider;
    public float _highlightPower;

    private int _highlightPowerId;
    private int _factionColorId;

    private void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        _mat = renderer.material;
        ColliderDictionary = GetColliderDictionary();
        _highlightPowerId = Shader.PropertyToID("_HighlightPower");
        _factionColorId = Shader.PropertyToID("_FactionColor");
    }
    
    private void SetProvince(Province newProvince)
    {
        // TODO: Sort this out when you get back to map drawing mechanics

        //if (_province != null)
        //{
        //    _province.Tiles.Remove(this);
        //}
        //newProvince.Tiles.Add(this);
        _province = newProvince;
        //_provincesNeedUpdate = true;
        //foreach (TileBehaviour tile in Neighbors)
        //{
        //    tile._provincesNeedUpdate = true;
        //}
    }

    private void Update()
    {
        if(_provincesNeedUpdate)
        {
            _provincesNeedUpdate = false;
            UpdateConnections();
        }
        _highlightPower = Mathf.Lerp(_highlightPower, Highlit ? 1 : 0, .1f);//Time.deltaTime * Map.HighlightDecaySpeed);
        _mat.SetFloat(_highlightPowerId, _highlightPower);
        _mat.SetColor(_factionColorId, Province.Owner.Color);
    }

    public TileBehaviour GetOffset(int rowOffset, int ascendingColumnOffset)
    {
        return Map.GetTile(Tile.Row + rowOffset, Tile.AscendingColumn + ascendingColumnOffset);
    }

    public void EstablishNeighbors()
    {
        PositiveRow = GetOffset(1, 0);
        NegativeRow = GetOffset(-1, 0);
        PositiveAscending = GetOffset(0, 1);
        NegativeAscending = GetOffset(0, -1);
        PositiveDescending = GetOffset(-1, 1);
        NegativeDescending = GetOffset(1, -1);
        Neighbors = GetNeighbors();
        _collider = GetComponent<MeshCollider>();
    }

    private Dictionary<Collider, TileBehaviour> GetColliderDictionary()
    {
        Dictionary<Collider, TileBehaviour> ret = new Dictionary<Collider, TileBehaviour>();
        foreach (TileBehaviour neighbor in Neighbors)
        {
            ret.Add(neighbor._collider, neighbor);
        }
        ret.Add(_collider, this);
        return ret;
    }

    private IEnumerable<TileBehaviour> GetNeighbors()
    {
        return new TileBehaviour[] {
            PositiveRow,
            NegativeRow,
            PositiveAscending,
            NegativeAscending,
            PositiveDescending,
            NegativeDescending
        };
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