using UnityEngine;

public class GridCell
{
    public Vector2Int Position { get; }
    public GameObject Occupant { get; private set; }

    private readonly bool isTerrainWalkable;

    // Daten für A* (kommt in Schritt 2)
    public int GCost;
    public int HCost;
    public int FCost => GCost + HCost;
    public GridCell Parent;

    public bool IsTerrainWalkable => isTerrainWalkable;
    public bool IsWalkable => isTerrainWalkable && Occupant == null;

    public GridCell(int x, int y, bool walkable)
    {
        Position = new Vector2Int(x, y);
        isTerrainWalkable = walkable;
    }

    public void SetOccupant(GameObject occupant) => Occupant = occupant;
    public void ClearOccupant() => Occupant = null;
}