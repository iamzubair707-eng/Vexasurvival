using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SmoothUIManager : MonoBehaviour
{
    public static SmoothUIManager Instance;
    
    [Header("Animation Durations")]
    public float fadeDuration = 0.3f;
    public float slideDuration = 0.4f;
    public float scaleDuration = 0.2f;
    
    [Header("UI Elements")]
    public CanvasGroup mainPanel;
    public CanvasGroup loadingPanel;
    
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
    }
    
    public void FadeIn(CanvasGroup panel, System.Action onComplete = null)
    {
        StartCoroutine(FadeCoroutine(panel, 0, 1, fadeDuration, onComplete));
    }
    
    public void FadeOut(CanvasGroup panel, System.Action onComplete = null)
    {
        StartCoroutine(FadeCoroutine(panel, 1, 0, fadeDuration, onComplete));
    }
    
    IEnumerator FadeCoroutine(CanvasGroup panel, float start, float end, float duration, System.Action onComplete)
    {
        float elapsed = 0;
        panel.alpha = start;
        panel.gameObject.SetActive(true);
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            panel.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        
        panel.alpha = end;
        if (end == 0) panel.gameObject.SetActive(false);
        onComplete?.Invoke();
    }
    
    public void SlideIn(RectTransform panel, Vector2 startPos, Vector2 endPos, System.Action onComplete = null)
    {
        StartCoroutine(SlideCoroutine(panel, startPos, endPos, slideDuration, onComplete));
    }
    
    IEnumerator SlideCoroutine(RectTransform panel, Vector2 start, Vector2 end, float duration, System.Action onComplete)
    {
        float elapsed = 0;
        panel.anchoredPosition = start;
        panel.gameObject.SetActive(true);
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            panel.anchoredPosition = Vector2.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        
        panel.anchoredPosition = end;
        onComplete?.Invoke();
    }
    
    public void ScaleAnimation(Transform target, System.Action onComplete = null)
    {
        StartCoroutine(ScaleCoroutine(target, scaleDuration, onComplete));
    }
    
    IEnumerator ScaleCoroutine(Transform target, float duration, System.Action onComplete)
    {
        Vector3 originalScale = target.localScale;
        target.localScale = originalScale * 0.9f;
        yield return new WaitForSeconds(duration / 2);
        target.localScale = originalScale;
        onComplete?.Invoke();
    }
    
    public void ButtonClickFeedback(Button button)
    {
        StartCoroutine(ButtonFeedbackCoroutine(button));
    }
    
    IEnumerator ButtonFeedbackCoroutine(Button button)
    {
        var colors = button.colors;
        colors.normalColor = colors.pressedColor;
        button.colors = colors;
        yield return new WaitForSeconds(0.1f);
        colors.normalColor = colors.normalColor;
        button.colors = colors;
    }
    
    public void ShowFloatingReward(string text, Color color, Vector3 position)
    {
        StartCoroutine(FloatingRewardCoroutine(text, color, position));
    }
    
    IEnumerator FloatingRewardCoroutine(string text, Color color, Vector3 position)
    {
        GameObject rewardObj = new GameObject("FloatingReward");
        rewardObj.transform.position = position;
        
        TextMesh tm = rewardObj.AddComponent<TextMesh>();
        tm.text = text;
        tm.color = color;
        tm.fontSize = 32;
        tm.alignment = TextAlignment.Center;
        
        float duration = 1f;
        float elapsed = 0;
        Vector3 startPos = position;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rewardObj.transform.position = startPos + new Vector3(0, elapsed * 50, 0);
            tm.color = new Color(color.r, color.g, color.b, 1 - (elapsed / duration));
            yield return null;
        }
        
        Destroy(rewardObj);
    }
    
    public void ShowLoading(bool show, string message = "Loading...")
    {
        if (loadingPanel != null)
        {
            if (show)
            {
                loadingPanel.GetComponentInChildren<Text>()?.SetText(message);
                FadeIn(loadingPanel);
            }
            else
            {
                FadeOut(loadingPanel);
            }
        }
    }
}
