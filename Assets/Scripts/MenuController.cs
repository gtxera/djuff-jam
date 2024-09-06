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
        // = screenIndex;
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
}
