using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding
{
    private const int StraightCost = 10;
    private const int DiagonalCost = 14;

    private static readonly Vector2Int[] Directions =
    {
        new Vector2Int( 0,  1), new Vector2Int( 1,  0),
        new Vector2Int( 0, -1), new Vector2Int(-1,  0),
        new Vector2Int( 1,  1), new Vector2Int( 1, -1),
        new Vector2Int(-1, -1), new Vector2Int(-1,  1)
    };

    /// <summary>
    /// Sucht einen Pfad von start nach target. Gibt null zurück, wenn kein Pfad existiert.
    /// Finds a path from start to target. Returns null if no path exists.
    /// </summary>
    public static List<Vector2Int> FindPath(Vector2Int start, Vector2Int target, bool stopBeforeTarget = false)
    {
        GridManager grid = GridManager.Instance;
        GridCell startCell = grid.GetCell(start);
        GridCell targetCell = grid.GetCell(target);

        if (startCell == null || targetCell == null) return null;
        if (!targetCell.IsWalkable && !stopBeforeTarget) return null;

        grid.ResetPathData();

        List<GridCell> openSet = new List<GridCell> { startCell };
        HashSet<GridCell> closedSet = new HashSet<GridCell>();

        startCell.GCost = 0;
        startCell.HCost = Heuristic(start, target);

        while (openSet.Count > 0)
        {
            GridCell current = GetLowestCost(openSet);

            if (current == targetCell)
                return RetracePath(startCell, targetCell, stopBeforeTarget);

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Vector2Int dir in Directions)
            {
                GridCell neighbor = grid.GetCell(current.Position + dir);
                if (neighbor == null || closedSet.Contains(neighbor)) continue;

                bool isTarget = neighbor == targetCell;
                if (!neighbor.IsWalkable && !(isTarget && stopBeforeTarget)) continue;

                bool isDiagonal = dir.x != 0 && dir.y != 0;
                if (isDiagonal && !CanMoveDiagonally(current.Position, dir)) continue;

                int newGCost = current.GCost + (isDiagonal ? DiagonalCost : StraightCost);

                if (newGCost < neighbor.GCost)
                {
                    neighbor.GCost = newGCost;
                    neighbor.HCost = Heuristic(neighbor.Position, target);
                    neighbor.Parent = current;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null;
    }

    private static GridCell GetLowestCost(List<GridCell> openSet)
    {
        GridCell best = openSet[0];
        for (int i = 1; i < openSet.Count; i++)
        {
            GridCell cell = openSet[i];
            if (cell.FCost < best.FCost || (cell.FCost == best.FCost && cell.HCost < best.HCost))
                best = cell;
        }
        return best;
    }

    private static bool CanMoveDiagonally(Vector2Int from, Vector2Int dir)
    {
        GridManager grid = GridManager.Instance;
        GridCell horizontal = grid.GetCell(from + new Vector2Int(dir.x, 0));
        GridCell vertical = grid.GetCell(from + new Vector2Int(0, dir.y));
        return horizontal != null && horizontal.IsWalkable
            && vertical != null && vertical.IsWalkable;
    }

    // Octile-Distanz: passend für 8 Richtungen / Octile distance: fits 8-directional movement
    private static int Heuristic(Vector2Int a, Vector2Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return DiagonalCost * Mathf.Min(dx, dy) + StraightCost * Mathf.Abs(dx - dy);
    }

    private static List<Vector2Int> RetracePath(GridCell startCell, GridCell endCell, bool stopBeforeTarget)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        GridCell current = endCell;

        while (current != startCell)
        {
            path.Add(current.Position);
            current = current.Parent;
        }

        path.Reverse();

        if (stopBeforeTarget && path.Count > 0)
            path.RemoveAt(path.Count - 1);

        return path;
    }
}