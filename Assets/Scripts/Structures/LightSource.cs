using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(PoweredStructure))]
public class LightSource : MonoBehaviour
{
    private static readonly List<LightSource> all = new List<LightSource>();

    [SerializeField] private Light2D light2D;
    [Tooltip("Anteil des Lichtradius, der als 'beleuchtet' zählt / Share of the light radius that counts as lit")]
    [SerializeField, Range(0.3f, 1f)] private float effectiveRadiusFactor = 0.8f;

    private PoweredStructure structure;

    public bool IsActive => structure != null && structure.IsPowered;
    public float EffectiveRadius => light2D != null ? light2D.pointLightOuterRadius * effectiveRadiusFactor : 0f;

    private void Awake()
    {
        structure = GetComponent<PoweredStructure>();
        if (light2D == null) light2D = GetComponentInChildren<Light2D>(true);
    }

    private void OnEnable() => all.Add(this);
    private void OnDisable() => all.Remove(this);

    public static bool IsPositionLit(Vector3 position)
    {
        foreach (LightSource source in all)
        {
            if (!source.IsActive) continue;

            float radius = source.EffectiveRadius;
            if ((source.transform.position - position).sqrMagnitude <= radius * radius)
                return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (light2D == null) light2D = GetComponentInChildren<Light2D>(true);
        if (light2D == null) return;

        Gizmos.color = new Color(1f, 0.9f, 0.5f, 0.6f);
        Gizmos.DrawWireSphere(transform.position, light2D.pointLightOuterRadius * effectiveRadiusFactor);
    }
}