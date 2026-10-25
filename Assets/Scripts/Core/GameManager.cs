using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GamePhase { Preparation, Wave, Won, Lost }

[DefaultExecutionOrder(-90)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    /// <summary>True, solange die Szene neu geladen wird / True while the scene is reloading</summary>
    public static bool IsReloading { get; private set; }

    [Header("Rang / Rank (max. verlorene Gebäude / max. structures lost)")]
    [SerializeField] private int rankSMaxLost = 0;
    [SerializeField] private int rankAMaxLost = 2;
    [SerializeField] private int rankBMaxLost = 5;

    private float powerUsageSum;
    private float waveTime;

    public GamePhase Phase { get; private set; } = GamePhase.Preparation;
    public float ElapsedTime { get; private set; }
    public int EnemiesKilled { get; private set; }
    public int StructuresLost { get; private set; }
    public bool IsPaused { get; private set; }
    public bool IsFinished => Phase == GamePhase.Won || Phase == GamePhase.Lost;

    /// <summary>Ø Energieauslastung während der Wellen (0–1) / Avg. power usage during waves (0–1)</summary>
    public float AveragePowerUsage => waveTime > 0f ? powerUsageSum / waveTime : 0f;

    public string Rank
    {
        get
        {
            if (Phase != GamePhase.Won) return "–";
            if (StructuresLost <= rankSMaxLost) return "S";
            if (StructuresLost <= rankAMaxLost) return "A";
            if (StructuresLost <= rankBMaxLost) return "B";
            return "C";
        }
    }

    public event Action<GamePhase> OnPhaseChanged;
    public event Action<bool> OnPauseChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        IsReloading = false;
        Time.timeScale = 1f;
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
        if (IsFinished || IsPaused) return;

        ElapsedTime += Time.deltaTime;

        PowerManager power = PowerManager.Instance;
        if (Phase == GamePhase.Wave && power != null && power.Capacity > 0)
        {
            powerUsageSum += (float)power.Used / power.Capacity * Time.deltaTime;
            waveTime += Time.deltaTime;
        }
    }

    public void SetPhase(GamePhase newPhase)
    {
        if (IsFinished || newPhase == Phase) return;

        Phase = newPhase;
        Debug.Log($"[Game] Phase: {Phase}");

        if (IsFinished)
        {
            Time.timeScale = 0f;
            Debug.Log($"[Game] Ergebnis / Result: Rang {Rank} | Zeit {ElapsedTime:F0}s | Gegner {EnemiesKilled} | Gebäude verloren {StructuresLost}");
        }

        OnPhaseChanged?.Invoke(Phase);
    }

    public void SetPaused(bool paused)
    {
        if (IsFinished || IsPaused == paused) return;

        IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
        OnPauseChanged?.Invoke(paused);
    }

    public void TogglePause() => SetPaused(!IsPaused);

    public void Restart()
    {
        IsReloading = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

    [ContextMenu("Test: Sieg / Victory")]
    private void TestWin() => SetPhase(GamePhase.Won);

    [ContextMenu("Test: Niederlage / Defeat")]
    private void TestLose() => SetPhase(GamePhase.Lost);
}