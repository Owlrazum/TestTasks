using System;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField]
    private Image[] hearts;

    private int health;

    private void Start()
    {
        EventSystem.Singleton.OnPlayerDestroyed += ProcessPlayerDestroyed;
        EventSystem.Singleton.OnNewGame += ProcessNewGame;
        health = 3;
    }

    private void ProcessNewGame()
    {
        health = 3;
        foreach (Image im in hearts)
        {
            im.enabled = true;
        }
    }

    private void ProcessPlayerDestroyed()
    {
        ReduceHealth();
    }

    private void ReduceHealth()
    {
        health--;
        hearts[health].enabled = false;
        if (health == 0)
        {
            EventSystem.Singleton.GameLosed();
            return;
        }
    }
}
