using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MovingObject
{
    public override void Collide()
    {
        /*StatsController.Instance.CurrencyGain(1);
        InterfaceController.Instance.UpdateGameCurrency(1);*/
        Destroy(gameObject);
    }
}
