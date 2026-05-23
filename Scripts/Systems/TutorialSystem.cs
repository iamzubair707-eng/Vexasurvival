using UnityEngine;
using System.Collections;

public class TutorialSystem : MonoBehaviour
{
    private int _currentStep = 0;
    private bool _isActive = false;
    private UIManager _ui;
    
    private readonly string[] _messages = {
        "👋 Welcome to VEXA SURVIVAL!\n\nTap GATHER to collect wood and stone!",
        "🏗️ Tap UPGRADE to improve your building.\n\nHigher level = more resources!",
        "⚔️ Tap TRAIN to recruit soldiers.\n\nYou need troops for raids!",
        "🎯 Tap RAID to attack enemies.\n\nWin = resources + XP!",
        "🎁 Tap CHEST for free rewards every 3 hours!\n\nDon't miss it!",
        "🎉 TUTORIAL COMPLETE!\n\n+500 coins, +50 gems rewarded!"
    };
    
    private readonly string[] _requiredActions = {
        "gather", "upgrade", "train", "raid", "chest", "complete"
    };
    
    void Start()
    {
        _ui = MasterGameManager.Instance?.UIManager;
        
        if (!IsTutorialComplete())
        {
            StartCoroutine(StartTutorial());
        }
    }
    
    IEnumerator StartTutorial()
    {
        yield return new WaitForSeconds(0.5f);
        _isActive = true;
        ShowStep();
    }
    
    void ShowStep()
    {
        if (_currentStep < _messages.Length)
        {
            _ui?.ShowNotification(_messages[_currentStep], Color.yellow, 5f);
        }
        else
        {
            CompleteTutorial();
        }
    }
    
    public void CheckAction(string action)
    {
        if (!_isActive) return;
        if (IsTutorialComplete()) return;
        
        if (_currentStep < _requiredActions.Length && action == _requiredActions[_currentStep])
        {
            _currentStep++;
            ShowStep();
        }
    }
    
    void CompleteTutorial()
    {
        _isActive = false;
        PlayerPrefs.SetInt("TutorialComplete", 1);
        
        var gm = MasterGameManager.Instance;
        gm?.GatherResource("wood", 0); // Just to trigger, actual rewards in GM
        
        _ui?.ShowNotification("🎉 TUTORIAL COMPLETE! +500 coins, +50 gems!", Color.green, 4f);
    }
    
    public void StartTutorial() => StartCoroutine(StartTutorial());
    public bool IsTutorialComplete() => PlayerPrefs.GetInt("TutorialComplete", 0) == 1;
}
