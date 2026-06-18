using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    protected Dictionary<Vector2Int, Block> _occupiedCellDict = new Dictionary<Vector2Int, Block>();
    protected Dictionary<Vector2Int, Tile> _tilesDict = new Dictionary<Vector2Int, Tile>();
    protected Dictionary<Vector2Int, Block> _blockDict = new Dictionary<Vector2Int, Block>();

    [ShowInInspector, ReadOnly] protected LevelDataSO _levelData;

    public float tileSize = 1.0f;
    public GameConfigSO gameConfig;

    void Awake()
    {
        ServiceLocator.Register(this);
    }
    
    public void Initialize(LevelDataSO levelData)
    {
        gameConfig = ServiceLocator.Get<GameConfigSO>();
        _levelData = levelData;
        CenterGridOnScreen();
        SpawnTiles();
        SpawnBlocks();
    }

    void CenterGridOnScreen()
    {
        Vector3 worldCenter = Vector3.zero;
        if (Camera.main != null)
        {
            float zDistance = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
            worldCenter = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, zDistance));
            worldCenter.z = 0f;
        }

        float halfMapWidth = ((_levelData.width * tileSize) - tileSize) / 2f;
        float halfMapHeight = ((_levelData.height * tileSize) - tileSize) / 2f;
        transform.position = worldCenter - new Vector3(halfMapWidth, halfMapHeight, 0f);
    }

    [Button]
    public void SpawnTiles()
    {
        var prefab = gameConfig.tilePrefab;
        GameObject tileContainer = new GameObject("Tile Container");
        tileContainer.transform.SetParent(transform);

        if (_levelData.tiles.Count == 0)
        {
            for (int y = 0; y < _levelData.height; ++y)
            {
                for (int x = 0; x < _levelData.width; ++x)
                {
                    GameObject tileGO = Instantiate(prefab);
                    tileGO.transform.SetParent(tileContainer.transform, false);
                    tileGO.name = $"Tile_{x}_{y}";
                    tileGO.transform.position = new Vector3(x * tileSize, y * tileSize, 0);
                    Tile tile = tileGO.GetComponent<Tile>();
                    tile.data = new TileData
                    {
                        position = new Vector2Int(x, y)
                    };
                    _tilesDict[tile.data.position] = tile;
                }
            }
        }
        else
        {
            foreach (var tileData in _levelData.tiles)
            {
                GameObject tileGO = Instantiate(prefab);
                tileGO.transform.SetParent(tileContainer.transform, false);
                tileGO.name = $"Tile_{tileData.position.x}_{tileData.position.y}";
                tileGO.transform.position = new Vector3(tileData.position.x * tileSize, tileData.position.y * tileSize, 0);
                Tile tile = tileGO.GetComponent<Tile>();
                tile.data = tileData;
                _tilesDict[tile.data.position] = tile;
            }
        }
    }

    void SpawnBlocks()
    {
        GameObject blockContainer = new GameObject("Block Container");
        blockContainer.transform.SetParent(transform);

        foreach (var blockData in _levelData.blocks)
        {
            GameObject blockGO = Instantiate(gameConfig.blockPrefabs[blockData.blockType]);
            blockGO.transform.SetParent(blockContainer.transform, false);
            blockGO.transform.position = new Vector3(blockData.position.x * tileSize, blockData.position.y * tileSize, 0);
            blockGO.name = $"Block_{blockData.position.x}_{blockData.position.y}";
            Block block = blockGO.GetComponent<Block>();
            block.data = blockData;
            block.position = blockData.position;
            _blockDict[block.position] = block;
        }

        RebuildOccupiedCellMap();
    }

    public Vector2Int GetGridPosition(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / tileSize);
        int y = Mathf.RoundToInt(worldPos.y / tileSize);
        return new Vector2Int(x, y);
    }

    public Vector2Int ClampToLevel(Vector2Int pos)
    {
        return new Vector2Int(
            Mathf.Clamp(pos.x, 0, _levelData.width - 1),
            Mathf.Clamp(pos.y, 0, _levelData.height - 1)
        );
    }

    void RebuildOccupiedCellMap()
    {
        _occupiedCellDict.Clear();
        foreach (var block in _blockDict.Values)
        {
            foreach (var cell in block.GetOccupiedGridPositions(block.position))
            {
                _occupiedCellDict[cell] = block;
            }
        }
    }

    public bool IsCellOccupied(Vector2Int pos, Block ignore = null)
    {
        if (_occupiedCellDict.TryGetValue(pos, out var occupying))
            return occupying != ignore;
        return false;
    }

    public bool CanPlaceBlock(Block block, Vector2Int origin)
    {
        // Ensure our occupied-cell map is up-to-date before validating placement
        RebuildOccupiedCellMap();
        foreach (var cell in block.GetOccupiedGridPositions(origin))
        {
            if (!IsPositionInsideLevel(cell))
                return false;

            if (IsCellOccupied(cell, block))
                return false;
        }

        return true;
    }

    public bool IsPositionInsideLevel(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < _levelData.width && pos.y >= 0 && pos.y < _levelData.height;
    }

    public bool IsOccupied(Vector2Int pos)
    {
        return _blockDict.ContainsKey(pos);
    }

    public Block GetBlockAt(Vector2Int pos)
    {
        _blockDict.TryGetValue(pos, out var block);
        return block;
    }

    // Attempts to move block to new grid position. Returns true on success.
    public bool TryMoveBlock(Block block, Vector2Int newPos)
    {
        if (block == null) return false;
        if (!CanPlaceBlock(block, newPos)) return false;

        if (_blockDict.ContainsKey(block.position))
            _blockDict.Remove(block.position);

        block.position = newPos;
        _blockDict[newPos] = block;
        block.transform.position = GetWorldPositionForGrid(newPos);
        RebuildOccupiedCellMap();
        return true;
    }

    public Vector3 GetWorldPositionForGrid(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * tileSize, gridPos.y * tileSize, 0);
    }
}
