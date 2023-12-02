using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioSystem;

public class AudioManagerTest : MonoBehaviour
{
    public AudioClip clip;

    // Start is called before the first frame update
    void Start()
    {
        if (AudioHandler.Instance)
            AudioHandler.Instance.SetTrackVolume("Player", 10, 5);

        for (int i = 0; i < 10; i++)
        {
            Invoke(nameof(PlayTest), 1);
        }
    }

    void PlayTest()
    {
        AudioHandler.Instance.PlayOneShotSound("Player", clip, transform.position, .5f, 0f, 128);
    }

}
