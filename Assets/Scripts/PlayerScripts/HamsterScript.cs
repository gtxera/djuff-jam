using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HamsterScript : MonoBehaviour
{
    [SerializeField] TMP_InputField hamsterNameInput;
    string currentHamsterType;
    string hamsterName;
    [SerializeField] Dialogue dialogue;
    [SerializeField] GameObject warningPopup;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("HamsterName"))
        {
            currentHamsterType = PlayerPrefs.GetString("HamsterType");
            hamsterName = PlayerPrefs.GetString("HamsterName");

            Color color;
            if (ColorUtility.TryParseHtmlString(currentHamsterType, out color))
            {
                PlayerController.Instance.playerSprite.color = color;
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
