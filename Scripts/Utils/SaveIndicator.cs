using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SaveIndicator : MonoBehaviour
{
    public GameObject savingIcon;
    public float showDuration = 0.5f;
    
    void Start()
    {
        if (savingIcon != null)
            savingIcon.SetActive(false);
    }
    
    public void ShowSaving()
    {
        StartCoroutine(ShowSavingRoutine());
    }
    
    IEnumerator ShowSavingRoutine()
    {
        if (savingIcon != null)
        {
            savingIcon.SetActive(true);
            yield return new WaitForSeconds(showDuration);
            savingIcon.SetActive(false);
        }
    }
}