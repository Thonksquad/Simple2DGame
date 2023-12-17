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
    [Tooltip("Theme for when character is alive")]
    [SerializeField] AudioClip _aliveMusic;
    [Tooltip("Theme for when character is dead")]
    [SerializeField] AudioClip _deadMusic;

    [Header("Player emitted Sounds")]
    [Tooltip("Near miss sound effect")]
    [SerializeField] AudioClip _nearMissSound;
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
        Enemy.NearMiss += PlayNearMiss;
    }

    private void OnDisable()
    {
        _player.TakeDamage -= HandleDefeat;
        _player.Revive -= HandleRevive;
        Enemy.NearMiss -= PlayNearMiss;
    }

    private void HandleDefeat()
    {
        PlayDefeatSound();
        Invoke(nameof(PlayDeadMusic), 2f);
    }
    private void HandleRevive()
    {
        PlayVictorySound();
        Invoke(nameof(PlayAliveMusic), 1);
    }

    public void PlayVictorySound()
    {
        AudioHandler.Instance.PlayOneShotSound("Scene", _victorySound, transform.position, .5f, 0f, 127);
    }

    public void PlayDefeatSound()
    {
        AudioHandler.Instance.SetTrackVolume("Player", -50, 0);
        AudioHandler.Instance.PlayOneShotSound("Scene", _defeatSound, transform.position, 1, 0f, 127);
        AudioHandler.Instance.SetTrackVolume("Player", -15, 1);
    }

    public void PlayNearMiss()
    {
        AudioHandler.Instance.PlayOneShotSound("Player", _nearMissSound, transform.position, 1, 0f, 120); ;
    }

    public void PlayAliveMusic()
    {
        AudioHandler.Instance.SetTrackVolume("Music", -15, 2);
        _bgmSource.clip = _aliveMusic;
        _bgmSource.Play();
    }

    private void PlayDeadMusic()
    {
        AudioHandler.Instance.SetTrackVolume("Music", -22, 1);
        _bgmSource.clip = _deadMusic;
        _bgmSource.Play();
    }

}
