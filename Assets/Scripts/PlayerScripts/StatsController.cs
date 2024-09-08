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

    private void Start()
    {

        if (PlayerPrefs.HasKey("Currency"))
        {
            currentCurrency = PlayerPrefs.GetInt("Currency");
            hungry = PlayerPrefs.GetFloat("Hungry");
            thirst = PlayerPrefs.GetFloat("Thirst");
            currentDirty = PlayerPrefs.GetInt("Dirty");
            SpawnPoop(currentDirty / 30);
            speedUpgradeIndex = PlayerPrefs.GetInt("SpeedIndex");
            lifeUpgradeIndex = PlayerPrefs.GetInt("LifeIndex");
            SetSpeed(true);
            SetLife(true);
        }
        else
        {
            hungry = maxHungry;
            thirst = maxThirst;
        }

        InterfaceController.Instance.UpdateMaxDirty(currentDirty);
        InterfaceController.Instance.UpdateMaxHungry(maxHungry);
        InterfaceController.Instance.UpdateHungry(hungry);
        InterfaceController.Instance.UpdateMaxThirst(maxThirst);
        InterfaceController.Instance.UpdateThirst(thirst);
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
        InterfaceController.Instance.UpdateSpeedPrice(speedUpgradePrices[speedUpgradeIndex]);
        extraSpeed = extraSpeedValues[speedUpgradeIndex];
        if (speedUpgradeIndex > speedUpgradePrices.Length)
        {
            speedUpgradeBtn.interactable = false;
            InterfaceController.Instance.UpdateSpeedPrice(0);
        }
        else
        {
            if (!isFirstTime) speedUpgradeIndex++;
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
        extraLife = extraLifeValues[lifeUpgradeIndex];
        InterfaceController.Instance.UpdateLifePrice(lifeUpgradePrices[lifeUpgradeIndex]);
        if (lifeUpgradeIndex > lifeUpgradePrices.Length)
        {
            lifeUpgradeBtn.interactable = false;
            InterfaceController.Instance.UpdateLifePrice(0);
        }
        else
        {
            if (!isFirstTime) lifeUpgradeIndex++;
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

    public void SpawnPoop(int _poopCount)
    {
        if (poopCount == 3) return;

        int definitivePoopCount = Mathf.Min(_poopCount, 3);
        for (int i = 0; i < definitivePoopCount; i++)
        {
            if (poopCount < 3)
            {
                Vector2 randomPos = Random.insideUnitCircle * spawnRadio;
                Instantiate(poop, randomPos, Quaternion.identity, poopParent);
                poopCount++;
            }
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

        if (btnsIsActive)
        {
            ActiveDesactiveButtons();
            if (hasTrained) practiceBtn.interactable = false;
        }
    }

    public void PoopUpdateCount(int count)
    {
        currentDirty += count;
        InterfaceController.Instance.UpdateDirty(currentDirty);
        PlayerPrefs.SetInt("Dirty", currentDirty);
    }
    #endregion
}
