using AudioSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    [SerializeField] AudioClip _soundClip;
    private Action<Spawn> _killAction;
    private Transform _ground;
    private Transform _player;

    // Global event
    public static event Action HitPlayer;

    private void Awake()
    {
        _ground = GameObject.FindWithTag("ObstacleCollector").transform;
        _player = GameObject.FindWithTag("Player").transform;
    }

    public void Init(Action<Spawn> killAction)
    {
        _killAction = killAction;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform == _player)
        {
            HitPlayer?.Invoke();
        }

        if (collision.transform == _ground)
        {
            _killAction(this);
            AudioHandler.Instance.PlayOneShotSound("Enemies", _soundClip, transform.position, .5f, 0, 80);
        }
    }
}
