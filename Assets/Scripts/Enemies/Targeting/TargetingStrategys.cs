using UnityEngine;

public abstract class TargetingStrategy : ScriptableObject
{
    public abstract StructureHealth FindTarget(Vector3 fromPosition);
}