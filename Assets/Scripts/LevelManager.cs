using UnityEngine;

public class LevelManager : MonoBehaviour
{
    void Awake()
    {
        ServiceLocator.Register(this);
    }
}
