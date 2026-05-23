using UnityEngine;
using System.Collections;

public class TutorialSystem : MonoBehaviour
{
    private int _step = 0;
    private bool _isActive = false;
    private UIManager _ui;
    
    private readonly string[] _messages = {
        "👋 Welcome to VEXA SURVIVAL!\nTap GATHER to collect resources.",
        "🏗️ Tap UPGRADE to improve your building.\nHigher level = more resources!",
        "⚔️ Tap TRAIN to recruit soldiers.\nYou need troops for raids!",
        "🎯 Tap RAID to attack enemies.\nWin = resources + XP!",
        "🎁 Tap CHEST for free rewards every 3 hours!",
        "🎉 Tutorial complete! +500 coins, +50 gems!"
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
        if (_step < _messages.Length)
        {
            _ui?.ShowNotification(_messages[_step], Color.yellow, 4f);
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
        
        if (_step < _requiredActions.Length && action == _requiredActions[_step])
        {
            _step++;
            ShowStep();
        }
    }
    
    void CompleteTutorial()
    {
        _isActive = false;
        PlayerPrefs.SetInt("TutorialComplete", 1);
        
        var currency = MasterGameManager.Instance?.Currency;
        currency?.AddCoins(500);
        currency?.AddGems(50);
        
        _ui?.ShowNotification("🎉 TUTORIAL COMPLETE! +500 coins, +50 gems!", Color.green, 4f);
    }
    
    public void StartTutorial() => StartCoroutine(StartTutorial());
    public bool IsTutorialComplete() => PlayerPrefs.GetInt("TutorialComplete", 0) == 1;
}
