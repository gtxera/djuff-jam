using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : SingletonBehaviour<MenuController>
{
    public enum Screens
    {
        menuScreen,
        configScreen,
        managerScreen,
        gameScreen,
        pauseScreen,
        creditsScreen
    }

    bool isPaused;

    [Header("Screens Variables")]
    [SerializeField] GameObject menuScreen;
    [SerializeField] GameObject configScreen;
    [SerializeField] GameObject managerScreen;
    [SerializeField] GameObject gameScreen;
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject creditsScreen;
 
    Screens currentScreen;
    Screens lastScreen;

    public void SwitchScreen(int screenIndex)
    {
        lastScreen = currentScreen;
        currentScreen = (Screens)screenIndex;
        menuScreen.SetActive(false);
        configScreen.SetActive(false);
        managerScreen.SetActive(false);
        gameScreen.SetActive(false);
        pauseScreen.SetActive(false);
        creditsScreen.SetActive(false);

        switch (currentScreen)
        {
            case Screens.menuScreen:
                menuScreen.SetActive(true);
                break;
            case Screens.configScreen:
                configScreen.SetActive(true);
                break;
            case Screens.managerScreen:
                managerScreen.SetActive(true);
                break;
            case Screens.gameScreen:
                gameScreen.SetActive(true);
                break;
            case Screens.pauseScreen:
                pauseScreen.SetActive(true);
                break;
            case Screens.creditsScreen:
                creditsScreen.SetActive(true);
                break;
        }
    }

    public void PauseUnpause()
    {
        if (isPaused)
        {
            switch (PlayerController.Instance.states)
            {
                case PlayerStates.manager:
                    SwitchScreen((int)Screens.managerScreen);
                    break;
                case PlayerStates.runner:
                    SwitchScreen((int)Screens.gameScreen);
                    break;
            }
            Time.timeScale = 1f;
            isPaused = false;
        }
        else
        {
            SwitchScreen((int)Screens.pauseScreen);
            Time.timeScale = 0f;
            isPaused = true;
        }
    }

    public void SwitchToLastScreen()
    {
        SwitchScreen((int)lastScreen);
    }
}
