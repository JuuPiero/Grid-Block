using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "GameConfig", menuName = "Config/GameConfig")]
public class GameConfigSO : ScriptableObject
{
    public GameObject tilePrefab;

    public List<GameObject> blockPrefabs = new List<GameObject>();
    public List<Color> colors = new List<Color>();
    public List<Material> materials = new List<Material>();



    // public List
}
