using AudioSystem;
using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class SwitchVisual : MonoBehaviour
{
    [SerializeField] private Sprite _switch;
    [SerializeField] private Sprite _pressed;

    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponentInParent<SpriteRenderer>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController _player))
        {
            _spriteRenderer.sprite = _pressed;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController _player))
        {
            _spriteRenderer.sprite = _switch;
        }
    }

}
