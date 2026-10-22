using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Blackout Protocol/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string displayName = "Enemy";
    public float maxHealth = 3f;
    public float moveSpeed = 2f;
    public float attackDamage = 1f;
    public float attackInterval = 1f;
    public TargetingStrategy targeting;
}