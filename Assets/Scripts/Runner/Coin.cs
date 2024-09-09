using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MovingObject
{
    [SerializeField] private FMODUnity.EventReference _pickupSound;
    
    public override void Collide()
    {
        RunnerManager.Instance.GainCoin();
        FMODUnity.RuntimeManager.PlayOneShot(_pickupSound);
        Destroy(gameObject);
    }
}
