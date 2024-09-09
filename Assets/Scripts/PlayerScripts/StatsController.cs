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
    public bool hasTrained;
    int poopCount;
    [SerializeField] GameObject poop;
    [SerializeField] Transform poopParent;
    [SerializeField] float spawnRadio;

    [SerializeField] private FMODUnity.EventReference _managerMusic;

    [SerializeField] private Animator _speedAnimator;
    [SerializeField] private Animator _eatAnimator;
    [SerializeField] private Animator _drinkAnimator;

    [SerializeField] private FMODUnity.EventReference _speedEvent;
    [SerializeField] private FMODUnity.EventReference _eatEvent;
    [SerializeField] private FMODUnity.EventReference _drinkEvent;

    [SerializeField] private FMODUnity.EventReference _upgradeEvent;

    private FMOD.Studio.EventInstance _upgradeLifeInstance;
    private FMOD.Studio.EventInstance _upgradeSpeedInstance;

    private bool _doingStuff;

    private FMOD.Studio.EventInstance _musicInstance;

    public int CurrentCurrency => currentCurrency;

    public void PlayManagerMusic()
    {
        _musicInstance.start();
        Debug.Log("musica");
    }

    public void StopManagerMusic()
    {
        _musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private void Start()
    {
        _musicInstance = FMODUnity.RuntimeManager.CreateInstance(_managerMusic);

        _upgradeLifeInstance = FMODUnity.RuntimeManager.CreateInstance(_upgradeEvent);
        _upgradeSpeedInstance = FMODUnity.RuntimeManager.CreateInstance(_upgradeEvent);

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

            PlayManagerMusic();
        }
        else
        {
            hungry = maxHungry;
            thirst = maxThirst;
            currentDirty = 0;

            PlayerPrefs.SetFloat("Hungry", hungry);
            PlayerPrefs.SetFloat("Thirst", thirst);
            PlayerPrefs.SetInt("Dirty", currentDirty);
            PlayerPrefs.SetInt("Currency", currentCurrency);

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

        _doingStuff = true;
        PlayerController.Instance.Dissapear();
        ActiveDesactiveButtons();
        StartCoroutine(Drink(statsQuantity));
    }

    private IEnumerator Drink(float statsQuantity)
    {
        var fmodEvent = FMODUnity.RuntimeManager.CreateInstance(_drinkEvent);

        fmodEvent.start();

        FMOD.Studio.PLAYBACK_STATE state;

        _drinkAnimator.Play("Active");

        fmodEvent.getPlaybackState(out state);

        while (state == FMOD.Studio.PLAYBACK_STATE.PLAYING ||
                state == FMOD.Studio.PLAYBACK_STATE.STARTING)
        {
            yield return null;
            fmodEvent.getPlaybackState(out state);
        }

        _drinkAnimator.Play("Idle");

        _doingStuff = false;
        PlayerController.Instance.Reappear();

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

        _doingStuff = true;
        ActiveDesactiveButtons();
        PlayerController.Instance.Dissapear();
        StartCoroutine(Eat(statsQuantity));
    }

    private IEnumerator Eat(float statsQuantity)
    {
        var fmodEvent = FMODUnity.RuntimeManager.CreateInstance(_eatEvent);

        fmodEvent.start();

        FMOD.Studio.PLAYBACK_STATE state;

        _eatAnimator.Play("Active");

        fmodEvent.getPlaybackState(out state);

        while (state == FMOD.Studio.PLAYBACK_STATE.PLAYING ||
                state == FMOD.Studio.PLAYBACK_STATE.STARTING)
        {
            yield return null;
            fmodEvent.getPlaybackState(out state);
        }

        _eatAnimator.Play("Idle");

        _doingStuff = false;
        PlayerController.Instance.Reappear();

        hungry = Mathf.Min(hungry + statsQuantity, maxHungry);
        InterfaceController.Instance.UpdateHungry(hungry);
        currentCurrency -= foodPrice;
        InterfaceController.Instance.UpdateManagerCurrency(currentCurrency);
        ActiveDesactiveButtons();
        PlayerPrefs.SetFloat("Hungry", thirst);
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
            if (speedUpgradeIndex > 0)
            {
                _upgradeSpeedInstance.setParameterByName("UpgradeState", speedUpgradeIndex);
            }
            _upgradeSpeedInstance.start();
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
            if (lifeUpgradeIndex > 0)
            {
                _upgradeLifeInstance.setParameterByName("UpgradeState", lifeUpgradeIndex);
            }
            _upgradeLifeInstance.start();
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
        if (hasTrained) return;

        _doingStuff = true;
        ActiveDesactiveButtons();
        StartCoroutine(PracticeRoutine());
    }

    private IEnumerator PracticeRoutine()
    {
        var fmodEvent = FMODUnity.RuntimeManager.CreateInstance(_speedEvent);

        fmodEvent.start();

        FMOD.Studio.PLAYBACK_STATE state;

        _speedAnimator.Play("Active");

        fmodEvent.getPlaybackState(out state);

        while (state == FMOD.Studio.PLAYBACK_STATE.PLAYING ||
                state == FMOD.Studio.PLAYBACK_STATE.STARTING)
        {
            yield return null;
            fmodEvent.getPlaybackState(out state);
        }

        _speedAnimator.Play("Idle");

        _doingStuff = false;
        PlayerController.Instance.Reappear();

        CurrencyGain(Random.Range(3, 5));
        hasTrained = true;
        practiceBtn.interactable = false;

        ActiveDesactiveButtons();
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
        if (currentCurrency < waterPrice || thirst == maxThirst || _doingStuff) waterBtn.interactable = false;
        else waterBtn.interactable = true;

        if (currentCurrency < foodPrice || hungry == maxHungry || _doingStuff) foodBtn.interactable = false;
        else foodBtn.interactable = true;

        if (_doingStuff) practiceBtn.interactable = false;
        else practiceBtn.interactable = !hasTrained;
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
