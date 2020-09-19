using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMusic : MonoBehaviour
{
    private AudioSource _audioSource;
    private static MainMusic instance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }

        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayMusic()
    {
        if (_audioSource.isPlaying) return;
        _audioSource.Play();
    }

    public void StopMusic()
    {
        _audioSource.Stop();
    }
}