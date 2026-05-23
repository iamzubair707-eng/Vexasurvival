using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Vexa/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string description;
    public int coinValue;
    public int gemValue;
    public ItemRarity rarity;
    public Sprite itemIcon;
    
    public enum ItemRarity
    {
        Common,
        Rare,
        Epic,
        Legendary
    }
}
