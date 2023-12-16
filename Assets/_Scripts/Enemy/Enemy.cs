using AudioSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] AudioClip _soundClip;
    private PlayerController _player;
    private Action<Enemy> _killAction;
    public static Action NearMiss;
    private Transform _ground;

    private void Awake()
    {
        _ground = GameObject.FindWithTag("ObstacleCollector").transform;
        _player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
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

        if (collision.transform == _player.transform)
        {
            _killAction(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform == _player.transform)
        {
            if (!_player.isAlive) return;
            NearMiss?.Invoke();
        }
    }
}
