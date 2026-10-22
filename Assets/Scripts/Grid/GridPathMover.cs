using System;
using System.Collections.Generic;
using UnityEngine;

public class GridPathMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;

    private List<Vector2Int> path;
    private int pathIndex;
    private Vector2Int finalTarget;
    private bool stopBeforeFinalTarget;

    public bool IsMoving => path != null && pathIndex < path.Count;
    public event Action OnPathCompleted;
    public event Action OnPathBlocked;

    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    public bool MoveTo(Vector2Int target, bool stopBeforeTarget = false)
    {
        Vector2Int start = GridManager.Instance.GetGridPosition(transform.position);
        List<Vector2Int> newPath = Pathfinding.FindPath(start, target, stopBeforeTarget);

        if (newPath == null) return false;

        path = newPath;
        pathIndex = 0;
        finalTarget = target;
        stopBeforeFinalTarget = stopBeforeTarget;
        return true;
    }

    public void Stop() => path = null;

    private void Update()
    {
        if (!IsMoving) return;

        // Nächstes Feld inzwischen blockiert? Neu berechnen / Next cell blocked? Recalculate
        GridCell nextCell = GridManager.Instance.GetCell(path[pathIndex]);
        if (nextCell != null && !nextCell.IsWalkable)
        {
            if (!MoveTo(finalTarget, stopBeforeFinalTarget))
            {
                path = null;
                OnPathBlocked?.Invoke();
            }
            return;
        }

        Vector3 targetPos = GridManager.Instance.GetWorldPosition(path[pathIndex]);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if ((transform.position - targetPos).sqrMagnitude < 0.0001f)
        {
            pathIndex++;
            if (pathIndex >= path.Count)
            {
                path = null;
                OnPathCompleted?.Invoke();
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!IsMoving || GridManager.Instance == null) return;

        Gizmos.color = new Color(0.7f, 0.3f, 1f);
        Vector3 previous = transform.position;
        for (int i = pathIndex; i < path.Count; i++)
        {
            Vector3 point = GridManager.Instance.GetWorldPosition(path[i]);
            Gizmos.DrawLine(previous, point);
            Gizmos.DrawSphere(point, 0.08f);
            previous = point;
        }
    }
}