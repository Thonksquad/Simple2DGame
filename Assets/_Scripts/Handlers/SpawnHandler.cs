using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SpawnHandler : MonoBehaviour
{
    [SerializeField] private Spawn _shapePrefab;
    [SerializeField] private int _spawnLimit = 10;
    [SerializeField] private int _poolLimit = 30;
    [SerializeField] private bool _usePool;
    public Coroutine CurrentSpawn;
    private ObjectPool<Spawn> _pool;

    private void OnEnable()
    {
        Spawn.HitPlayer += HandleDefeat;
    }

    private void OnDisable()
    {
        Spawn.HitPlayer -= HandleDefeat;
    }

    private void Start()
    {
        _pool = new ObjectPool<Spawn>(() => {
            //Spawning
            return Instantiate(_shapePrefab);
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

        CurrentSpawn = StartCoroutine(SpawnObstacles(3));
    }

    private void SpawnObject()
    {
        if (_spawnLimit < _poolLimit)
        {
            _spawnLimit += 2;
        }

        for (int i = 0; i < _spawnLimit; i++)
        {
            var spawn = _usePool ? _pool.Get() : Instantiate(_shapePrefab);
            spawn.transform.position = transform.position + Random.insideUnitSphere * 18;
            spawn.Init(KillSpawn);
        }
    }

    private void KillSpawn(Spawn spawn)
    {
        if (_usePool) _pool.Release(spawn);
        else Destroy(spawn.gameObject);
    }

    public IEnumerator SpawnObstacles (int delay)
    {
        while (true)
        {
            SpawnObject();
            yield return new WaitForSeconds(delay);
        }
    }

    private void HandleDefeat()
    {
        StopCoroutine(CurrentSpawn);
    }

}
