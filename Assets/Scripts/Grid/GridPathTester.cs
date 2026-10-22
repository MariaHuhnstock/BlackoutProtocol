using UnityEngine;
using UnityEngine.InputSystem;

public class GridPathTester : MonoBehaviour
{
    [SerializeField] private GridPathMover mover;

    private Camera cam;

    private void Start() => cam = Camera.main;

    private void Update()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2Int pos = GetMouseGridPosition();
            if (!mover.MoveTo(pos))
                Debug.Log($"Kein Pfad zu {pos} / No path to {pos}");
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            ToggleBlocker(GetMouseGridPosition());
        }
    }

    private Vector2Int GetMouseGridPosition()
    {
        Vector3 world = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        world.z = 0f;
        return GridManager.Instance.GetGridPosition(world);
    }

    private void ToggleBlocker(Vector2Int pos)
    {
        GridCell cell = GridManager.Instance.GetCell(pos);
        if (cell == null || !cell.IsTerrainWalkable) return;

        if (cell.Occupant != null)
        {
            Destroy(cell.Occupant);
            GridManager.Instance.ClearOccupant(pos);
        }
        else
        {
            GameObject blocker = new GameObject($"TestBlocker_{pos}");
            blocker.transform.position = GridManager.Instance.GetWorldPosition(pos);
            GridManager.Instance.SetOccupant(pos, blocker);
        }
    }
}