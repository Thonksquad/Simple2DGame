using System;
using System.Collections;
using TarodevController;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameHandler : MonoBehaviour
{
    [Header("External references")]
    [SerializeField] private UIDocument _gameUI;
    [SerializeField] private StyleSheet _styleSheet;
    [SerializeField] private RedSwitch _redSwitch;
    [SerializeField] private BlueSwitch _blueSwitch;
    [SerializeField] private PlayerController _player;
    [SerializeField] private SoundStorage _soundStorage;

    [Header("Internal variables")]
    [SerializeField] private float _nearMissBonus;
    [SerializeField] private float _bonusDuration;
    [SerializeField] private float _timer;

    private VisualElement _root;
    private Label _timeCounter;
    private Slider _slider;
    private VisualElement _draggable;
    private Button _restartButton;
    private VisualElement _gameScreen;
    private VisualElement _endingScreen;
    private Label _announcement;
    private VisualElement _graphic;
    private Label _time;

    private IVisualElementScheduledItem _sliderSchedule;
    private Coroutine _gameTracker;
    private float _gameTime;
    private bool _isAlive;

    public event Action GameFinished;

    private void OnEnable()
    {
        _blueSwitch.BluePressed += BlueEvents;
        _redSwitch.RedPressed += RedEvents;
        _player.TakeDamage += SwapToDead;
        _player.Revive += SwapToAlive;
        Enemy.NearMiss += HandleNearMiss;
    }

    private void OnDisable()
    {
        _blueSwitch.BluePressed -= BlueEvents;
        _redSwitch.RedPressed -= RedEvents;
        _player.TakeDamage -= SwapToDead;
        _player.Revive -= SwapToAlive;
        Enemy.NearMiss -= HandleNearMiss;
    }

    private void Awake()
    {
        _root = _gameUI.rootVisualElement;
        _slider = _root.Q<Slider>("Progression");
        _slider.SetEnabled(false);
        _timeCounter = _root.Q<Label>("InGameTimer");
        _draggable = _root.Q<VisualElement>("unity-dragger");
        _draggable.AddToClassList("spriteAlive");
        _gameScreen = _root.Q<VisualElement>("gameContainer");
        _restartButton = _root.Q<Button>("Restart");
        _endingScreen = _root.Q<VisualElement>("EndingScreen");
        _announcement = _root.Q<Label>("Announcement");
        _graphic = _root.Q<VisualElement>("Graphic");
        _time = _root.Q<Label>("TimeCounter");
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

        _gameScreen.RemoveFromClassList("hidden");
        _gameTracker = StartCoroutine(StartGameTimer());
    }

    IEnumerator StartGameTimer()
    {
        _gameTime = 0;
        _isAlive = true;
        _slider.value = 0.1f;


        SwapToAlive();

        while (_slider.value > 0 && _slider.value < 100)
        {
            _timeCounter.text = "Time " + Math.Round(_gameTime);
            _gameTime += Time.deltaTime;

            var rate = _isAlive ? 2f : -MathF.Max((_slider.value/18), 1f);
            _slider.value += rate * Time.deltaTime;

            yield return null;
        }
        EvaluateVictory();
    }

    private void EvaluateVictory()
    {
        _gameScreen.AddToClassList("hidden");
        _endingScreen.RemoveFromClassList("hidden");
        GameFinished?.Invoke();

        if (_slider.value <= 0)
        {
            _announcement.text = "DEFEAT";
            _graphic.AddToClassList("spriteDead");
            _soundStorage.PlayDefeatSound();
        }

        if (_slider.value >= 100)
        {
            _announcement.text = "VICTORY";
            _graphic.AddToClassList("spriteAlive");
            _soundStorage.PlayVictorySound();
        }

        _time.text = "FINAL TIME: " + Math.Round(_gameTime,2);
        _restartButton.clicked += RestartGame; 
    }

    private void RestartGame()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    private void HandleNearMiss()
    {
        if (_isAlive)
        {
            StartCoroutine(NearMissBonus(_slider.value));
            if (_sliderSchedule != null) return;
            _slider.RegisterCallback<TransitionEndEvent>(OnStateTransitionEnd);
            _sliderSchedule = _slider.schedule.Execute(() => _slider.ToggleInClassList("dragger-bounce")).StartingIn(150);
        }
    }
    
    private void OnStateTransitionEnd(TransitionEndEvent evt)
    {
        _sliderSchedule = null;
        _slider.ToggleInClassList("dragger-bounce");
        _slider.UnregisterCallback<TransitionEndEvent>(OnStateTransitionEnd);
    }

    private IEnumerator NearMissBonus(float originalValue)
    {
        yield return new WaitForSeconds(.01f);
        if (!_isAlive) yield break;
        float startValue = originalValue;
        float endValue = _slider.value + _nearMissBonus;

        _timer = 0f;

        yield return new WaitForSeconds(.02f);

        while (_timer < _bonusDuration)
        {
            _timer += Time.unscaledDeltaTime;
            float t = _timer / _bonusDuration;
            _slider.value = Mathf.Lerp(startValue, endValue, t);
            yield return null;
        }
    }
}
