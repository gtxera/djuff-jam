using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

using Random = UnityEngine.Random;

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

    public  Action ColorSet;

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
        playerSprite.gameObject.SetActive(false);

        hamsterAnim.speed = 0f;

        StopCoroutine(ChangeAnimation());
    }

    public void Reappear()
    {
        playerSprite.gameObject.SetActive(true);

        hamsterAnim.speed = 1f;

        StartCoroutine(ChangeAnimation());

        eyesIndex = 0;

        eyes[eyesIndex].SetActive(true);
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
                var lastAnimName = hamsterAnim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                Debug.Log(lastAnimName);
                hamsterAnim.SetTrigger("ChangePose");
                for (int i = 0; i < eyes.Length; i++)
                {
                    eyes[i].SetActive(false);
                }
                yield return new WaitUntil(() => 
                {
                    var animatorState = hamsterAnim.GetCurrentAnimatorStateInfo(0);
                    return !animatorState.IsName(lastAnimName) && !animatorState.IsName("Idle2") && !animatorState.IsName("Idle2 0");
                });
                eyesIndex++;
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
