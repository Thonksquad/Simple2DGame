using AudioSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class RedSwitch : MonoBehaviour
{
    public event Action RedPressed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController _player))
        {
            RedPressed?.Invoke();
            SpawnHandler.Instance.StartGame();
        }
    }

}
