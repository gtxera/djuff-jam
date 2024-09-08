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
        creditsScreen
    }

    bool isPaused;

    [Header("Screens Variables")]
    [SerializeField] GameObject menuScreen;
    [SerializeField] GameObject configScreen;
    [SerializeField] GameObject managerScreen;
    [SerializeField] GameObject gameScreen;
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
            case Screens.creditsScreen:
                creditsScreen.SetActive(true);
                break;
        }
    }

    public void NewGame()
    {
        PlayerPrefs.DeleteAll();
        SwitchScreen((int)Screens.menuScreen);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void PauseUnpause()
    {
        if (isPaused)
        {
            SwitchScreen((int)Screens.gameScreen);
            Time.timeScale = 1f;
            isPaused = false;
        }
        else
        {
            SwitchScreen((int)Screens.configScreen);
            Time.timeScale = 0f;
            isPaused = true;
        }
    }

    public void SwitchToLastScreen()
    {
        SwitchScreen((int)lastScreen);
    }
}
