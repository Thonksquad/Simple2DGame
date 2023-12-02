using AudioSystem;
using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class SwitchSound : MonoBehaviour
{
    [Header("UI Sounds")]
    [Tooltip("General button click.")]
    [SerializeField] AudioClip _defaultButtonSound;
    [Tooltip("General button click.")]
    [SerializeField] AudioClip _altButtonSound;


    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController _player))
        {
            AudioHandler.Instance.PlayOneShotSound("UI", _defaultButtonSound, transform.position, .5f, 0, 120);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController _player))
        {
            AudioHandler.Instance.PlayOneShotSound("UI", _altButtonSound, this.transform.position, 1, 0, 120);
        }
    }

}
