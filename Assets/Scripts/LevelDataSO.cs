using System;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
}

public enum BlockType
{
    
} 
[Serializable]
public class TileData
{
    public Vector2Int position;
}

[Serializable]
public class BlockData
{
    public Vector2Int position;
    public int blockType;

    public int colorType;
}

[CreateAssetMenu(fileName = "LevelDataSO", menuName = "Config/LevelDataSO")]
public class LevelDataSO : ScriptableObject
{
    public int width;
    public int height;

    public List<TileData> tiles = new List<TileData>();

    public List<BlockData> blocks = new List<BlockData>();


}
