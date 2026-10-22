using UnityEngine;
using UnityEngine.InputSystem;

public class PowerInput : MonoBehaviour
{
    private Camera cam;

    private void Start() => cam = Camera.main;

    private void Update()
    {
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame) return;

        Vector3 world = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        world.z = 0f;

        GridCell cell = GridManager.Instance.GetCell(world);
        if (cell == null || cell.Occupant == null) return;

        IPowerConsumer consumer = cell.Occupant.GetComponent<IPowerConsumer>();
        if (consumer == null) return;

        PowerManager.Instance.TryTogglePower(consumer);
    }
}