using UnityEngine;

[DefaultExecutionOrder(-75)]
public class GridOccupant : MonoBehaviour
{
    public Vector2Int GridPosition { get; private set; }

    private void Start()
    {
        GridManager grid = GridManager.Instance;
        GridPosition = grid.GetGridPosition(transform.position);
        transform.position = grid.GetWorldPosition(GridPosition);
        grid.SetOccupant(GridPosition, gameObject);
    }

    private void OnDestroy()
    {
        if (GridManager.Instance != null)
            GridManager.Instance.ClearOccupant(GridPosition);
    }
}