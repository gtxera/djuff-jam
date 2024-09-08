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

    public void UpdateMaxGameLife(int maxLife)
    {
        lifeSlider.maxValue = maxLife;
        lifeSlider.value = maxLife;
    }

    public void UpdateMaxGameTime(float maxTime)
    {
        timeSlider.maxValue = maxTime;
        timeSlider.value = maxTime;
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
        dirtySlider.value = 3 - dirty;
    }

    public void UpdateSpeedPrice(int price)
    {
        speedPrice.text = price.ToString();
        Debug.Log("aqui");
    }

    public void UpdateLifePrice(int price)
    {
        lifePrice.text = price.ToString();
    }

    public void UpdateGameLife(int life)
    {
        lifeSlider.value = life;
    }

    public void UpdateGameTime(float time)
    {
        timeSlider.value = time;
    }

    public void UpdateGameSpeed(float percentage)
    {
        const float LIGHT_SPEED = 299_792_458;
        var currentSpeed = Mathf.Lerp(100, LIGHT_SPEED, percentage);
        speedInGame.text = $"Velocidade: {currentSpeed} m/s";
    }
    #endregion
}
