using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject gamePanel;
    public GameObject shopPanel;
    public GameObject clanPanel;
    
    [Header("Effects")]
    public Image fadePanel;
    public float fadeDuration = 0.5f;
    
    void Start()
    {
        // Start with fade in
        StartCoroutine(FadeIn());
    }
    
    public void ShowPanel(GameObject panel)
    {
        // Disable all panels first
        mainMenuPanel.SetActive(false);
        gamePanel.SetActive(false);
        shopPanel.SetActive(false);
        clanPanel.SetActive(false);
        
        // Enable requested panel with scale animation
        panel.SetActive(true);
        StartCoroutine(ScaleAnimation(panel));
    }
    
    public void ButtonClickEffect(Button button)
    {
        StartCoroutine(ButtonAnimation(button));
    }
    
    public void ShowFloatingText(string text, Vector3 position, Color color)
    {
        // Create floating text object
        GameObject textObj = new GameObject("FloatingText");
        textObj.transform.position = position;
        
        Text txt = textObj.AddComponent<Text>();
        txt.text = text;
        txt.color = color;
        txt.fontSize = 24;
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        
        // Animate and destroy
        StartCoroutine(FloatAndFade(textObj));
    }
    
    IEnumerator FadeIn()
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }
    
    IEnumerator ScaleAnimation(GameObject obj)
    {
        obj.transform.localScale = Vector3.zero;
        float t = 0;
        while (t < 0.3f)
        {
            t += Time.deltaTime;
            float scale = Mathf.Lerp(0, 1, t / 0.3f);
            obj.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
    }
    
    IEnumerator ButtonAnimation(Button button)
    {
        Vector3 originalScale = button.transform.localScale;
        button.transform.localScale = originalScale * 0.95f;
        yield return new WaitForSeconds(0.1f);
        button.transform.localScale = originalScale;
    }
    
    IEnumerator FloatAndFade(GameObject obj)
    {
        Text txt = obj.GetComponent<Text>();
        float t = 0;
        Vector3 startPos = obj.transform.position;
        
        while (t < 1f)
        {
            t += Time.deltaTime;
            obj.transform.position = startPos + new Vector3(0, t * 50, 0);
            txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, 1 - t);
            yield return null;
        }
        
        Destroy(obj);
    }
}