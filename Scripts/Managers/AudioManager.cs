using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("Sound Effects - Assign free assets")]
    public AudioClip buttonClick;
    public AudioClip coinCollect;
    public AudioClip levelUp;
    public AudioClip raidStart;
    public AudioClip buildComplete;
    public AudioClip damageTaken;
    public AudioClip notification;
    
    [Header("Background Music")]
    public AudioClip menuMusic;
    public AudioClip gameplayMusic;
    public AudioClip raidMusic;
    
    private AudioSource sfxSource;
    private AudioSource musicSource;
    private AudioSource raidMusicSource;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeAudio()
    {
        sfxSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        raidMusicSource = gameObject.AddComponent<AudioSource>();
        
        musicSource.loop = true;
        raidMusicSource.loop = true;
        
        PlayMusic(gameplayMusic);
    }
    
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip, volume);
    }
    
    public void PlayMusic(AudioClip clip)
    {
        if (clip != null)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }
    
    public void PlayRaidMusic()
    {
        if (raidMusic != null)
        {
            musicSource.Pause();
            raidMusicSource.clip = raidMusic;
            raidMusicSource.Play();
        }
    }
    
    public void StopRaidMusic()
    {
        raidMusicSource.Stop();
        musicSource.UnPause();
    }
    
    // Convenience methods
    public void ButtonClick() => PlaySFX(buttonClick, 0.7f);
    public void CoinCollect() => PlaySFX(coinCollect, 0.8f);
    public void LevelUp() => PlaySFX(levelUp, 1f);
    public void RaidStart() => PlaySFX(raidStart, 0.9f);
    public void BuildComplete() => PlaySFX(buildComplete, 0.8f);
    public void DamageTaken() => PlaySFX(damageTaken, 0.8f);
    public void Notification() => PlaySFX(notification, 0.6f);
}