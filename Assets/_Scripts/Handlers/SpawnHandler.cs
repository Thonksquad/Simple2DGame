using AudioSystem;
using System.Collections;
using System.Collections.Generic;
using TarodevController;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class SpawnHandler : MonoBehaviour
{
    [Header("References to instantiated prefabs")]
    [SerializeField] private Enemy _spikesPrefab;
    [SerializeField] private Revive _revivePrefab;

    [Header("References to external objects")]
    [SerializeField] private PlayerController _player;
    [SerializeField] private RedSwitch _startSwitch;
    [SerializeField] private GameHandler _gameHandler;

    [Header("Spike pool relevant information")]
    [SerializeField] private float _startingSpikes;
    [SerializeField] private float _currentSpikes;
    [SerializeField] private int _maxSpikes;
    [SerializeField] private bool _useSpikesPool;

    [Header("Revive pool relevant information")]
    [SerializeField] private float _startingRevives;
    [SerializeField] private float _currentRevive;
    [SerializeField] private int _maxRevives;
    [SerializeField] private bool _useRevivePool;


    public Coroutine CurrentSpawn;
    private ObjectPool<Enemy> _enemyPool;
    private ObjectPool<Revive> _revivePool;

    private static SpawnHandler _instance = null;

    public static SpawnHandler Instance
    {
        get
        {
            if (_instance == null)
                _instance = (SpawnHandler)FindAnyObjectByType(typeof(SpawnHandler));
            return _instance;
        }
    }

    private void OnEnable()
    {
        _player.TakeDamage += HandleDefeat;
        _player.Revive += HandleRevive;
        _gameHandler.GameFinished += HandleGameFinish;
    }

    private void OnDisable()
    {
        _player.TakeDamage -= HandleDefeat;
        _player.Revive -= HandleRevive;
        _gameHandler.GameFinished -= HandleGameFinish;
    }

    private void Start()
    {
        _enemyPool = new ObjectPool<Enemy>(() => {
            //Spawning
            return Instantiate(_spikesPrefab);
        }, spawn =>
        {
            //Enabling
            spawn.gameObject.SetActive(true);
            spawn.transform.rotation = Quaternion.Euler(0,0,180);
        }, spawn =>
        {
            //Disabling
            spawn.gameObject.SetActive(false);
        }, spawn =>
        {
            //Destroy
            Destroy(spawn.gameObject);
        }, false, 10, 20);

        _revivePool = new ObjectPool<Revive>(() => {
            //Spawning
            return Instantiate(_revivePrefab);
        }, spawn =>
        {
            //Enabling
            spawn.gameObject.SetActive(true);
            spawn.transform.rotation = Quaternion.Euler(0, 0, 180);
        }, spawn =>
        {
            //Disabling
            spawn.gameObject.SetActive(false);
        }, spawn =>
        {
            //Destroy
            Destroy(spawn.gameObject);
        }, false, 10, 20);
    }

    public void HandleStart()
    {
        _currentSpikes = _startingSpikes;
        _currentRevive = _startingRevives;
        CurrentSpawn = StartCoroutine(StartObstacles(3));
    }

    public void HandleGameFinish()
    {
        StopCoroutine(CurrentSpawn);
    }

    private void SpawnObstacles()
    {
        for (int i = 0; i < Mathf.FloorToInt(_currentSpikes); i++)
        {
            var spawn = _useSpikesPool ? _enemyPool.Get() : Instantiate(_spikesPrefab);
            spawn.transform.position = transform.position + (Vector3)Random.insideUnitCircle * 18;
            spawn.Init(KillEnemy);
        }

        if (_currentSpikes < _maxSpikes)
        {
            _currentSpikes += 1.5f;
        }
    }

    private void SpawnRevives()
    {
        for (int i = 0; i < Mathf.FloorToInt(_currentRevive); i++)
        {
            var spawn = _useRevivePool ? _revivePool.Get() : Instantiate(_revivePrefab);
            spawn.transform.position = transform.position + (Vector3)Random.insideUnitCircle * 10;
            spawn.Init(KillRevive);
        }

        if (_currentRevive < _maxRevives)
        {
            _currentRevive += .25f;
        }
    }

    private void KillEnemy(Enemy spawn)
    {
        if (_useSpikesPool) _enemyPool.Release(spawn);
        else Destroy(spawn.gameObject);
    }

    private void KillRevive(Revive spawn)
    {
        if (_useRevivePool) _revivePool.Release(spawn);
        else Destroy(spawn.gameObject);
    }


    public IEnumerator StartObstacles (int delay)
    {
        yield return new WaitForSeconds(2);
        while (_useSpikesPool)
        {
            SpawnObstacles();
            yield return new WaitForSeconds(delay);
        }
    }

    public IEnumerator StartRevives(int delay)
    {
        yield return new WaitForSeconds(2);
        while (_useRevivePool)
        {
            SpawnRevives();
            yield return new WaitForSeconds(delay);
        }
    }

    private void HandleDefeat()
    {
        if (_useSpikesPool)
        {
            _useSpikesPool = false;
            _useRevivePool = true;
            StopCoroutine(CurrentSpawn);
            CurrentSpawn = StartCoroutine(StartRevives(5));
        }
    }

    private void HandleRevive()
    {
        if (_useRevivePool)
        {
            _useRevivePool = false;
            _useSpikesPool = true;
            StopCoroutine(CurrentSpawn);
            CurrentSpawn = StartCoroutine(StartObstacles(3));
        }
    }
}
