using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    private bool isVisible;
    private bool isGamePlayed;
    [SerializeField]
    private GameObject activeBuffer;
    [SerializeField]
    private GameObject continueButton;

    private void Start()
    {
        isVisible = false;
        isGamePlayed = true;
        EventSystem.Singleton.OnGameLosed += ProcessLoss;
    }
    private void ProcessLoss()
    {
        isGamePlayed = false;
        continueButton.SetActive(false);
        if (!isVisible)
        {
            SetPause();
        }
    }
    public void NewGame()
    {
        isGamePlayed = true;
        continueButton.SetActive(true);
        SetPause();
        EventSystem.Singleton.NewGame();
    }
    public void Continue()
    {
        SetPause();
    }
    public void Exit()
    {
        Application.Quit();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isGamePlayed)
        {
            SetPause();
        }
    }
    private void SetPause()
    {
        isVisible = !isVisible;
        Time.timeScale = isVisible ? 0 : 1;
        activeBuffer.SetActive(isVisible);
    }
}
