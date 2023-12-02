using AudioSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class StartSwitch : MonoBehaviour
{
    public event Action StartGame;
    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController _player))
        {
            StartGame?.Invoke();
            SpawnHandler.Instance.StartGame();
        }
    }

}
