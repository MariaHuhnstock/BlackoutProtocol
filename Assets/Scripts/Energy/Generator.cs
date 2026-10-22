using UnityEngine;

[DefaultExecutionOrder(-50)]
[RequireComponent(typeof(GridOccupant))]
public class Generator : MonoBehaviour
{
    public static Generator Instance { get; private set; }

    [SerializeField] private int powerOutput = 10;

    public int PowerOutput => powerOutput;

    private void Awake() => Instance = this;

    private void Start()
    {
        PowerManager.Instance.SetCapacity(powerOutput);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;

        if (PowerManager.Instance != null)
        {
            PowerManager.Instance.SetCapacity(0);
            Debug.Log("Generator zerstört – Game Over / Generator destroyed – Game Over");
        }
    }

    public void SetOutput(int value)
    {
        powerOutput = Mathf.Max(0, value);
        PowerManager.Instance.SetCapacity(powerOutput);
    }

    [ContextMenu("Test: Output auf 5 / Output to 5")]
    private void TestReduceOutput() => SetOutput(5);

    [ContextMenu("Test: Output auf 10 / Output to 10")]
    private void TestRestoreOutput() => SetOutput(10);
}