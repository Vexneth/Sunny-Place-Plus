using System;
using UnityEngine;

public class GameEventsManager : MonoBehaviour
{
    public static GameEventsManager instance { get; private set; }

    void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one instance tried to be created!");
        }
        instance = this;
    }

    public event Action<int> OnCollectableCollected;
    public void CollectableCollected(int score)
    {
        OnCollectableCollected?.Invoke(score);
    }

    public event Action<int> OnEnemyDeath;
    public void EnemyDeath(int score)
    {
        OnEnemyDeath?.Invoke(score);
    }

    public event Action OnPlayerDeath;
    public void PlayerDeath()
    {
        OnPlayerDeath.Invoke();
    }

    public event Action OnPauseKeyPressed;
    public void PauseKeyPressed()
    {
        OnPauseKeyPressed.Invoke();
    }

    public event Action OnGamePaused;
    public void GamePaused()
    {
        OnGamePaused.Invoke();
    }

}
