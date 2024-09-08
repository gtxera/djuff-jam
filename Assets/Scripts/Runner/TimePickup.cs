using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimePickup : MovingObject
{
    public override void Collide()
    {
        RunnerManager.Instance.GainTime(3f);
        Destroy(gameObject);
    }
}
