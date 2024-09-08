using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MovingObject
{
    public bool Collided;

    protected override void Start()
    {
        base.Start();

        var rb = GetComponent<Rigidbody2D>();

        rb.rotation = Random.Range(0f, 360f);
    }

    public override void Collide()
    {
        if (Collided)
            return;

        RunnerManager.Instance.Collide();
        Collided = true;
    }
}
