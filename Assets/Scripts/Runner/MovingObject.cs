using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class MovingObject : MonoBehaviour
{
    private Rigidbody2D _rb2d;

    private bool _running = true;

    protected virtual void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        RunnerManager.Instance.RunnerGameOver += OnRunnerGameOver;
    }

    private void OnDestroy()
    {
        RunnerManager.Instance.RunnerGameOver -= OnRunnerGameOver;
    }

    private void Update()
    {
        if (!_running)
            return;

        var velocity = _rb2d.velocity;
        velocity.x = -RunnerManager.Instance.Velocity;
        _rb2d.velocity = velocity;

        if (_rb2d.position.x < -20f)
            Destroy(gameObject);
    }

    private void OnRunnerGameOver()
    {
        _rb2d.velocity = Vector2.zero;
        _running = false;
    }

    public abstract void Collide();
    
}
