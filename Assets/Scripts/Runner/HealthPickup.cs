using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MovingObject
{
    public override void Collide()
    {
        RunnerManager.Instance.GainLife();
        Destroy(gameObject);
    }
}
