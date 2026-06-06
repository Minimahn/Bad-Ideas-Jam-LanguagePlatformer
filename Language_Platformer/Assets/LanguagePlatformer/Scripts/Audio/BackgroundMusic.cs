using UnityEngine;

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
        // update this later to have beats as an array and to randomly select based on the size of it so we don't need to change this each time
        audioSource = GetComponent<AudioSource>();
        int random = Random.Range(0, 2);
        if (random == 0)
        {
            audioSource.clip = beatOne;
            nextBeat = beatTwo;
        }
        else
        {
            audioSource.clip = beatTwo;
            nextBeat = beatOne;
        }
        audioSource.Play();
    }

    private void OnDestroy()
    {
        audioSource.Stop();
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
