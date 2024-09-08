using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HamsterScript : MonoBehaviour
{
    [SerializeField] TMP_InputField hamsterNameInput;
    string currentHamsterType;
    public string CurrentHamsterType => currentHamsterType;
    string hamsterName;
    public string HamsterName => hamsterName;
    [SerializeField] Dialogue dialogue;
    [SerializeField] GameObject warningPopup;

    [SerializeField] private bool _resetPrefs;
    // Start is called before the first frame update
    void Start()
    {
        if (_resetPrefs)
        {
            PlayerPrefs.DeleteAll();
        }

        if (PlayerPrefs.HasKey("HamsterName"))
        {
            currentHamsterType = PlayerPrefs.GetString("HamsterType");
            hamsterName = PlayerPrefs.GetString("HamsterName");

            Color color;
            if (ColorUtility.TryParseHtmlString(currentHamsterType, out color))
            {
                PlayerController.Instance.playerSprite.color = color;
            }

            if (!string.IsNullOrEmpty(hamsterName))
            {
                PlayerController.Instance.playerName = hamsterName;
            }

            MenuController.Instance.SwitchScreen((int)MenuController.Screens.managerScreen);
        }
        else
        {
            MenuController.Instance.SwitchScreen((int)MenuController.Screens.menuScreen);
        }
    }

    public void SelectHamster(string hamster)
    {
        currentHamsterType = "#" + hamster;
    }

    public void ConfirmSelection()
    {
        if (string.IsNullOrEmpty(hamsterNameInput.text))
        {
            warningPopup.SetActive(true);
            return;
        }

        hamsterName = hamsterNameInput.text;
        PlayerController.Instance.playerName = hamsterName;

        Color color;
        if (ColorUtility.TryParseHtmlString(currentHamsterType, out color))
        {
            PlayerController.Instance.playerSprite.color = color;
        }

        PlayerPrefs.SetString("HamsterName", hamsterName);
        PlayerPrefs.SetString("HamsterType", currentHamsterType);

        MenuController.Instance.SwitchScreen((int)MenuController.Screens.managerScreen);
        dialogue.StartDialogue();
    }
}
