using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class PowerManager : MonoBehaviour
{
    public static PowerManager Instance { get; private set; }

    private readonly List<IPowerConsumer> consumers = new List<IPowerConsumer>();
    private readonly List<IPowerConsumer> activationOrder = new List<IPowerConsumer>();

    public int Capacity { get; private set; }
    public int Used { get; private set; }
    public int Available => Capacity - Used;
    public IReadOnlyList<IPowerConsumer> Consumers => consumers;

    public event Action<int, int> OnPowerChanged;          // used, capacity
    public event Action<IPowerConsumer> OnPowerDenied;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetCapacity(int value)
    {
        Capacity = Mathf.Max(0, value);

        // Überlast: zuletzt aktivierte Verbraucher abschalten / Overload: shed most recently activated consumers
        while (Used > Capacity && activationOrder.Count > 0)
        {
            SetConsumerState(activationOrder[activationOrder.Count - 1], false);
        }

        NotifyChanged();
    }

    public void Register(IPowerConsumer consumer)
    {
        if (!consumers.Contains(consumer))
            consumers.Add(consumer);
    }

    public void Unregister(IPowerConsumer consumer)
    {
        if (consumer.IsPowered)
            SetConsumerState(consumer, false);

        consumers.Remove(consumer);
        NotifyChanged();
    }

    public bool TryTogglePower(IPowerConsumer consumer)
    {
        if (consumer.IsPowered)
        {
            SetConsumerState(consumer, false);
            NotifyChanged();
            return true;
        }

        if (consumer.PowerCost > Available)
        {
            OnPowerDenied?.Invoke(consumer);
            return false;
        }

        SetConsumerState(consumer, true);
        NotifyChanged();
        return true;
    }

    private void SetConsumerState(IPowerConsumer consumer, bool powered)
    {
        consumer.SetPowered(powered);

        if (powered)
        {
            Used += consumer.PowerCost;
            activationOrder.Add(consumer);
        }
        else
        {
            Used -= consumer.PowerCost;
            activationOrder.Remove(consumer);
        }
    }

    private void NotifyChanged() => OnPowerChanged?.Invoke(Used, Capacity);
}