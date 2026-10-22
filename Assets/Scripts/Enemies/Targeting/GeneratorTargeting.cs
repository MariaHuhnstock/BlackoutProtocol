using UnityEngine;

[CreateAssetMenu(fileName = "GeneratorFirst", menuName = "Blackout Protocol/Targeting/Generator First")]
public class GeneratorTargeting : TargetingStrategy
{
    [Tooltip("Falls der Generator nicht existiert / If the generator doesn't exist")]
    [SerializeField] private TargetingStrategy fallback;

    public override StructureHealth FindTarget(Vector3 fromPosition)
    {
        if (Generator.Instance != null)
        {
            StructureHealth health = Generator.Instance.GetComponent<StructureHealth>();
            if (health != null && !health.IsDestroyed)
                return health;
        }

        return fallback != null ? fallback.FindTarget(fromPosition) : null;
    }
}