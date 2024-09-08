using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoopSell : MonoBehaviour
{
    [SerializeField] int poopPrice;

    private void Start()
    {
        poopPrice = Random.Range(1, 3);
    }

    public void Sell()
    {
        StatsController.Instance.CurrencyGain(poopPrice);
        StatsController.Instance.RemovePoop();
        Destroy(gameObject);
    }
}
