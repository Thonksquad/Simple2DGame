using System;
using System.Collections;
using TarodevController;
using UnityEngine;
using UnityEngine.UIElements;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private UIDocument _gameUI;
    [SerializeField] private StyleSheet _styleSheet;
    [SerializeField] private RedSwitch _redSwitch;
    [SerializeField] private BlueSwitch _blueSwitch;
    [SerializeField] private PlayerController _player;
    private VisualElement _root;
    private Slider _slider;
    private VisualElement _draggable;
    VisualElement _game;
    private Coroutine _gameTracker;
    private float _gameTime;
    private bool _isAlive;

    private void OnEnable()
    {
        _blueSwitch.BluePressed += BlueEvents;
        _redSwitch.RedPressed += RedEvents;
        _player.TakeDamage += SwapToDead;
        _player.Revive += SwapToAlive;
    }

    private void OnDisable()
    {
        _blueSwitch.BluePressed -= BlueEvents;
        _redSwitch.RedPressed -= RedEvents;
        _player.TakeDamage -= SwapToDead;
        _player.Revive -= SwapToAlive;
    }

    private void Awake()
    {
        _root = _gameUI.rootVisualElement;
        _slider = _root.Q<Slider>("Progression");
        _slider.SetEnabled(false);
        _draggable = _root.Q<VisualElement>("unity-dragger");
        _draggable.AddToClassList("spriteAlive");
        _game = _root.Q<VisualElement>("gameContainer");

    }

    private void SwapToAlive()
    {
        _isAlive = true;
        _draggable.RemoveFromClassList("spriteDead");
        _draggable.AddToClassList("spriteAlive");
    }

    private void SwapToDead()
    {
        _isAlive = false;
        _draggable.RemoveFromClassList("spriteAlive");
        _draggable.AddToClassList("spriteDead");
    }

    private void BlueEvents()
    {
        _blueSwitch.gameObject.SetActive(false);
        _redSwitch.gameObject.SetActive(true);
        VisualElement message1 = _root.Q<TextElement>("Message1");
        message1.AddToClassList("hidden");
        VisualElement message2 = _root.Q<TextElement>("Message2");
        message2.RemoveFromClassList("hidden");
    }

    private void RedEvents()
    {
        _isAlive = true;
        _redSwitch.gameObject.SetActive(false);
        VisualElement tutorial = _root.Q<VisualElement>("tutorialContainer");
        tutorial.AddToClassList("hidden");

        _game.RemoveFromClassList("hidden");
        _gameTracker = StartCoroutine(StartGameTimer());
    }

    IEnumerator StartGameTimer()
    {
        _slider.value = 0.1f;

        while (_slider.value > 0 && _slider.value < 100)
        {
            _gameTime += Time.deltaTime;
            var rate = _isAlive ? 1.25f : -2f;
            _slider.value += rate * Time.deltaTime;

            yield return null;
        }

        EvaluateVictory();
    }

    private void EvaluateVictory()
    {
        VisualElement endingScreen = _root.Q<VisualElement>("EndingScreen");
        Label announcement = _root.Q<Label>("Announcement");
        VisualElement graphic = _root.Q<VisualElement>("Graphic");
        Label time = _root.Q<Label>("Time");

        _game.AddToClassList("hidden");
        endingScreen.RemoveFromClassList("hidden");

        if (_slider.value <= 0)
        {
            announcement.text = "VICTORY";
            graphic.AddToClassList("spriteDead");
        }

        if (_slider.value >= 100)
        {
            announcement.text = "DEFEAT";
            graphic.AddToClassList("spriteAlive");
        }

        time.text = "FINAL TIME: " + Math.Round(_gameTime,2);
    }

}
