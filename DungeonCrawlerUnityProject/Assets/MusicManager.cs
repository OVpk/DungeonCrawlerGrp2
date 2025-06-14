using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    
    public AudioSource audioSource;
    
    private Coroutine musicRoutine;
    private float previousVolume = 1f;

    public AudioClip fightIntro;
    public AudioClip fightLoop;

    public AudioClip shopIntro;
    public AudioClip shopLoop;
    
    public enum MusicMode
    {
        Nothing,
        InExplo,
        InFight
    }

    public MusicMode currentMusicMode = MusicMode.Nothing;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void PlayFightMusic()
    {
        if (currentMusicMode == MusicMode.InFight) return;
        PlayMusic(fightIntro, fightLoop);
        currentMusicMode = MusicMode.InFight;
    }
    
    public void PlayShopMusic()
    {
        if (currentMusicMode == MusicMode.InExplo) return;
        PlayMusic(shopIntro, shopLoop);
        currentMusicMode = MusicMode.InExplo;
    }

    public void ToggleMute()
    {
        if (Mathf.Approximately(audioSource.volume, 0f))
        {
            audioSource.volume = previousVolume;
        }
        else
        {
            previousVolume = audioSource.volume;
            audioSource.volume = 0f;
        }
    }

    private void PlayMusic(AudioClip intro, AudioClip loop)
    {
        if (musicRoutine != null)
        {
            StopCoroutine(musicRoutine);
        }

        audioSource.Stop();

        musicRoutine = StartCoroutine(PlayIntroThenLoop(intro, loop));
    }
    
    private IEnumerator PlayIntroThenLoop(AudioClip intro, AudioClip loop)
    {
        audioSource.clip = intro;
        audioSource.loop = false;
        audioSource.Play();
        
        yield return new WaitForSeconds(intro.length);

        audioSource.clip = loop;
        audioSource.loop = true;
        audioSource.Play();
    }
}