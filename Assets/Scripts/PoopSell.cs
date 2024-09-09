using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class PoopSell : MonoBehaviour
{
    [SerializeField] int poopPrice;
    [SerializeField] private EventReference _poopEvent;

    private void Start()
    {
        poopPrice = Random.Range(1, 3);
    }

    public void Sell()
    {
        StatsController.Instance.CurrencyGain(poopPrice);
        StatsController.Instance.RemovePoop();
        RuntimeManager.PlayOneShot(_poopEvent);
        Destroy(gameObject);
    }
}
