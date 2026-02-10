using UnityEngine;

public class LiveDemoAudioManager : MonoBehaviour
{
    public AudioClip carEngine;
    public AudioClip carHorn;
    public AudioClip backgroundMusic;

    public AudioSource carEngineSource;
    public AudioSource carHornSource;
    public AudioSource backgroundSource;

    public void PlayCarEngineLoop()
    {
        carEngineSource.clip = carEngine;
        carEngineSource.loop = true;
        carEngineSource.Play();
    }
    
    public void PlayCarHorn()
    {
        carHornSource.PlayOneShot(carHorn);
    }
}
