using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PassiveParallax : MonoBehaviour
{
    float length;
    float startPos;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float parallaxEffect;
    [SerializeField] private float _runningSpeedMultiplier;

    private bool _running;

    IEnumerator Start()
    {
        while (RunnerManager.Instance is null)
            yield return null;

        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        RunnerManager.Instance.RunnerStarted += OnRunnerStarted;
        RunnerManager.Instance.RunnerGameOver += OnRunnerEnded;
        RunnerManager.Instance.RunnerWin += OnRunnerEnded;
    }

    private void OnRunnerStarted()
    {
        _running = true;
        Debug.Log("start");
    }

    private void OnRunnerEnded()
    {
        _running = false;
    }

    // Update is called once per frame
    void Update()
    {
        var velocity = new Vector2(parallaxEffect, rb.velocity.y);

        if (_running)
        {
            velocity.x += RunnerManager.Instance.Velocity * _runningSpeedMultiplier;
            velocity.x *= -1f;
        }

        rb.velocity = velocity;

        if (rb.position.x > startPos + length)
        {
            rb.MovePosition(new Vector2(startPos, transform.position.y));
        }
    }
}
