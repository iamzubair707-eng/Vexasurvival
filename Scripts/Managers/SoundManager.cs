using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    
    public AudioClip buttonClick;
    public AudioClip coinCollect;
    public AudioClip levelUp;
    public AudioClip raidAlert;
    
    private AudioSource audioSource;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }
    
    public void PlayButtonClick()
    {
        if (buttonClick != null)
            audioSource.PlayOneShot(buttonClick);
    }
    
    public void PlayCoinCollect()
    {
        if (coinCollect != null)
            audioSource.PlayOneShot(coinCollect);
    }
    
    public void PlayLevelUp()
    {
        if (levelUp != null)
            audioSource.PlayOneShot(levelUp);
    }
    
    public void PlayRaidAlert()
    {
        if (raidAlert != null)
            audioSource.PlayOneShot(raidAlert);
    }
}