using AudioSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] AudioClip _soundClip;
    private Action<Enemy> _killAction;
    private Transform _ground;

    private void Awake()
    {
        _ground = GameObject.FindWithTag("ObstacleCollector").transform;
    }

    public void Init(Action<Enemy> killAction)
    {
        _killAction = killAction;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform == _ground)
        {
            _killAction(this);
            AudioHandler.Instance.PlayOneShotSound("Enemies", _soundClip, transform.position, .5f, 0, 80);
        }
    }
}
