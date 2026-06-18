using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameConfigSO gameConfig;

    public LevelDataSO levelData;


    public GridManager gridManager;

    void Awake()
    {
        ServiceLocator.Register<GameConfigSO>(gameConfig);
    }
    void Start()
    {
        EventBus.Emit("NEW_GAME");
        
    }

    void OnEnable()
    {
        EventBus.On("NEW_GAME", OnNewGame);
    }

    void OnDisable()
    {
        EventBus.Off("NEW_GAME", OnNewGame);
    }


    void OnNewGame()
    {
        gridManager.Initialize(levelData);
    }
}
