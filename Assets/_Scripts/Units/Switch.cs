using System;
using TarodevController;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [Tooltip ("Reference to which switch")]
    [SerializeField] private Sprite _switch;
    [Tooltip ("Sprite when pressed")]
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
