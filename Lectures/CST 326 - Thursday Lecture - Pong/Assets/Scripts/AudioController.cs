using System.Collections;
using UnityEngine;

public class AudioController : MonoBehaviour
{

    public AudioClip wallHit;
    public AudioClip paddleHit;
    public AudioClip goalHit;
    public AudioClip spawnPowerUp;
    public AudioClip collectPowerUp;
    public AudioClip playerWin;
    public AudioClip backgroundMusic;
    public AudioClip gameBegin;
    public float gameBeginVolume = 1f;

    public AudioSource audioSource;
    public AudioSource backgroundSource;

    private float baseBallSpeed = 20f;

    private float maxBallSpeed = 40f;

    // Range for the audio of the ball when colliding
    private float minPitch = 0.95f;
    private float maxPitch = 1.35f;

    // Instance is so that any script can call AudioController functions to play audio
    public static AudioController Instance
    {
        get; 
        private set;
    }
    
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
    }

    // Going to use another coroutine to handle the game starting until sfx is over
    private void Start()
    {
        StartCoroutine(IntroSequence());
    }

    private System.Collections.IEnumerator IntroSequence()
    {
        // Prevent the game from starting
        GameState.gameCanPlay(false);

        // Play gameBegin sfx
        if (gameBegin != null && audioSource != null)
        {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(gameBegin, gameBeginVolume);
            yield return new WaitForSeconds(gameBegin.length);
        }
        else
        {
            // In case there is no audio assigned
            // the game will wait eitherway
            // this is good to let me know if I
            // forgot to assign audio or not
            yield return new WaitForSeconds(0.5f);
        }

        // Start the game and music
        PlayBackgroundMusicLoop();
        GameState.gameCanPlay(true);
    }

    
    // The general idea is that as the ball gets fast
    // whatever sound it plays when colliding
    // will be higher pitched to let the player know
    // that the ball has become faster
    private float PitchFromSpeed(float speed)
    {
        if (maxBallSpeed <= baseBallSpeed)
        {
            return 1f;
        }

        float t = Mathf.InverseLerp(baseBallSpeed, maxBallSpeed, speed);
        return Mathf.Lerp(minPitch, maxPitch, t);
    }

    private void PlayOneShot(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip == null || audioSource == null) return;

        audioSource.pitch = pitch;
        audioSource.PlayOneShot(clip, volume);
    }

    public void PlayWallHit(float ballSpeed)
    {
        PlayOneShot(wallHit, 1f, PitchFromSpeed(ballSpeed));
    }

    public void PlayPaddleHit(float ballSpeed)
    {
        PlayOneShot(paddleHit, 1f, PitchFromSpeed(ballSpeed));
    }

    public void PlayGoalHit()
    {
        PlayOneShot(goalHit, 1f, 1f);
    }

    public void PlayPowerupSpawn()
    {
        PlayOneShot(spawnPowerUp, 1f, 1f);
    }

    public void PlayPowerUpCollect()
    {
        PlayOneShot(collectPowerUp, 1f, 1f);
    }

    public void PlayPlayerWin()
    {
        PlayOneShot(playerWin, 1f, 1f);
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

    public void FadeOutBackgroundMusic(float duration)
    {
        if (backgroundSource == null)
        {
            return;
        }

        StartCoroutine(FadeOutMusicRoutine(duration));
    }

    private IEnumerator FadeOutMusicRoutine(float duration)
    {
        float startVolume = backgroundSource.volume;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime; // works even if you pause gameplay
            float a = t / duration;
            backgroundSource.volume = Mathf.Lerp(startVolume, 0f, a);
            yield return null;
        }

        backgroundSource.volume = 0f;
        backgroundSource.Stop();
    }


}
