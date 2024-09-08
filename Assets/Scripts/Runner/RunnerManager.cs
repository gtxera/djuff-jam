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

    private float _acceleration;

    [SerializeField] private float _defaultDuration = 4f;
    [SerializeField] private float _initialVelocity;

    [SerializeField, Min(0)] private float _spawnRange;
    [SerializeField] private float _spawnX;

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

    private bool _hitRecently;
    private float _lastHitTime;

    private Spawner _asteroidSpawner;
    private Spawner _coinSpawner;
    private Spawner _timePickupSpawner;

    private float _remaningTime;

    private int _lifes;

    private bool _running;

    private float _initialCameraSize;
    private float _velocityDifference;
    private float _currentDistortion = 1f;

    private void Start()
    {
        _asteroidSpawner = new Spawner(_asteroidPrefab, _asteroidSpawnInterval, _asteroidSpawnDeviation);
        _coinSpawner = new Spawner(_coinPrefab, _coinSpawnInterval, _coinSpawnDeviation);
        _timePickupSpawner = new Spawner(_timePickupPrefab, _timePickupSpawnInterval, _timePickupSpawnDeviation);

        _initialCameraSize = Camera.main.orthographicSize;

        _velocityDifference = _victoryVelocity - _initialVelocity;

        StartGame();
    }

    public void StartGame()
    {
        _running = true;

        RunnerStarted?.Invoke();

        _remaningTime = _defaultDuration;

        _asteroidSpawner.Reset();
        _coinSpawner.Reset();
        _timePickupSpawner.Reset();

        _acceleration = 1f;
        _lifes = 3;
        _currentDistortion = 1f;

        if (StatsController.Instance is null)
            return;

        _remaningTime += GetExtraDuration(StatsController.Instance.hungry);
        _remaningTime += GetExtraDuration(StatsController.Instance.thirst);
        _remaningTime += GetExtraDuration(StatsController.Instance.currentDirty);

        _acceleration = StatsController.Instance.extraSpeed;

        _lifes = StatsController.Instance.extraLife;
    }

    public void GameOver()
    {
        RunnerGameOver?.Invoke();

        _running = false;
    }

    public void GainTime(float time)
    {
        _remaningTime += time;
    }

    public void Collide()
    {
        if (_hitRecently)
            return;

        Velocity -= Velocity * 0.15f;
        _lifes -= 1;

        _hitRecently = true;

        if (_lifes <= 0)
        {
            GameOver();
        }
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

        var ease = Mathf.InverseLerp(_initialVelocity, _victoryVelocity, Velocity);

        _currentDistortion = Mathf.Lerp(1f, _distortionScale, ease * ease * ease);

        var scale = _distortionRoot.transform.localScale;
        scale.x = _currentDistortion;
        _distortionRoot.transform.localScale = scale;

        Camera.main.orthographicSize = _initialCameraSize * _currentDistortion;

        Physics2D.SyncTransforms();

        _remaningTime -= dt;

        if (_remaningTime <= 0)
        {
            GameOver();
        }


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

        do
        {
            var cameraSize = Camera.main.orthographicSize;

            spawnPos = new Vector2(_spawnX, Random.Range(-cameraSize + 0.1f, cameraSize - 0.1f));

        } while (Physics2D.OverlapBox(spawnPos, (Vector2)collider.bounds.size, 0f));

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
