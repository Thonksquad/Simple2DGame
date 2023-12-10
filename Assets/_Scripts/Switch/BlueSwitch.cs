using AudioSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class BlueSwitch : MonoBehaviour
{
    public event Action BluePressed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController _player))
        {
            BluePressed?.Invoke();
        }
    }

}
