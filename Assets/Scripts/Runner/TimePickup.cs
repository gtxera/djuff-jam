using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimePickup : MovingObject
{
    [SerializeField] private FMODUnity.EventReference _pickupSound;

    public override void Collide()
    {
        RunnerManager.Instance.GainTime(3f);
        FMODUnity.RuntimeManager.PlayOneShot(_pickupSound);
        Destroy(gameObject);
    }
}
