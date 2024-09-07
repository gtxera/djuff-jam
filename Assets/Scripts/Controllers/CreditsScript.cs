using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScript : MonoBehaviour
{
    [SerializeField] GameObject endCreditsBtn;

    public void ActiveCreditsBtn()
    {
        endCreditsBtn.SetActive(true);
    }
}
