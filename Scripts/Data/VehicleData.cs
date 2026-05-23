using UnityEngine;

[CreateAssetMenu(fileName = "New Vehicle", menuName = "Vexa/Vehicle")]
public class VehicleData : ScriptableObject
{
    public string vehicleName;
    public int scrapCost;
    public int gemCost;
    public float speed;
    public float fuelCapacity;
    public float attackBonus;
    public float defenseBonus;
    public Sprite vehicleSprite;
}
