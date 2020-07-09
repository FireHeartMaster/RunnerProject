using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource backgroundMusic;
    [SerializeField] AudioSource inGameSounds;

    [SerializeField] AudioClip[] collectibleGrabbingSound;
    [SerializeField] AudioClip[] jumpSounds;
    [SerializeField] AudioClip losingSound;

    public static SoundManager soundManager;

    [SerializeField] AudioClip[] backgroundMusicClips;    

    float timeSinceLastBackgroundMusicStarted = 0f;
    float lastBackgroundMusicDuration;
    int currentBackgroundMusicIndex = 0;
    bool firstBackgroundMusic = true;

    bool canChangeBackgroundMusic = true;

    bool startMusic = false;

    private void Awake()
    {
        if(soundManager == null)
        {
            soundManager = this;
        }
        else
        {
            Destroy(this);
        }
    }


    [ContextMenu("Start Background Music")]
    public void StartBackgroundMusic()
    {
        startMusic = true;
        PlayNewBackgroundMusic();
    }

    [ContextMenu("Reset Sounds")]
    public void ResetSounds()
    {
        backgroundMusic.Stop();
        inGameSounds.Stop();

        timeSinceLastBackgroundMusicStarted = 0f;
        lastBackgroundMusicDuration = 0f;
        currentBackgroundMusicIndex = 0;
        firstBackgroundMusic = true;
        startMusic = false;

        if(increaseVolumeCoroutine != null)
        {
            StopCoroutine(increaseVolumeCoroutine);
        }

        if(decreaseVolumeCoroutine != null)
        {
            StopCoroutine(decreaseVolumeCoroutine);
        }

        inGameSounds.volume = 1f;

        canChangeBackgroundMusic = true;        
    }

    private void Update()
    {
        if (startMusic)
        {
            timeSinceLastBackgroundMusicStarted += Time.deltaTime;

            if (timeSinceLastBackgroundMusicStarted >= lastBackgroundMusicDuration && canChangeBackgroundMusic/* && canChangeBackgroundMusic*/)
            {
                timeSinceLastBackgroundMusicStarted = 0;
                PlayNewBackgroundMusic();
            }


            if (timeSinceLastBackgroundMusicStarted < (1f / increaseDecreaseSpeed) && !alreadyIncreasedVolume && canChangeBackgroundMusic)
            {
                if (increaseVolumeCoroutine != null)
                {
                    StopCoroutine(increaseVolumeCoroutine);
                }
                increaseVolumeCoroutine = BackgroundMusicIncreaseVolume();
                StartCoroutine(increaseVolumeCoroutine);
            }

            if ((lastBackgroundMusicDuration - timeSinceLastBackgroundMusicStarted) < (1f / increaseDecreaseSpeed) && !alreadyDecreasedVolume && canChangeBackgroundMusic)
            {
                if (decreaseVolumeCoroutine != null)
                {
                    StopCoroutine(decreaseVolumeCoroutine);
                }
                decreaseVolumeCoroutine = BackgroundMusicDecreaseVolume();
                StartCoroutine(decreaseVolumeCoroutine);
            }
        }
    }

    public void PlaySoundCollectible(int type = 0)
    {
        float pitch = Random.Range(0.85f, 1.1f);
        inGameSounds.pitch = pitch;
        inGameSounds.PlayOneShot(collectibleGrabbingSound[type]);
    }

    void PlayNewBackgroundMusic()
    {
        alreadyIncreasedVolume = false;
        alreadyDecreasedVolume = false;
        
        if(backgroundMusicClips.Length > 1)
        {
            if (!firstBackgroundMusic)
            {
                int trials = 0;
                int maxTrials = 4;
                int nextIndex;
                do
                {
                    nextIndex = Random.Range(0, backgroundMusicClips.Length - 1);
                    trials++;
                } while (trials < maxTrials && nextIndex == currentBackgroundMusicIndex);

                currentBackgroundMusicIndex = nextIndex;
            }
            else
            {
                firstBackgroundMusic = false;
                currentBackgroundMusicIndex = Random.Range(0, backgroundMusicClips.Length - 1);
            }
            
        }
        else
        {
            currentBackgroundMusicIndex = 0;
        }

        backgroundMusic.clip = backgroundMusicClips[currentBackgroundMusicIndex];
        backgroundMusic.Play();

        lastBackgroundMusicDuration = backgroundMusicClips[currentBackgroundMusicIndex].length;        
    }


    public void DeathHandleSounds()
    {
        backgroundMusic.Stop();
        backgroundMusic.volume = 1f;
        backgroundMusic.PlayOneShot(losingSound);
        inGameSounds.volume = 0f;

        canChangeBackgroundMusic = false;

        if (increaseVolumeCoroutine != null)
        {
            StopCoroutine(increaseVolumeCoroutine);
        }

        if (decreaseVolumeCoroutine != null)
        {
            StopCoroutine(decreaseVolumeCoroutine);
        }
    }

    public void JumpSound()
    {
        int index = Random.Range(0, jumpSounds.Length);
        float pitch = Random.Range(0.85f, 1.1f);
        inGameSounds.pitch = pitch;
        inGameSounds.PlayOneShot(jumpSounds[index]);
    }


    bool alreadyIncreasedVolume = false;
    bool alreadyDecreasedVolume = false;

    [SerializeField] float increaseDecreaseSpeed = 0.5f;

    IEnumerator increaseVolumeCoroutine;
    IEnumerator decreaseVolumeCoroutine;

    IEnumerator BackgroundMusicIncreaseVolume()
    {
        backgroundMusic.volume = 0f;
        alreadyIncreasedVolume = true;
        while (backgroundMusic.volume < 0.9f)
        {
            backgroundMusic.volume += increaseDecreaseSpeed * Time.deltaTime;

            yield return new WaitForSeconds(Time.deltaTime);
        }

        backgroundMusic.volume = 1f;
    }

    IEnumerator BackgroundMusicDecreaseVolume()
    {
        backgroundMusic.volume = 1f;
        alreadyDecreasedVolume = true;
        while (backgroundMusic.volume > 0.1f)
        {
            backgroundMusic.volume -= increaseDecreaseSpeed * Time.deltaTime;

            yield return new WaitForSeconds(Time.deltaTime);
        }

        backgroundMusic.volume = 0f;
    }

}
