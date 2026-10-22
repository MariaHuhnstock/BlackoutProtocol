using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyTestSpawner : MonoBehaviour
{
    [SerializeField] private EnemyController crawlerPrefab;
    [SerializeField] private EnemyController breakerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private void Start()
    {
        Debug.Log($"[Spawner] aktiv | Crawler: {(crawlerPrefab != null)} | Breaker: {(breakerPrefab != null)} | Spawnpunkte: {spawnPoints.Length}");
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            Debug.LogWarning("[Spawner] Keine Tastatur gefunden / No keyboard found");
            return;
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame || Keyboard.current.numpad1Key.wasPressedThisFrame)
        {
            Debug.Log("[Spawner] Taste 1 erkannt / Key 1 detected");
            Spawn(crawlerPrefab);
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame || Keyboard.current.numpad2Key.wasPressedThisFrame)
        {
            Debug.Log("[Spawner] Taste 2 erkannt / Key 2 detected");
            Spawn(breakerPrefab);
        }
    }

    private void Spawn(EnemyController prefab)
    {
        if (prefab == null)
        {
            Debug.LogWarning("[Spawner] Prefab fehlt / Prefab missing");
            return;
        }

        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("[Spawner] Keine Spawnpunkte / No spawn points");
            return;
        }

        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        if (point == null)
        {
            Debug.LogWarning("[Spawner] Spawnpunkt-Eintrag ist leer / Spawn point entry is empty");
            return;
        }

        EnemyController enemy = Instantiate(prefab, point.position, Quaternion.identity);
        Debug.Log($"[Spawner] {enemy.name} gespawnt bei {point.position}");
    }
}