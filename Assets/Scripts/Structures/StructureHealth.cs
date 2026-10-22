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

    public float CurrentHealth { get; private set; }
    public float MaxHealth => data.maxHealth;
    public bool IsDestroyed { get; private set; }
    public string DisplayName => data.displayName;
    public Vector2Int GridPosition => occupant.GridPosition;

    public event Action<float, float> OnHealthChanged;          // current, max
    public static event Action<StructureHealth> OnAnyDestroyed;

    private void Awake()
    {
        occupant = GetComponent<GridOccupant>();

        if (data == null)
        {
            PoweredStructure powered = GetComponent<PoweredStructure>();
            if (powered != null) data = powered.Data;
        }

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
        OnAnyDestroyed?.Invoke(this);
        Destroy(gameObject);
    }
}