using UnityEngine;

[CreateAssetMenu(fileName = "New Building", menuName = "Vexa/Building")]
public class BuildingData : ScriptableObject
{
    public string buildingName;
    public int woodCost;
    public int stoneCost;
    public int coinCost;
    public int buildTime;
    public int maxLevel;
    public float productionBonus;
    public Sprite buildingIcon;
}
