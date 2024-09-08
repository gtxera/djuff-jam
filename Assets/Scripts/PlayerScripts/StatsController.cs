using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StatsController : SingletonBehaviour<StatsController>
{
    [Header("Player Stats")]
    [SerializeField] int currentCurrency;
    public float maxHungry;
    public float maxThirst;
    public int currentDirty;
    public float extraSpeed;
    public int extraLife;
    [SerializeField] float[] extraSpeedValues;
    [SerializeField] int[] extraLifeValues;
    [HideInInspector] public float hungry;
    [HideInInspector] public float thirst;

    [Header("Prices")]
    [SerializeField] int foodPrice;
    [SerializeField] int waterPrice;
    [SerializeField] int[] speedUpgradePrices; 
    [SerializeField] int[] lifeUpgradePrices;
    int speedUpgradeIndex = 0;
    int lifeUpgradeIndex = 0;

    [Header("UI Components")]
    [SerializeField] Button[] managerOptions;
    bool btnsIsActive;
    [SerializeField] Button speedUpgradeBtn;
    [SerializeField] Button lifeUpgradeBtn;

    [Header("World Interface Components")]
    [SerializeField] Button waterBtn;
    [SerializeField] Button foodBtn;
    [SerializeField] Button practiceBtn;
    bool hasTrained;
    [SerializeField] GameObject poop;
    [SerializeField] Transform poopParent;
    [SerializeField] float spawnRadio;

    private void Start()
    {
        hungry = maxHungry;
        thirst = maxThirst;

        InterfaceController.Instance.UpdateMaxDirty(currentDirty);
        InterfaceController.Instance.UpdateMaxHungry(maxHungry);
        InterfaceController.Instance.UpdateMaxThirst(maxThirst);
        InterfaceController.Instance.UpdateManagerCurrency(currentCurrency);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) SpawnPoop(3);
    }

    #region Stats Control
    public void DrinkWater(float statsQuantity)
    {
        if (thirst == maxThirst) return;
        thirst = Mathf.Min(thirst + statsQuantity, maxThirst);
        InterfaceController.Instance.UpdateThirst(thirst);
        currentCurrency -= waterPrice;
        InterfaceController.Instance.UpdateManagerCurrency(currentCurrency);
        ActiveDesactiveButtons();
    }

    public void EatFood(float statsQuantity)
    {
        if (hungry == maxHungry) return;
        hungry = Mathf.Min(hungry + statsQuantity, maxHungry);
        InterfaceController.Instance.UpdateHungry(hungry);
        currentCurrency -= foodPrice;
        InterfaceController.Instance.UpdateManagerCurrency(currentCurrency);
        ActiveDesactiveButtons();
    }

    public void UpgradeSpeed()
    {
        if (speedUpgradePrices[speedUpgradeIndex] > currentCurrency) return;

        currentCurrency -= speedUpgradePrices[speedUpgradeIndex];
        extraSpeed = extraSpeedValues[speedUpgradeIndex];
        speedUpgradeIndex++;
        if (speedUpgradeIndex > speedUpgradePrices.Length)
        {
            speedUpgradeBtn.interactable = false;
            InterfaceController.Instance.UpdateSpeedPrice(0);
        }
        else InterfaceController.Instance.UpdateSpeedPrice(speedUpgradePrices[speedUpgradeIndex]);

        InterfaceController.Instance.UpdateManagerCurrency(currentCurrency);
    }

    public void UpgradeLife()
    {
        if (lifeUpgradePrices[lifeUpgradeIndex] > currentCurrency) return;

        currentCurrency -= lifeUpgradePrices[lifeUpgradeIndex];
        extraLife = extraLifeValues[lifeUpgradeIndex];
        lifeUpgradeIndex++;
        if (lifeUpgradeIndex > lifeUpgradePrices.Length)
        {
            lifeUpgradeBtn.interactable = false;
            InterfaceController.Instance.UpdateLifePrice(0);
        }
        else InterfaceController.Instance.UpdateLifePrice(lifeUpgradePrices[lifeUpgradeIndex]);

        InterfaceController.Instance.UpdateManagerCurrency(currentCurrency);
    }
    #endregion

    #region Economy Control
    public void CurrencyGain(int currencyCount)
    {
        currentCurrency += currencyCount;
        InterfaceController.Instance.UpdateManagerCurrency(currentCurrency);
        ActiveDesactiveButtons();
    }

    public void Practice()
    {
        CurrencyGain(Random.Range(3, 5));
        hasTrained = true;
        practiceBtn.interactable = false;
    }

    public void SpawnPoop(int poopCount)
    {
        int definitivePoopCount = Mathf.Min(poopCount, 3);
        for (int i = 0; i < definitivePoopCount; i++)
        {
            Vector2 randomPos = Random.insideUnitCircle * spawnRadio;
            var _poop = Instantiate(poop, randomPos, Quaternion.identity, poopParent);
        }
        PoopUpdateCount(-poopCount * 30);
    }

   /* private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(poopParent.transform.position, spawnRadio);
    }*/
    #endregion

    #region UI Control
    public void ActiveDesactiveButtons()
    {
        if (currentCurrency < waterPrice || thirst == maxThirst) waterBtn.interactable = false;
        else waterBtn.interactable = true;

        if (currentCurrency < foodPrice || hungry == maxHungry) foodBtn.interactable = false;
        else foodBtn.interactable = true;
    }

    public void SwitchAllManagerBtnsActivation()
    {
        btnsIsActive = !btnsIsActive;
        for (int i = 0; i < managerOptions.Length; i++)
        {
            managerOptions[i].interactable = btnsIsActive;
        }
        /*ActiveDesactiveButtons();
        if (hasTrained) practiceBtn.interactable = false;*/
    }

    public void PoopUpdateCount(int count)
    {
        currentDirty += count;
        InterfaceController.Instance.UpdateDirty(currentDirty);
    }
    #endregion
}
