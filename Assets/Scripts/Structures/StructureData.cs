using UnityEngine;

[CreateAssetMenu(fileName = "NewStructure", menuName = "Blackout Protocol/Structure Data")]
public class StructureData : ScriptableObject
{
    public string displayName = "Structure";
    public int maxHealth = 10;
    public int powerCost = 1;
    public bool startsPowered = false;
}