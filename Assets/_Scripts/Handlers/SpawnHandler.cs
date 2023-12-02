using AudioSystem;
using System.Collections;
using System.Collections.Generic;
using TarodevController;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class SpawnHandler : MonoBehaviour
{
    [SerializeField] private Enemy _spikesPrefab;
    [SerializeField] private Revive _revivePrefab;
    [SerializeField] private float _currentSpikes = 10;
    [SerializeField] private float _currentRevive = 1;
    [SerializeField] private int _MaxSpikes = 20;
    [SerializeField] private int _MaxRevives = 5;
    [SerializeField] private bool _useSpikesPool;
    [SerializeField] private bool _useRevivePool;
    [SerializeField] private PlayerController _player;
    [SerializeField] private StartSwitch _startSwitch;

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

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        _player.TakeDamage += HandleDefeat;
        _player.Revive += HandleRevive;
    }

    private void OnDisable()
    {
        _player.TakeDamage -= HandleDefeat;
        _player.Revive -= HandleRevive;
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

    public void StartGame()
    {
        CurrentSpawn = StartCoroutine(StartObstacles(3));
    }


    private void SpawnObstacles()
    {
        if (_currentSpikes < _MaxSpikes)
        {
            _currentSpikes += 2;
        }

        for (int i = 0; i < _currentSpikes; i++)
        {
            var spawn = _useSpikesPool ? _enemyPool.Get() : Instantiate(_spikesPrefab);
            spawn.transform.position = transform.position + (Vector3)Random.insideUnitCircle * 18;
            spawn.Init(KillEnemy);
        }
    }

    private void SpawnRevives()
    {
        if (_currentRevive < _MaxRevives)
        {
            _currentRevive += .25f;
        }

        for (int i = 0; i < Mathf.FloorToInt(_currentRevive); i++)
        {
            var spawn = _useRevivePool ? _revivePool.Get() : Instantiate(_revivePrefab);
            spawn.transform.position = transform.position + (Vector3)Random.insideUnitCircle * 18;
            spawn.Init(KillRevive);
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
        //Ensures the coroutine is only called once
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
        //Ensures that the coroutine is only called once, bugged
        if (_useRevivePool)
        {
            _useRevivePool = false;
            _useSpikesPool = true;
            StopCoroutine(CurrentSpawn);
            CurrentSpawn = StartCoroutine(StartObstacles(3));
        }
    }
}
