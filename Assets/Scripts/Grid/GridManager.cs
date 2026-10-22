using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Grid")]
    [SerializeField] private int width = 20;
    [SerializeField] private int height = 12;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Vector2 origin = new Vector2(-10f, -6f);

    [Header("Hindernisse")]
    [SerializeField] private Tilemap obstacleTilemap;

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = true;

    private GridCell[,] cells;

    public int Width => width;
    public int Height => height;
    public float CellSize => cellSize;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        BuildGrid();
    }

    private void BuildGrid()
    {
        cells = new GridCell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool blocked = false;
                if (obstacleTilemap != null)
                {
                    Vector3Int tilePos = obstacleTilemap.WorldToCell(GetWorldPosition(x, y));
                    blocked = obstacleTilemap.HasTile(tilePos);
                }
                cells[x, y] = new GridCell(x, y, !blocked);
            }
        }
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(
            origin.x + (x + 0.5f) * cellSize,
            origin.y + (y + 0.5f) * cellSize,
            0f);
    }

    public Vector3 GetWorldPosition(Vector2Int gridPos) => GetWorldPosition(gridPos.x, gridPos.y);

    public Vector2Int GetGridPosition(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - origin.x) / cellSize);
        int y = Mathf.FloorToInt((worldPos.y - origin.y) / cellSize);
        return new Vector2Int(x, y);
    }

    public bool IsInside(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    public GridCell GetCell(Vector2Int pos)
    {
        return IsInside(pos) ? cells[pos.x, pos.y] : null;
    }

    public GridCell GetCell(Vector3 worldPos) => GetCell(GetGridPosition(worldPos));

    public void ResetPathData()
    {
        foreach (GridCell cell in cells)
        {
            cell.GCost = int.MaxValue;
            cell.HCost = 0;
            cell.Parent = null;
        }
    }

    public void SetOccupant(Vector2Int pos, GameObject occupant)
    {
        GetCell(pos)?.SetOccupant(occupant);
    }

    public void ClearOccupant(Vector2Int pos)
    {
        GetCell(pos)?.ClearOccupant();
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 center = GetWorldPosition(x, y);
                Vector3 size = new Vector3(cellSize, cellSize, 0f) * 0.95f;

                if (cells != null)
                {
                    GridCell cell = cells[x, y];
                    if (!cell.IsTerrainWalkable)
                    {
                        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.4f);
                        Gizmos.DrawCube(center, size);
                    }
                    else if (cell.Occupant != null)
                    {
                        Gizmos.color = new Color(0.7f, 0.3f, 1f, 0.4f);
                        Gizmos.DrawCube(center, size);
                    }
                }

                Gizmos.color = new Color(1f, 1f, 1f, 0.15f);
                Gizmos.DrawWireCube(center, size);
            }
        }
    }
}