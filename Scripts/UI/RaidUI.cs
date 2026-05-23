using UnityEngine;
using UnityEngine.UI;

public class RaidUI : MonoBehaviour
{
    public GameObject raidPanel;
    public Button pveRaidButton;
    public Button pvpRaidButton;
    public Text raidStatusText;
    public Text energyText;
    
    private PVERaidSystem pveRaid;
    private RaidSystem pvpRaid;
    private EnergySystem energy;
    
    void Start()
    {
        pveRaid = FindObjectOfType<PVERaidSystem>();
        pvpRaid = FindObjectOfType<RaidSystem>();
        energy = FindObjectOfType<EnergySystem>();
        
        if (pveRaidButton != null)
            pveRaidButton.onClick.AddListener(StartPVERaid);
        
        if (pvpRaidButton != null)
            pvpRaidButton.onClick.AddListener(StartPVPRaid);
    }
    
    void StartPVERaid()
    {
        if (pveRaid != null)
            pveRaid.StartRaid(PVERaidSystem.RaidType.ZombieHorde);
    }
    
    void StartPVPRaid()
    {
        if (pvpRaid != null)
            pvpRaid.StartRaid("enemy_player");
    }
    
    void Update()
    {
        if (energyText != null && energy != null)
            energyText.text = $"Energy: {energy.currentEnergy}/{energy.maxEnergy}";
    }
    
    public void ShowPanel() => raidPanel?.SetActive(true);
    public void HidePanel() => raidPanel?.SetActive(false);
}
