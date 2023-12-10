using System;
using TarodevController;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] private Sprite _switch;
    [SerializeField] private Sprite _pressed;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
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
