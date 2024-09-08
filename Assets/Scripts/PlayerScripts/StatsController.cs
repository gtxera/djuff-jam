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
    int poopCount;
    [SerializeField] GameObject poop;
    [SerializeField] Transform poopParent;
    [SerializeField] float spawnRadio;

    public int CurrentCurrency => currentCurrency;

    private void Start()
    {

        if (PlayerPrefs.HasKey("Currency"))
        {
            currentCurrency = PlayerPrefs.GetInt("Currency");
            hungry = PlayerPrefs.GetFloat("Hungry");
            thirst = PlayerPrefs.GetFloat("Thirst");
            currentDirty = PlayerPrefs.GetInt("Dirty");
            SetupPoop(currentDirty);
            speedUpgradeIndex = PlayerPrefs.GetInt("SpeedIndex");
            lifeUpgradeIndex = PlayerPrefs.GetInt("LifeIndex");
            extraLife = extraLifeValues[lifeUpgradeIndex];
            extraSpeed = extraSpeedValues[speedUpgradeIndex];

            InterfaceController.Instance.UpdateSpeedPrice(speedUpgradePrices[speedUpgradeIndex]);
            InterfaceController.Instance.UpdateLifePrice(lifeUpgradePrices[lifeUpgradeIndex]);
        }
        else
        {
            hungry = maxHungry;
            thirst = maxThirst;
            currentDirty = 0;

            PlayerPrefs.SetFloat("Hungry", hungry);
            PlayerPrefs.SetFloat("Thirst", thirst);
            PlayerPrefs.SetInt("Dirty", currentDirty);

            SetLife(true);
            SetSpeed(true);
        }


        InterfaceController.Instance.UpdateMaxDirty(3);
        InterfaceController.Instance.UpdateDirty(currentDirty);
        InterfaceController.Instance.UpdateMaxHungry(maxHungry);
        InterfaceController.Instance.UpdateHungry(hungry);
        InterfaceController.Instance.UpdateMaxThirst(maxThirst);
        InterfaceController.Instance.UpdateThirst(thirst);
        InterfaceController.Instance.UpdateManagerCurrency(currentCurrency);

        ActiveDesactiveButtons();
    }

    #region Stats Control
    public void RestartStats()
    {
        currentCurrency = 10;
        hungry = maxHungry;
        thirst = maxThirst;
        currentDirty = 0;
        lifeUpgradeIndex = 0;
        speedUpgradeIndex = 0;
        InterfaceController.Instance.UpdateMaxDirty(3);
        InterfaceController.Instance.UpdateDirty(poopCount);
        InterfaceController.Instance.UpdateMaxHungry(maxHungry);
        InterfaceController.Instance.UpdateHungry(hungry);
        InterfaceController.Instance.UpdateMaxThirst(maxThirst);
        InterfaceController.Instance.UpdateThirst(thirst);
        InterfaceController.Instance.UpdateManagerCurrency(currentCurrency);
        extraLife = 0;
        extraSpeed = 0;
    }

    public void DrinkWater(float statsQuantity)
    {
        if (thirst == maxThirst) return;
        thirst = Mathf.Min(thirst + statsQuantity, maxThirst);
        InterfaceController.Instance.UpdateThirst(thirst);
        currentCurrency -= waterPrice;
        InterfaceController.Instance.UpdateManagerCurrency(currentCurrency);
        ActiveDesactiveButtons();
        PlayerPrefs.SetFloat("Thirst", thirst);
        PlayerPrefs.SetInt("Currency", currentCurrency);
    }

    public void EatFood(float statsQuantity)
    {
        if (hungry == maxHungry) return;
        hungry = Mathf.Min(hungry + statsQuantity, maxHungry);
        InterfaceController.Instance.UpdateHungry(hungry);
        currentCurrency -= foodPrice;
        InterfaceController.Instance.UpdateManagerCurrency(currentCurrency);
        ActiveDesactiveButtons();
        PlayerPrefs.SetFloat("Hungry", hungry);
        PlayerPrefs.SetInt("Currency", currentCurrency);
    }

    public void UpgradeSpeed()
    {
        if (speedUpgradePrices[speedUpgradeIndex] > currentCurrency) return;

        currentCurrency -= speedUpgradePrices[speedUpgradeIndex];
        SetSpeed(false);

        PlayerPrefs.SetInt("SpeedIndex", speedUpgradeIndex);
        PlayerPrefs.SetInt("Currency", currentCurrency);
        InterfaceController.Instance.UpdateManagerCurrency(currentCurrency);
    }

    void SetSpeed(bool isFirstTime)
    {
        extraSpeed = extraSpeedValues[isFirstTime ? 0 : speedUpgradeIndex];
        if (speedUpgradeIndex + 1 >= speedUpgradePrices.Length)
        {
            speedUpgradeBtn.interactable = false;
            InterfaceController.Instance.UpdateSpeedPrice(0);
        }
        else
        {
            speedUpgradeIndex++;
            InterfaceController.Instance.UpdateSpeedPrice(speedUpgradePrices[speedUpgradeIndex]);
        }
    }

    public void UpgradeLife()
    {
        if (lifeUpgradePrices[lifeUpgradeIndex] > currentCurrency) return;

        currentCurrency -= lifeUpgradePrices[lifeUpgradeIndex];
        SetLife(false);

        PlayerPrefs.SetInt("LifeIndex", lifeUpgradeIndex);
        PlayerPrefs.SetInt("Currency", currentCurrency);
        InterfaceController.Instance.UpdateManagerCurrency(currentCurrency);
    }

    void SetLife(bool isFirstTime)
    {
        extraLife = extraLifeValues[isFirstTime ? 0 : lifeUpgradeIndex];
        if (lifeUpgradeIndex + 1 >= lifeUpgradePrices.Length)
        {
            lifeUpgradeBtn.interactable = false;
            InterfaceController.Instance.UpdateLifePrice(0);
        }
        else
        {
            lifeUpgradeIndex++;
            InterfaceController.Instance.UpdateLifePrice(lifeUpgradePrices[lifeUpgradeIndex]);
        }
    }
    #endregion

    #region Economy Control
    public void CurrencyGain(int currencyCount)
    {
        currentCurrency += currencyCount;
        InterfaceController.Instance.UpdateManagerCurrency(currentCurrency);
        ActiveDesactiveButtons();
        PlayerPrefs.SetInt("Currency", currentCurrency);
    }

    public void Practice()
    {
        CurrencyGain(Random.Range(3, 5));
        hasTrained = true;
        practiceBtn.interactable = false;
    }

    public void SpawnPoop()
    {
        if (poopCount == 3) return;

        int definitivePoopCount = Random.Range(1, 3 - poopCount);
        for (int i = 0; i < definitivePoopCount; i++)
        {
            if (poopCount < 3)
            {
                Vector2 randomPos = Random.insideUnitCircle * spawnRadio;
                Instantiate(poop, randomPos, Quaternion.identity, poopParent);
                poopCount++;
            }
        }
        PoopUpdateCount(poopCount);
    }

    private void SetupPoop(int poopAmount)
    {
        for (int i = 0; i < poopAmount; i++)
        {
            if (poopCount < 3)
            {
                Vector2 randomPos = Random.insideUnitCircle * spawnRadio;
                Instantiate(poop, randomPos, Quaternion.identity, poopParent);
                poopCount++;
            }
        }
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

        if (btnsIsActive)
        {
            ActiveDesactiveButtons();
            if (hasTrained) practiceBtn.interactable = false;
        }
    }

    public void PoopUpdateCount(int count)
    {
        currentDirty = count;
        InterfaceController.Instance.UpdateDirty(currentDirty);
        PlayerPrefs.SetInt("Dirty", currentDirty);
    }

    public void RemovePoop()
    {
        currentDirty -= 1;
        poopCount = currentDirty;
        InterfaceController.Instance.UpdateDirty(currentDirty);
        PlayerPrefs.SetInt("Dirty", currentDirty);
    }

    #endregion
}
