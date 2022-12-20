using UnityEngine;

public class AStarTile
{
    public bool IsCorner;
    public bool IsWalkable;
    public Vector2Int Position;

    // G -> Actual movement cost. H -> Hypothesised cost (Manhattan distance)
    public int G, H;
    
    // Value that we use to TRY to determine the final cost of the path.
    public int F => G + H;

    public AStarTile Parent;

    public AStarTile(Vector2Int pos, bool walkable)
    {
        IsWalkable = walkable;
        Position = pos;
    }
}