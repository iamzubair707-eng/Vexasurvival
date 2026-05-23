using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "Vexa/Resource")]
public class ResourceData : ScriptableObject
{
    public string resourceName;
    public int startingAmount;
    public int maxAmount;
    public int gatherAmount;
    public float gatherTime;
    public Color resourceColor;
}
