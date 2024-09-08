using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToCredtis : MonoBehaviour
{
    public void GoToCredits()
    {
        MenuController.Instance.SwitchScreen((int)MenuController.Screens.creditsScreen);
    } 
}
