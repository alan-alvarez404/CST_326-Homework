using System.Collections;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [Header("Sound Effects")] public AudioClip marioJump;
    public AudioClip bumpCoinBlock;
    public AudioClip breakBrickBlock;
    public AudioClip marioTouchesTheFlag;

    [Header("Music Effects")] public AudioClip backgroundMusic;
    public AudioClip defeatMusic;
    public AudioClip victoryMusic;

    [Header("Volume")] 
    public float volume = 0.3f;
    public float musicVolume = 0.3f;

    [Header("Sources")] public AudioSource audioSource;
    public AudioSource backgroundSource;

    // Instance is so that any script can call AudioController functions to play audio
    public static AudioController Instance { get; private set; }

    private void Start()
    {
        PlayBackgroundMusicLoop();
    }

    // Doing this somewhat similarly to how I did in pong
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (backgroundSource == null)
        {
            AudioSource[] sources = GetComponents<AudioSource>();
            if (sources != null && sources.Length > 1)
            {
                backgroundSource = sources[1];
            }
        }

        // These should override inspector settings
        // Clamping both volumes to the values passed in
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.volume = Mathf.Clamp01(volume);
        }

        if (backgroundSource != null)
        {
            backgroundSource.playOnAwake = false;
            backgroundSource.loop = true;
            backgroundSource.volume = Mathf.Clamp01(musicVolume);
        }
        
        // Just to make sure that there is no spatial audio effect in the inspector
        // Whatever settings that are set to 3D, should be overwritten by these two lines
        if (audioSource != null) audioSource.spatialBlend = 0f;
        if (backgroundSource != null) backgroundSource.spatialBlend = 0f;
    }


    private void PlayOneShot(AudioClip clip, float volume = 1f)
    {
        if (clip == null || audioSource == null) return;

        audioSource.PlayOneShot(clip, volume);
    }

    public void PlayMarioJump()
    {
        PlayOneShot(marioJump);
    }

    public void PlayBumpCoinBlock()
    {
        PlayOneShot(bumpCoinBlock);
    }

    public void PlayBreakBrickBlock()
    {
        PlayOneShot(breakBrickBlock);
    }

    public void PlayMarioTouchesTheFlag()
    {
        StartCoroutine(MarioTouchesTheFlagCoroutine());
    }

    private IEnumerator MarioTouchesTheFlagCoroutine()
    {
        StopBackgroundMusicLoop();
        
        audioSource.PlayOneShot(marioTouchesTheFlag);
        yield return new WaitForSecondsRealtime(marioTouchesTheFlag.length); // Wait until this sound is over
        
        backgroundSource.clip = victoryMusic;
        backgroundSource.loop = false;
        backgroundSource.volume = Mathf.Clamp01(musicVolume);
        backgroundSource.Play();
    }
    
    public void PlayLoseMusic()
    {
        PlayOneShot(defeatMusic);
    }

    public void PlayWinMusic()
    {
        PlayOneShot(victoryMusic);
    }

    public void PlayBackgroundMusicLoop()
    {
        if (backgroundSource == null || backgroundMusic == null)
        {
            return; // No backgroundSource or backgroundMusic assigned
        }

        backgroundSource.clip = backgroundMusic;
        backgroundSource.loop = true;
        backgroundSource.Play();
    }

    public void StopBackgroundMusicLoop()
    {
        if (backgroundSource != null) backgroundSource.Stop();
    }


}
