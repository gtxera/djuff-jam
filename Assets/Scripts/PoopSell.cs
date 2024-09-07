using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoopSell : MonoBehaviour
{
    [SerializeField] int poopPrice;

    private void Start()
    {
        poopPrice = Random.Range(10, 50);
    }

    public void Sell()
    {
        StatsController.Instance.CurrencyGain(poopPrice);
        StatsController.Instance.PoopUpdateCount(1);
        Destroy(gameObject);
    }
}
