using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class Block : MonoBehaviour
{
    public static readonly Vector2Int origin = new(0, 0);
    public List<Vector2Int> offsets = new();
    public BlockData data;

    // Real time pos
    public Vector2Int position;

    public GameObject visual;

    public IEnumerable<Vector2Int> GetOccupiedGridPositions(Vector2Int origin)
    {
        yield return origin;
        foreach (var offset in offsets)
            yield return origin + offset;
    }

    public IEnumerable<Vector2Int> OccupiedGridPositions => GetOccupiedGridPositions(position);

    protected int _width = 0;
    [ShowInInspector, ReadOnly]
    public int Width
    {
        get
        {
            if (_width == 0)
            {

                Vector2Int max = offsets.OrderByDescending(v => v.x).FirstOrDefault();
                Vector2Int min = offsets.OrderBy(v => v.x).FirstOrDefault();

                _width = Math.Abs(min.x) + Math.Abs(max.x) + 1;
            }
            return _width;
        }
    }

    [ShowInInspector, ReadOnly] protected int _height = 0;
    public int Height
    {
        get
        {
            if (_height == 0)
            {
                Vector2Int max = offsets.OrderByDescending(v => v.y).FirstOrDefault();
                Vector2Int min = offsets.OrderBy(v => v.y).FirstOrDefault();
                _height = Math.Abs(min.y) + Math.Abs(max.y) + 1;
            }
            return _height;
        }
    }



    public void SetHightLight(bool value, Material h) 
    {
        var meshes = GetComponentsInChildren<MeshRenderer>();
        if(value)
        {
            foreach (var item in meshes)
            {
                item.material = h;
                // var mat = item.sharedMaterial;
                // mat.SetColor("Color", Color.red);
            }
        }
        else
        {
            foreach (var item in meshes)
            {
                // var mat = item.sharedMaterial;
                // mat.SetColor("Color", Color.white);
                item.material = h;

                // item.sharedMaterial = mat;
            }
        }
    }
}
