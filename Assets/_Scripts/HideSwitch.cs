using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideSwitch : MonoBehaviour
{
    [SerializeField] StartSwitch _startSwitch;

    private void OnEnable()
    {
        _startSwitch.StartGame += TurnOffSwitch;
    }

    private void OnDisable()
    {
        _startSwitch.StartGame -= TurnOffSwitch;
    }

    private void TurnOffSwitch()
    {
        this.gameObject.SetActive(false);
    }
}
