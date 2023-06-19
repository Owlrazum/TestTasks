using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class Score : MonoBehaviour
{
    private int score;
    private TextMeshProUGUI message;

    private void Start()
    {
        message = GetComponent<TextMeshProUGUI>();

        EventSystem.Singleton.OnAsteroidDestroyed += ProcessAsteroidDestroyed;
        EventSystem.Singleton.OnUFODestroyed += ProcessUFODestroyed;
        EventSystem.Singleton.OnNewGame += ProcessNewGame;
    }

    private void ProcessNewGame()
    {
        ScoreReset();
    }

    private void ProcessAsteroidDestroyed(Asteroid.Size size)
    {
        switch (size)
        {
            case Asteroid.Size.Big:
                ScoreIncreased(20);
                break;
            case Asteroid.Size.Medium:
                ScoreIncreased(50);
                break;
            case Asteroid.Size.Small:
                ScoreIncreased(100);
                break;
        }
    }

    private void ProcessUFODestroyed(bool isByPlayer)
    {
        if (!isByPlayer)
        {
            return;
        }
        ScoreIncreased(200);
    }

    private void ScoreIncreased(int inc)
    {
        score += inc;
        message.text = "Score: " + score;
    }

    private void ScoreReset()
    {
        score = 0;
        message.text = "Score: ";
    }
}
