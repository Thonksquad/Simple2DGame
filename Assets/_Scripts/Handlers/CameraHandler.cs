using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private Vector3 _offset = new Vector3(0f, 6.6f, -10f);
    [SerializeField] private float _smoothTime = 0.3f;
    [SerializeField] private Vector3 _velocity = Vector3.zero;
    [SerializeField] private Transform _player;

    private void FixedUpdate()
    {
        Vector3 targetPosition = _player.position + _offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, _smoothTime);
    }
}
