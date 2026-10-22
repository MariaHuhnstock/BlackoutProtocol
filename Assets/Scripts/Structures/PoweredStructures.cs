using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(GridOccupant))]
public class PoweredStructure : MonoBehaviour, IPowerConsumer
{
    [SerializeField] protected StructureData data;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Light2D[] lights;
    [SerializeField] private Color unpoweredTint = new Color(0.35f, 0.35f, 0.4f);

    private Color poweredColor = Color.white;

    public StructureData Data => data;
    public string DisplayName => data.displayName;
    public int PowerCost => data.powerCost;
    public bool IsPowered { get; private set; }

    public event Action<bool> OnPowerStateChanged;

    protected virtual void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            poweredColor = spriteRenderer.color;

        ApplyVisuals();
    }

    protected virtual void Start()
    {
        PowerManager.Instance.Register(this);

        if (data.startsPowered)
            PowerManager.Instance.TryTogglePower(this);
    }

    protected virtual void OnDestroy()
    {
        if (PowerManager.Instance != null)
            PowerManager.Instance.Unregister(this);
    }

    public void SetPowered(bool powered)
    {
        if (IsPowered == powered) return;

        IsPowered = powered;
        ApplyVisuals();
        OnPowerStateChanged?.Invoke(powered);
    }

    private void ApplyVisuals()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = IsPowered ? poweredColor : unpoweredTint;

        foreach (Light2D light in lights)
        {
            if (light != null)
                light.enabled = IsPowered;
        }
    }
}