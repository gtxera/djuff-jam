using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class RunnerManager : SingletonBehaviour<RunnerManager>
{
    public float Velocity { get; private set; }

    public event Action RunnerStarted;

    public event Action RunnerGameOver;

    public event Action RunnerWin;

    public event Action RunnerLeave;

    private float _acceleration;

    [SerializeField] private float _defaultDuration = 4f;
    [SerializeField] private float _initialVelocity;

    [SerializeField] private Asteroid[] _asteroidPrefab;
    [SerializeField] private Coin[] _coinPrefab;
    [SerializeField] private TimePickup[] _timePickupPrefab;

    [SerializeField] private float _asteroidSpawnInterval;
    [SerializeField] private float _asteroidSpawnDeviation;

    [SerializeField] private float _coinSpawnInterval;
    [SerializeField] private float _coinSpawnDeviation;

    [SerializeField] private float _timePickupSpawnInterval;
    [SerializeField] private float _timePickupSpawnDeviation;

    [SerializeField] private float _hitInvulnerabilityTime;

    [SerializeField] private float _victoryVelocity;
    [SerializeField] private float _distortionScale;
    [SerializeField] private GameObject _distortionRoot;

    [SerializeField] private GameObject _managerRoot;
    [SerializeField] private GameObject _gameOverScreen;

    private bool _hitRecently;
    private float _lastHitTime;

    private Spawner _asteroidSpawner;
    private Spawner _coinSpawner;
    private Spawner _timePickupSpawner;

    private float _remaningTime;

    private int _lifes;
    private int _maxLifes;

    private bool _running;

    private float _initialCameraSize;
    private float _targetCameraSize;
    private float _velocityDifference;
    private float _currentDistortion = 1f;

    private int _currencyCollected;

    protected override void Awake()
    {
        base.Awake();

        gameObject.SetActive(false);

        _asteroidSpawner = new Spawner(_asteroidPrefab, _asteroidSpawnInterval, _asteroidSpawnDeviation);
        _coinSpawner = new Spawner(_coinPrefab, _coinSpawnInterval, _coinSpawnDeviation);
        _timePickupSpawner = new Spawner(_timePickupPrefab, _timePickupSpawnInterval, _timePickupSpawnDeviation);

        _initialCameraSize = Camera.main.orthographicSize;
        _targetCameraSize = (_initialCameraSize * _distortionScale + _initialCameraSize) / 2;

        _velocityDifference = _victoryVelocity - _initialVelocity;
    }

    public void GoToRunner()
    {
        _managerRoot.SetActive(false);
        gameObject.SetActive(true);

        MenuController.Instance.SwitchScreen((int)MenuController.Screens.gameScreen);

        _currencyCollected = 0;
        InterfaceController.Instance.UpdateGameCurrency(0);

        _maxLifes = 1;

        _maxLifes += StatsController.Instance.extraLife;
        InterfaceController.Instance.UpdateMaxGameLife(_maxLifes);

        _lifes = _maxLifes;
        InterfaceController.Instance.UpdateGameLife(_lifes);

        _remaningTime = _defaultDuration;

        _remaningTime += GetExtraDuration(StatsController.Instance.hungry / StatsController.Instance.maxHungry);
        _remaningTime += GetExtraDuration(StatsController.Instance.thirst / StatsController.Instance.maxThirst);
        _remaningTime += GetExtraDuration((3 - StatsController.Instance.currentDirty) / 3);

        InterfaceController.Instance.UpdateGameTime(_remaningTime);

        StartGame();
    }

    private void StartGame()
    {
        _running = true;

        Velocity = _initialVelocity;

        RunnerStarted?.Invoke();

        _asteroidSpawner.Reset();
        _coinSpawner.Reset();
        _timePickupSpawner.Reset();

        _currentDistortion = 1f;

        if (StatsController.Instance is null)
            return;

        _acceleration = StatsController.Instance.extraSpeed;
    }

    public void GameOver()
    {
        RunnerGameOver?.Invoke();

        _running = false;

        _gameOverScreen.SetActive(true);
    }

    public void GainTime(float time)
    {
        if (_remaningTime < 15f)
            _remaningTime = Mathf.Clamp(_remaningTime + time, 0, 15);
    }

    public void GainCoin()
    {
        _currencyCollected += 1;
        InterfaceController.Instance.UpdateGameCurrency(_currencyCollected);
    }

    public void GainLife()
    {
        if (_lifes < _maxLifes)
        {
            _lifes += 1;
            InterfaceController.Instance.UpdateGameLife(_lifes);
        }
    }

    public void Collide()
    {
        if (_hitRecently)
            return;

        Velocity -= Velocity * 0.15f;
        _lifes -= 1;

        InterfaceController.Instance.UpdateGameLife(_lifes);

        _hitRecently = true;

        if (_lifes <= 0)
        {
            GameOver();
        }
    }

    public void LeaveRunner()
    {
        RunnerLeave?.Invoke();

        MenuController.Instance.SwitchScreen((int)MenuController.Screens.managerScreen);
        gameObject.SetActive(false);
        _managerRoot.SetActive(true);
        _distortionRoot.transform.localScale = Vector3.one;

        StatsController.Instance.SpawnPoop();
        StatsController.Instance.thirst -= 20;
        StatsController.Instance.hungry -= 20;
        InterfaceController.Instance.UpdateThirst(StatsController.Instance.thirst);
        InterfaceController.Instance.UpdateHungry(StatsController.Instance.hungry);

        StatsController.Instance.CurrencyGain(_currencyCollected);
        InterfaceController.Instance.UpdateManagerCurrency(StatsController.Instance.CurrentCurrency);
    }

    private void Update()
    {
        if (!_running)
            return;

        var dt = Time.deltaTime;

        Velocity += _acceleration * dt;

        _asteroidSpawner.Update();
        _coinSpawner.Update();
        _timePickupSpawner.Update();

        if (_hitRecently)
        {
            _lastHitTime += dt;

            if (_lastHitTime >= _hitInvulnerabilityTime)
            {
                _hitRecently = false;
                _lastHitTime = 0f;
            }
        }

        var velocityPercent = Mathf.InverseLerp(_initialVelocity, _victoryVelocity, Velocity);

        float lighspeedEase = velocityPercent;
        for (int i = 0; i < 10; i++)
        {
            lighspeedEase *= velocityPercent;
        }

        InterfaceController.Instance.UpdateGameSpeed(lighspeedEase);

        Physics2D.SyncTransforms();

        _remaningTime -= dt;

        if (_remaningTime <= 0)
        {
            GameOver();
        }

        InterfaceController.Instance.UpdateGameTime(_remaningTime);

        Debug.Log(Velocity);

        if (Velocity >= _victoryVelocity)
        {
            RunnerWin?.Invoke();
            _running = false;
        }
    }

    private float GetExtraDuration(float statsPercentage)
    {
        if (statsPercentage >= 1f)
            return 2f;
        if (statsPercentage >= 0.5f)
            return 1f;
        else
            return 0f;
    }

    private void SpawnObject(GameObject prefab)
    {
        var collider = prefab.GetComponent<Collider2D>();
        Vector2 spawnPos;

        int attempts = 0;

        do
        {
            var cameraSize = Camera.main.orthographicSize;

            spawnPos = new Vector2(cameraSize * 2f * Camera.main.aspect + 3f, Random.Range(-cameraSize + 0.1f, cameraSize - 0.1f));

            attempts += 1;

        } while (Physics2D.OverlapBox(spawnPos, (Vector2)collider.bounds.size, 0f) && attempts <= 100);

        Instantiate(prefab, (Vector3)spawnPos, Quaternion.identity, _distortionRoot.transform);
    }

    private class Spawner
    {
        private MovingObject[] _prefabs;
        private float _spawnInterval;
        private float _spawnDeviation;

        private float _elapsedTime;
        private float _currentInterval;

        public Spawner(MovingObject[] prefabs, float spawnInterval, float spawnDeviation)
        {
            _prefabs = prefabs;
            _spawnInterval = spawnInterval;
            _spawnDeviation = spawnDeviation;
        }

        public void Reset()
        {
            _elapsedTime = 0f;
            _currentInterval = RandomGaussian(_spawnInterval - _spawnDeviation, _spawnInterval + _spawnDeviation);
        }

        public void Update()
        {
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime >= _currentInterval)
            {
                RunnerManager.Instance.SpawnObject(_prefabs[Random.Range(0, _prefabs.Length)].gameObject);
                Reset();
            }
        }

        public static float RandomGaussian(float minValue = 0.0f, float maxValue = 1.0f)
        {
            float u, v, S;

            do
            {
                u = 2.0f * UnityEngine.Random.value - 1.0f;
                v = 2.0f * UnityEngine.Random.value - 1.0f;
                S = u * u + v * v;
            }
            while (S >= 1.0f);

            // Standard Normal Distribution
            float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

            // Normal Distribution centered between the min and max value
            // and clamped following the "three-sigma rule"
            float mean = (minValue + maxValue) / 2.0f;
            float sigma = (maxValue - mean) / 3.0f;
            return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
        }
    }
}
