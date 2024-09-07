using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StatsController : SingletonBehaviour<StatsController>
{
    [Header("Player Stats")]
    public int currentCurrency;
    [SerializeField] float maxHungry;
    [SerializeField] float maxThirst;
    [SerializeField] float maxSpeed;
    [SerializeField] int foodPrice;
    [SerializeField] int waterPrice;
    [SerializeField] int currentDirty;
    float hungry;
    float thirst;
    float speed;

    [Header("UI Components")]
    [SerializeField] Button[] managerOptions;
    bool btnsIsActive;
    [SerializeField] Button waterBtn;
    [SerializeField] Button foodBtn;
    [SerializeField] Button trainSpeedBtn;
    [SerializeField] GameObject poop;
    [SerializeField] Transform poopParent;
    [SerializeField] float spawnRadio;

    private void Start()
    {
        InterfaceController.Instance.UpdateMaxDirty(currentDirty);
        InterfaceController.Instance.UpdateMaxHungry(maxHungry);
        InterfaceController.Instance.UpdateMaxThirst(maxThirst);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) SpawnPoop(3);
    }

    #region Stats Control
    public void DrinkWater(float statsQuantity)
    {
        if (hungry == maxHungry) return;
        thirst = Mathf.Min(thirst + statsQuantity, maxThirst);
        InterfaceController.Instance.UpdateThirst(thirst);
        currentCurrency -= waterPrice;
        InterfaceController.Instance.UpdateManagerCurrency(currentCurrency);
        ActiveDesactiveButtons();
    }

    public void EatFood(float statsQuantity)
    {
        if (thirst == maxThirst) return;
        hungry = Mathf.Min(hungry + statsQuantity, maxHungry);
        InterfaceController.Instance.UpdateHungry(hungry);
        currentCurrency -= foodPrice;
        InterfaceController.Instance.UpdateManagerCurrency(currentCurrency);
        ActiveDesactiveButtons();
    }

    public void TrainSpeed(float statsQuantity)
    {
        speed = Mathf.Min(speed + statsQuantity, maxSpeed);
        trainSpeedBtn.interactable = false;
    }
    #endregion

    #region Economy Control
    public void CurrencyGain(int currencyCount)
    {
        currentCurrency += currencyCount;
        InterfaceController.Instance.UpdateManagerCurrency(currentCurrency);
    }

    public void SpawnPoop(int poopCount)
    {
        int definitivePoopCount = Mathf.Min(poopCount, 10);
        for (int i = 0; i < definitivePoopCount; i++)
        {
            Vector2 randomPos = Random.insideUnitCircle * spawnRadio;
            var _poop = Instantiate(poop, randomPos, Quaternion.identity, poopParent);
        }
        PoopUpdateCount(-poopCount);
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
        ActiveDesactiveButtons();
    }

    public void PoopUpdateCount(int count)
    {
        currentDirty += count;
        InterfaceController.Instance.UpdateDirty(currentDirty);
    }
    #endregion
}
