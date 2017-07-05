using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip[] music;
    AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(!source.isPlaying)
        {
            source.clip = music[Random.Range(0, music.Length)];
            source.Play();
        }
    }
}
