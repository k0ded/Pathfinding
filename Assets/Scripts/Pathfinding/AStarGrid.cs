using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarGrid : MonoBehaviour
{

    [SerializeField] public Tilemap walkableMap;

    public static AStarGrid AStarGridInstance;
    
    public AStarTile[,] tileMap;

    private int xSize, ySize;
    
    private void Awake()
    {
        AStarGridInstance = this;
        Debug.Log("ASTAR: Grid initialized");

        CreateMap();
    }

    private void CreateMap()
    {
        var cellBounds = walkableMap.cellBounds;
        xSize = cellBounds.xMax - cellBounds.xMin;
        ySize = cellBounds.yMax - cellBounds.yMin;

        tileMap = new AStarTile[xSize, ySize];
        for (var i = 0; i < xSize; i++)
        {
            for (var j = 0; j < ySize; j++)
            {
                var checkPos = new Vector3Int(cellBounds.xMin + i, cellBounds.yMin + j, 0);

                var isWalkable = walkableMap.HasTile(checkPos);

                tileMap[i, j] = new AStarTile(new Vector2Int(i, j), isWalkable);
            }
        }
    }

    public List<AStarTile> GetNeighbors(AStarTile tile)
    {
        var tiles = new List<AStarTile>();
        var x = tile.Position.x;
        var y = tile.Position.y;
        
        for (var i = 0; i < 9; i++)
        {
            var dX = i % 3 - 1;
            var dY = i / 3 - 1;

            var nX = x + dX;
            var nY = y + dY;

            if (dX == 0 && dY == 0) continue;
            if (nX < 0) continue;
            if (nX >= xSize) continue;
            if (nY < 0) continue;
            if (nY >= ySize) continue;
            
            tiles.Add(tileMap[nX, nY]);
        }
        
        return FilterNeighbors(tiles, tile);
    }

    private List<AStarTile> FilterNeighbors(List<AStarTile> neighbors, AStarTile tile)
    {
        var tiles = neighbors;
        var removedNeighbors = new List<AStarTile>();

        if (neighbors.Any(starTile => !starTile.IsWalkable && (starTile.Position - tile.Position).sqrMagnitude != 1))
            tile.IsCorner = true;
        
        // Grabs neighbors that are not walkable and are not diagonal to the tile.
        var directContactWalls = neighbors.Where(starTile => !starTile.IsWalkable && (starTile.Position - tile.Position).sqrMagnitude == 1);

        foreach (var wall in directContactWalls)
        {
            var horizontal = (wall.Position - tile.Position) * Vector2Int.right;
            var wallX = wall.Position.x;
            var wallY = wall.Position.y;
            
            // The wall is to the horizontal to the tile.
            if (horizontal.sqrMagnitude == 1)
            {
                if(wallY < ySize - 1)
                    removedNeighbors.Add(tileMap[wallX, wallY + 1]);
                if(wallY > 0)
                    removedNeighbors.Add(tileMap[wallX, wallY - 1]);
            }
            // The wall is vertical to the tile.
            else
            {
                if(wallX < xSize - 1)
                    removedNeighbors.Add(tileMap[wallX + 1, wallY]);
                if(wallX > 0)
                    removedNeighbors.Add(tileMap[wallX - 1, wallY]);
            }
        }

        tiles.RemoveAll(starTile => removedNeighbors.Contains(starTile));
        return tiles;
    }
}
