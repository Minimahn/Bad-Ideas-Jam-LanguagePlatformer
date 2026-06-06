using System.Data.Common;
using UnityEngine;
using UnityEngine.Rendering;

public class BackgroundMusic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AudioClip beatOne;
    public AudioClip beatTwo;
    private AudioSource audioSource;
    private AudioClip nextBeat;
    private bool current = true;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = beatOne;
        audioSource.Play();
        nextBeat = beatTwo;
    }

    void LateUpdate()
    {
        if (audioSource.isPlaying == false)
        {
            audioSource.clip = nextBeat;
            audioSource.Play();
            if (current)
            {
               nextBeat = beatOne; 
               current = false;
            }
            else
            {
                nextBeat = beatTwo;
                current = true;
            }
        }
    }
}
