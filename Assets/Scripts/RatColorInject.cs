using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RatColorInject : MonoBehaviour
{
    private IEnumerator Start()
    {
        while (PlayerController.Instance is null)
            yield return null;

        PlayerController.Instance.ColorSet += () => GetComponent<Image>().color = PlayerController.Instance.playerSprite.color;
    }

}
