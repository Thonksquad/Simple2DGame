using AudioSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Tooltip("Sound to play when player dodges")]
    [SerializeField] AudioClip _soundClip;
    [Tooltip("Particle source")]
    [SerializeField] ParticleSystem _particleSystem;
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
            _particleSystem.Play();
            AudioHandler.Instance.PlayOneShotSound("Enemies", _soundClip, transform.position, .5f, 0, 80);
            Invoke(nameof(RemoveObject), .5f);
        }

        if (collision.transform == _player.transform)
        {
            RemoveObject();
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

    private void RemoveObject()
    {
        _killAction(this);
    }

}
