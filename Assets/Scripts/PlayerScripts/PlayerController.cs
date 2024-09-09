using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public enum PlayerStates
{
    manager,
    runner
}

public class PlayerController : SingletonBehaviour<PlayerController>
{
    public SpriteRenderer playerSprite;
    public string playerName;
    public PlayerStates states;

    [Header("Runner Stats")]
    [SerializeField] float speed;

    [Header("External Variables")]
    [SerializeField] Animator hamsterAnim;
    [SerializeField] Animator shipAnim;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] GameObject[] eyes;
    public int eyesIndex;

    private void Start()
    {
        StartAnimRandomizing();
    }

    public void StartAnimRandomizing()
    {
        StartCoroutine(ChangeAnimation());
    }

    public void Dissapear()
    {
        var color = playerSprite.color;
        color.a = 0f;
        playerSprite.color = color;

        hamsterAnim.speed = 0f;
    }

    public void Reappear()
    {
        var color = playerSprite.color;
        color.a = 1f;
        playerSprite.color = color;

        hamsterAnim.speed = 1f;
    }

    IEnumerator ChangeAnimation()
    {
        switch (states)
        {
            case PlayerStates.manager:
                for (int i = 0; i <= Random.Range(5, 15); i++)
                {
                    yield return new WaitForSeconds(1f);
                }
                hamsterAnim.SetTrigger("ChangePose");
                for (int i = 0; i < eyes.Length; i++)
                {
                    eyes[i].SetActive(false);
                }
                eyesIndex++;
                yield return new WaitForSeconds(.5f);
                if(eyes.Length < eyesIndex + 1)
                {
                    eyesIndex = 0;
                }
                hamsterAnim.SetInteger("IdlePose", eyesIndex);
                eyes[eyesIndex].SetActive(true);
                StartAnimRandomizing();
                break;
        }
    }
}
