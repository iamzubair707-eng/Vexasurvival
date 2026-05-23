using UnityEngine;
using System.Collections;

public class VisualManager : MonoBehaviour
{
    public static VisualManager Instance;
    
    // Particle effects (assign free assets in Unity)
    public ParticleSystem coinEffect;
    public ParticleSystem levelUpEffect;
    public ParticleSystem damageEffect;
    public ParticleSystem buildEffect;
    
    // Screen effects
    public GameObject damageFlashPanel;
    public GameObject levelUpFlashPanel;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    
    public void PlayCoinEffect(Vector3 position)
    {
        if (coinEffect != null)
        {
            var effect = Instantiate(coinEffect, position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, 2f);
        }
        StartCoroutine(ScreenFlash(coinEffect != null));
    }
    
    public void PlayLevelUpEffect(Vector3 position)
    {
        if (levelUpEffect != null)
        {
            var effect = Instantiate(levelUpEffect, position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, 2f);
        }
        StartCoroutine(ScreenFlash(true, "levelup"));
    }
    
    public void PlayDamageEffect(Vector3 position)
    {
        if (damageEffect != null)
        {
            var effect = Instantiate(damageEffect, position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, 1f);
        }
        StartCoroutine(ScreenFlash(false, "damage"));
    }
    
    IEnumerator ScreenFlash(bool isPositive, string type = "")
    {
        GameObject flashPanel = isPositive ? levelUpFlashPanel : damageFlashPanel;
        if (flashPanel == null) yield break;
        
        flashPanel.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        flashPanel.SetActive(false);
    }
    
    public void AnimateUIButton(Transform button)
    {
        StartCoroutine(ButtonAnimation(button));
    }
    
    IEnumerator ButtonAnimation(Transform button)
    {
        Vector3 originalScale = button.localScale;
        button.localScale = originalScale * 0.9f;
        yield return new WaitForSeconds(0.1f);
        button.localScale = originalScale;
    }
    
    public void FloatingText(string text, Vector3 position, Color color)
    {
        // Simple floating text using Unity's UI system
        GameObject textObj = new GameObject("FloatingText");
        textObj.transform.position = position;
        TextMesh tm = textObj.AddComponent<TextMesh>();
        tm.text = text;
        tm.color = color;
        tm.fontSize = 24;
        StartCoroutine(FloatAndFade(textObj));
    }
    
    IEnumerator FloatAndFade(GameObject obj)
    {
        float duration = 1f;
        float elapsed = 0f;
        Vector3 startPos = obj.transform.position;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            obj.transform.position = startPos + new Vector3(0, elapsed * 2f, 0);
            yield return null;
        }
        
        Destroy(obj);
    }
}