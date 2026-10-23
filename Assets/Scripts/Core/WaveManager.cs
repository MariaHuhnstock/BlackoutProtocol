using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("Wellen / Waves")]
    [SerializeField] private WaveData[] waves;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Zeiten / Timing")]
    [SerializeField] private float firstPrepDuration = 30f;
    [SerializeField] private float prepDuration = 20f;

    /// <summary>Nummer der laufenden bzw. zuletzt beendeten Welle (0 = noch keine) / Number of the current or last finished wave</summary>
    public int CurrentWave { get; private set; }
    public int TotalWaves => waves != null ? waves.Length : 0;
    public float PrepTimeLeft { get; private set; }
    public int EnemiesToSpawn { get; private set; }
    public int EnemiesAlive => EnemyController.Active.Count;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.OnPhaseChanged += HandlePhaseChanged;
        StartCoroutine(RunLevel());
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPhaseChanged -= HandlePhaseChanged;
    }

    private void HandlePhaseChanged(GamePhase phase)
    {
        // Spiel vorbei: keine weiteren Gegner / Game over: stop spawning
        if (phase == GamePhase.Won || phase == GamePhase.Lost)
        {
            StopAllCoroutines();
            EnemiesToSpawn = 0;
            PrepTimeLeft = 0f;
        }
    }

    private IEnumerator RunLevel()
    {
        GameManager.Instance.SetPhase(GamePhase.Preparation);
        yield return Prep(firstPrepDuration);

        for (int i = 0; i < waves.Length; i++)
        {
            CurrentWave = i + 1;
            GameManager.Instance.SetPhase(GamePhase.Wave);

            yield return SpawnWave(waves[i]);

            // Warten, bis alle Gegner besiegt sind / Wait until all enemies are dead
            while (EnemyController.Active.Count > 0)
                yield return null;

            if (i < waves.Length - 1)
            {
                GameManager.Instance.SetPhase(GamePhase.Preparation);
                yield return Prep(prepDuration);
            }
        }

        GameManager.Instance.SetPhase(GamePhase.Won);
    }

    private IEnumerator Prep(float duration)
    {
        PrepTimeLeft = duration;

        while (PrepTimeLeft > 0f)
        {
            // Leertaste: Welle früher starten / Space: start wave early
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
                break;

            PrepTimeLeft -= Time.deltaTime;
            yield return null;
        }

        PrepTimeLeft = 0f;
    }

    private IEnumerator SpawnWave(WaveData wave)
    {
        EnemiesToSpawn = 0;
        foreach (SpawnGroup group in wave.groups)
            EnemiesToSpawn += group.count;

        // Alle Gruppen laufen parallel / All groups run in parallel
        int runningGroups = wave.groups.Length;
        foreach (SpawnGroup group in wave.groups)
            StartCoroutine(SpawnGroupRoutine(group, () => runningGroups--));

        while (runningGroups > 0)
            yield return null;
    }

    private IEnumerator SpawnGroupRoutine(SpawnGroup group, Action onDone)
    {
        if (group.startDelay > 0f)
            yield return new WaitForSeconds(group.startDelay);

        for (int i = 0; i < group.count; i++)
        {
            SpawnEnemy(group.prefab);
            EnemiesToSpawn--;

            if (i < group.count - 1)
                yield return new WaitForSeconds(group.interval);
        }

        onDone?.Invoke();
    }

    private void SpawnEnemy(EnemyController prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("[WaveManager] Spawn-Gruppe ohne Prefab / Spawn group without prefab");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[WaveManager] Keine Spawnpunkte / No spawn points");
            return;
        }

        Transform point = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        Instantiate(prefab, point.position, Quaternion.identity);
    }
}