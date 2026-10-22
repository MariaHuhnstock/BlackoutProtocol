using UnityEngine;

[CreateAssetMenu(fileName = "NewRepairStation", menuName = "Blackout Protocol/Repair Data")]
public class RepairData : StructureData
{
    public float range = 6f;
    public float repairAmount = 1f;
    public float repairInterval = 0.5f;
    public float droneSpeed = 3f;
    public float hoverHeight = 0.6f;
}