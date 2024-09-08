using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MovingObject
{
    public override void Collide()
    {
        RunnerManager.Instance.GainCoin();
        Destroy(gameObject);
    }
}
