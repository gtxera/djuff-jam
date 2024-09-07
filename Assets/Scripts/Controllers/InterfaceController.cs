using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceController : SingletonBehaviour<InterfaceController>
{
    [Header("Player manager stats objects")]
    [SerializeField] Slider hungrySlider;
    [SerializeField] Slider thirstSlider;
    [SerializeField] Slider dirtySlider;
    [SerializeField] TextMeshProUGUI currencyManagerCount;
    [SerializeField] TextMeshProUGUI speedPrice;
    [SerializeField] TextMeshProUGUI lifePrice;

    [Header("Player manager stats objects")]
    [SerializeField] TextMeshProUGUI currencyGameCount;
    [SerializeField] TextMeshProUGUI speedInGame;
    [SerializeField] Slider lifeSlider;
    [SerializeField] Slider timeSlider;

    #region Update Max Values
    public void UpdateMaxHungry(float hungry)
    {
        hungrySlider.maxValue = hungry;
        //hungrySlider.value = hungry;
    }

    public void UpdateMaxThirst(float thirst)
    {
        thirstSlider.maxValue = thirst;
        //thirstSlider.value = thirst;
    }

    public void UpdateMaxDirty(float dirty)
    {
        dirtySlider.maxValue = dirty;
        dirtySlider.value = dirty;
    }
    #endregion

    #region Update Current Values
    public void UpdateManagerCurrency(int currency)
    {
        currencyManagerCount.text = currency.ToString();
    }
    
    public void UpdateGameCurrency(int currency)
    {
        currencyGameCount.text = currency.ToString();
    }

    public void UpdateHungry(float hungry)
    {
        hungrySlider.value = hungry;
    }

    public void UpdateThirst(float thirst)
    {
        thirstSlider.value = thirst;
    }

    public void UpdateDirty(float dirty)
    {
        dirtySlider.value = dirty;
    }

    public void UpdateSpeedPrice(int price)
    {
        speedPrice.text = price.ToString();
    }

    public void UpdateLifePrice(int price)
    {
        lifePrice.text = price.ToString();
    }
    #endregion
}
