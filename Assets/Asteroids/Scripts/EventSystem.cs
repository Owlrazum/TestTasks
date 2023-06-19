using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystem : MonoBehaviour
{
    public static EventSystem Singleton;
    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
    }

    public event Action<Asteroid.Size> OnAsteroidDestroyed;
    public void AsteroidDestroyed(Asteroid.Size size)
    {
        OnAsteroidDestroyed?.Invoke(size);
    }

    public event Action<bool> OnUFODestroyed;
    public void UFODestroyed(bool isByPlayer)
    {
        OnUFODestroyed?.Invoke(isByPlayer);
    }

    public event Action OnPlayerDestroyed;
    public void PlayerDestroyed()
    {
        OnPlayerDestroyed?.Invoke();
    }

    public event Action OnNewGame;
    public void NewGame()
    {
        Debug.Log("New Game Event");
        OnNewGame?.Invoke();
    }

    public event Action OnGameLosed;
    public void GameLosed()
    {
        OnGameLosed?.Invoke();
    }
}
