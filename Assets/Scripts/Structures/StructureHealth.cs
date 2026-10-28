using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridOccupant))]
public class StructureHealth : MonoBehaviour, IDamageable
{
    private static readonly List<StructureHealth> all = new List<StructureHealth>();
    public static IReadOnlyList<StructureHealth> All => all;

    [Tooltip("Leer lassen, wenn eine PoweredStructure am Objekt ist / Leave empty if a PoweredStructure is attached")]
    [SerializeField] private StructureData data;

    private GridOccupant occupant;
    private PoweredStructure powered;

    public float CurrentHealth { get; private set; }
    public float MaxHealth => data.maxHealth;
    public bool IsDestroyed { get; private set; }
    public string DisplayName => data.displayName;
    public int RebuildCost => data.rebuildCost;
    public Vector2Int GridPosition => occupant.GridPosition;

    // Der Generator ist nicht wiederaufbaubar / The generator cannot be rebuilt
    public bool CanBeRebuilt => GetComponent<Generator>() == null;

    public event Action<float, float> OnHealthChanged;          // current, max
    public static event Action<StructureHealth> OnAnyDestroyed;

    private void Awake()
    {
        occupant = GetComponent<GridOccupant>();
        powered = GetComponent<PoweredStructure>();

        if (data == null && powered != null)
            data = powered.Data;

        CurrentHealth = data.maxHealth;
    }

    private void OnEnable() => all.Add(this);
    private void OnDisable() => all.Remove(this);

    public void TakeDamage(float amount)
    {
        if (IsDestroyed) return;

        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0f)
            DestroyStructure();
    }

    public void Repair(float amount)
    {
        if (IsDestroyed) return;

        CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    private void DestroyStructure()
    {
        IsDestroyed = true;
        Debug.Log($"{DisplayName} zerstört / destroyed");

        // Energie freigeben / Release power
        if (powered != null && powered.IsPowered && PowerManager.Instance != null)
            PowerManager.Instance.TryTogglePower(powered);

        OnAnyDestroyed?.Invoke(this);

        bool ruinCreated = CanBeRebuilt
            && RebuildManager.Instance != null
            && RebuildManager.Instance.CreateRuin(this);

        if (ruinCreated)
        {
            occupant.Release();
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Rebuild()
    {
        if (!IsDestroyed) return;

        IsDestroyed = false;
        CurrentHealth = MaxHealth;
        occupant.Claim();
        gameObject.SetActive(true);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    [ContextMenu("Test: 50 % Schaden / 50% damage")]
    private void TestDamageHalf() => TakeDamage(MaxHealth * 0.5f);

    [ContextMenu("Test: Zerstören / Destroy")]
    private void TestDestroy() => TakeDamage(MaxHealth);
}