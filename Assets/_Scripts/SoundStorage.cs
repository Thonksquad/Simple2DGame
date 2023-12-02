using AudioSystem;
using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class SoundStorage : MonoBehaviour
{
    [Header("Background music")]
    [Tooltip("Currently BGM")]
    [SerializeField] AudioSource _bgmSource;
    [SerializeField] AudioClip _aliveMusic;
    [SerializeField] AudioClip _deadMusic;

    // basic range of UI sound clips
    [Tooltip("General shop purchase clip.")]
    [SerializeField] AudioClip _transactionSound;
    [Tooltip("General error sound.")]
    [SerializeField] AudioClip _defaultWarningSound;

    [Header("Game Sounds")]
    [Tooltip("Level up or level win sound.")]
    [SerializeField] AudioClip _victorySound;
    [Tooltip("Level defeat sound.")]
    [SerializeField] AudioClip _defeatSound;

    private PlayerController _player;

    private void Awake()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    private void Start()
    {
        PlayAliveMusic();
    }

    private void OnEnable()
    {
        _player.TakeDamage += HandleDefeat;
        _player.Revive += HandleRevive;
    }

    private void OnDisable()
    {
        _player.TakeDamage -= HandleDefeat;
        _player.Revive -= HandleRevive;
    }

    private void HandleDefeat()
    {
        AudioHandler.Instance.SetTrackVolume("Music", -22, 1);
        PlayDefeatSound();
        Invoke(nameof(PlayDeadMusic), 3.5f);
    }
    private void HandleRevive()
    {
        AudioHandler.Instance.SetTrackVolume("Music", -15, 2);
        PlayVictorySound();
        Invoke(nameof(PlayAliveMusic), 1);
    }

    public void PlayTransactionSound()
    {
        AudioHandler.Instance.PlayOneShotSound("UI", _transactionSound, transform.position, .5f, 0f, 110);
    }

    public void PlayErrorSound()
    {
        AudioHandler.Instance.PlayOneShotSound("UI", _defaultWarningSound, transform.position, .5f, 0f, 120);
    }

    public void PlayVictorySound()
    {
        AudioHandler.Instance.PlayOneShotSound("Scene", _victorySound, transform.position, .5f, 0f, 127);
    }

    public void PlayDefeatSound()
    {
        AudioHandler.Instance.PlayOneShotSound("Scene", _defeatSound, transform.position, 1, 0f, 127);
    }

    public void PlayAliveMusic()
    {
        _bgmSource.clip = _aliveMusic;
        _bgmSource.Play();
    }

    private void PlayDeadMusic()
    {
        _bgmSource.clip = _deadMusic;
        _bgmSource.Play();
    }

}
