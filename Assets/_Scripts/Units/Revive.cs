using AudioSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class Revive : MonoBehaviour
{
    [Tooltip("Sound to play when missed")]
    [SerializeField] AudioClip _missedClip;
    private Transform _player;
    private Action<Revive> _killAction;
    private Transform _ground;

    private void Awake()
    {
        _ground = GameObject.FindWithTag("ObstacleCollector").transform;
        _player = GameObject.FindWithTag("Player").transform;
    }

    public void Init(Action<Revive> killAction)
    {
        _killAction = killAction;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform == _ground)
        {
            _killAction(this);
            AudioHandler.Instance.PlayOneShotSound("Enemies", _missedClip, transform.position, .5f, 0, 80);
        }

        if (collision.transform == _player)
        {
            _killAction(this);
        }

    }
}
