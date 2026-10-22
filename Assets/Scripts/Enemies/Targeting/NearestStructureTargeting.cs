using UnityEngine;

[CreateAssetMenu(fileName = "NearestStructure", menuName = "Blackout Protocol/Targeting/Nearest Structure")]
public class NearestStructureTargeting : TargetingStrategy
{
    public override StructureHealth FindTarget(Vector3 fromPosition)
    {
        StructureHealth best = null;
        float bestDistance = float.MaxValue;

        foreach (StructureHealth structure in StructureHealth.All)
        {
            if (structure.IsDestroyed) continue;

            float distance = (structure.transform.position - fromPosition).sqrMagnitude;
            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = structure;
            }
        }

        return best;
    }
}