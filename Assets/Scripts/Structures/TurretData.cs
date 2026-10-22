using UnityEngine;

[CreateAssetMenu(fileName = "NewTurret", menuName = "Blackout Protocol/Turret Data")]
public class TurretData : StructureData
{
    public float range = 5f;
    public float fireInterval = 0.5f;
    public float damage = 1f;
}