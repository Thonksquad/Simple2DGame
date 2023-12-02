using AudioSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundStorage : MonoBehaviour
{
    [Header("Background music")]
    [Tooltip("Currently BGM")]
    [SerializeField] AudioSource _bgmSource;
    [SerializeField] AudioClip _backgroundMusic;

    // basic range of UI sound clips
    [Header("UI Sounds")]
    [Tooltip("General button click.")]
    [SerializeField] AudioClip _defaultButtonSound;
    [Tooltip("General button click.")]
    [SerializeField] AudioClip _altButtonSound;
    [Tooltip("General shop purchase clip.")]
    [SerializeField] AudioClip _transactionSound;
    [Tooltip("General error sound.")]
    [SerializeField] AudioClip _defaultWarningSound;

    [Header("Game Sounds")]
    [Tooltip("Level up or level win sound.")]
    [SerializeField] AudioClip _victorySound;
    [Tooltip("Level defeat sound.")]
    [SerializeField] AudioClip _defeatSound;

    private Spawn _spawn;

    public void PlayMainMenuSound()
    {
        AudioHandler.Instance.PlayOneShotSound("UI", _defaultButtonSound, transform.position, .5f, 0f, 127);
    }

    public void PlaySubmenuSound()
    {
        AudioHandler.Instance.PlayOneShotSound("UI", _altButtonSound, transform.position, .5f, 0f, 80);
    }

    public void PlayTransactionSound()
    {
        AudioHandler.Instance.PlayOneShotSound("UI", _transactionSound, transform.position, .5f, 0f, 110);
    }

    public void PlayErrorSound()
    {
        AudioHandler.Instance.PlayOneShotSound("UI", _defaultWarningSound, transform.position, .5f, 0f, 127);
    }

    public void PlayVictorySound()
    {
        AudioHandler.Instance.PlayOneShotSound("Scene", _victorySound, transform.position, .5f, 0f, 127);
    }

    public void PlayDefeatSound()
    {
        AudioHandler.Instance.PlayOneShotSound("Scene", _defeatSound, transform.position, .5f, 0f, 127);
    }

    public void PlayMusic()
    {
        _bgmSource.clip = _backgroundMusic;
        _bgmSource.Play();
    }

    private void Start()
    {
        PlayMusic();
    }

    private void OnEnable()
    {
        Spawn.HitPlayer += HandleDefeat;
    }

    private void OnDisable()
    {
        Spawn.HitPlayer -= HandleDefeat;
    }

    private void HandleDefeat()
    {
        AudioHandler.Instance.SetTrackVolume("Music", -100, 2);
        PlayDefeatSound();
    }

}
