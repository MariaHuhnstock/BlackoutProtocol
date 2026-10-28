using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebuildManager : MonoBehaviour
{
    public static RebuildManager Instance { get; private set; }

    [SerializeField] private Ruin ruinPrefab;
    [SerializeField] private int startScrap = 0;

    private Camera cam;

    public int Scrap { get; private set; }

    public event Action<int> OnScrapChanged;
    public event Action<Ruin> OnRebuildDenied;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Scrap = startScrap;
    }

    private void Start()
    {
        cam = Camera.main;
        OnScrapChanged?.Invoke(Scrap);
    }

    private void OnEnable() => EnemyController.OnEnemyKilled += HandleEnemyKilled;
    private void OnDisable() => EnemyController.OnEnemyKilled -= HandleEnemyKilled;

    private void HandleEnemyKilled(EnemyController enemy)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsFinished) return;

        Scrap += enemy.Data.scrapReward;
        OnScrapChanged?.Invoke(Scrap);
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame) return;
        if (GameManager.Instance == null || GameManager.Instance.Phase != GamePhase.Preparation) return;

        Vector3 world = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        world.z = 0f;

        Ruin ruin = Ruin.FindAt(GridManager.Instance.GetGridPosition(world));
        if (ruin != null) TryRebuild(ruin);
    }

    public bool CreateRuin(StructureHealth structure)
    {
        if (ruinPrefab == null) return false;

        Vector3 position = GridManager.Instance.GetWorldPosition(structure.GridPosition);
        Ruin ruin = Instantiate(ruinPrefab, position, Quaternion.identity);
        ruin.Setup(structure);
        return true;
    }

    public bool TryRebuild(Ruin ruin)
    {
        int cost = ruin.Structure.RebuildCost;

        if (Scrap < cost)
        {
            OnRebuildDenied?.Invoke(ruin);
            return false;
        }

        Scrap -= cost;
        OnScrapChanged?.Invoke(Scrap);

        ruin.Structure.Rebuild();
        Destroy(ruin.gameObject);
        return true;
    }

    [ContextMenu("Test: +10 Schrott / +10 scrap")]
    private void TestAddScrap()
    {
        Scrap += 10;
        OnScrapChanged?.Invoke(Scrap);
    }
}