using System;
using UnityEngine;

public enum GamePhase { Preparation, Wave, Won, Lost }

[DefaultExecutionOrder(-90)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GamePhase Phase { get; private set; } = GamePhase.Preparation;
    public float ElapsedTime { get; private set; }
    public int EnemiesKilled { get; private set; }
    public int StructuresLost { get; private set; }
    public bool IsFinished => Phase == GamePhase.Won || Phase == GamePhase.Lost;

    public event Action<GamePhase> OnPhaseChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        EnemyController.OnEnemyKilled += HandleEnemyKilled;
        StructureHealth.OnAnyDestroyed += HandleStructureDestroyed;
    }

    private void OnDisable()
    {
        EnemyController.OnEnemyKilled -= HandleEnemyKilled;
        StructureHealth.OnAnyDestroyed -= HandleStructureDestroyed;
    }

    private void Update()
    {
        if (!IsFinished) ElapsedTime += Time.deltaTime;
    }

    public void SetPhase(GamePhase newPhase)
    {
        if (IsFinished || newPhase == Phase) return;

        Phase = newPhase;
        Debug.Log($"[Game] Phase: {Phase}");

        if (IsFinished)
        {
            Debug.Log($"[Game] Ergebnis / Result: Zeit {ElapsedTime:F0}s | Gegner besiegt {EnemiesKilled} | Gebäude verloren {StructuresLost}");
        }

        OnPhaseChanged?.Invoke(Phase);
    }

    private void HandleEnemyKilled(EnemyController enemy)
    {
        if (!IsFinished) EnemiesKilled++;
    }

    private void HandleStructureDestroyed(StructureHealth structure)
    {
        if (IsFinished) return;

        StructuresLost++;

        if (structure.GetComponent<Generator>() != null)
            SetPhase(GamePhase.Lost);
    }
}