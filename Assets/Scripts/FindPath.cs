using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FindPath : MonoBehaviour
{
    [SerializeField] private Tilemap walkableTilemap;

    private void Awake()
    {
        Debug.Log("ASTAR: Finding path");
        
        var cellBounds = walkableTilemap.cellBounds;
        var xSize = cellBounds.xMax - cellBounds.xMin;
        var ySize = cellBounds.yMax - cellBounds.yMin;

        Vector2Int start = default, end = default;
        
        for (var i = 0; i < xSize; i++)
        {
            for (var j = 0; j < ySize; j++)
            {
                var tile = walkableTilemap.GetTile(new Vector3Int(i + cellBounds.xMin, j + cellBounds.yMin, 0));
                if (tile == null) continue;
                switch (tile.name)
                {
                    case "Start":
                        start = new Vector2Int(i, j);
                        break;
                    case "End":
                        end = new Vector2Int(i, j);
                        break;
                }
            }
        }

        Debug.Log("START: " + start + " END: " + end);

        AStarPathfinding.PathfindingInstance.PathfindingEvent += OnFoundGoal;
        AStarPathfinding.PathfindingInstance.FindPath(start, end, this);

        
    }

    private void OnFoundGoal(List<AStarTile> path)
    {
        Debug.Log("Path found");
        foreach (var tile in path)
        {
            var bounds = walkableTilemap.cellBounds;
            var x = bounds.xMin + tile.Position.x; 
            var y = bounds.yMin + tile.Position.y;
            var pos = new Vector3Int(x, y, 0);
            walkableTilemap.SetTileFlags(pos, TileFlags.None);
            walkableTilemap.SetColor(pos, Color.yellow);
        }
    }
}