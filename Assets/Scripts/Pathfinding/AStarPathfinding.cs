using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class AStarPathfinding : Tile
{
    public static AStarPathfinding PathfindingInstance;
    public UnityAction<List<AStarTile>> PathfindingEvent;

    private AStarGrid grid => AStarGrid.AStarGridInstance;

    public AStarPathfinding()
    {
        PathfindingInstance = this;
        Debug.Log("ASTAR: Pathfinder hooked");
    }

    public void FindPath(Vector2Int startPos, Vector2Int endPos, MonoBehaviour behaviour)
    {
        behaviour.StartCoroutine(WorkOutPath(startPos, endPos));
    }

    public IEnumerator WorkOutPath(Vector2Int startPos, Vector2Int endPos)
    {
        var startTile = grid.tileMap[startPos.x, startPos.y];
        var endTile = grid.tileMap[endPos.x, endPos.y];
        
        var openTiles = new List<AStarTile> { startTile };
        var closedTiles = new List<AStarTile>();

        while (openTiles.Count > 0)
        {
            //yield return new WaitForSeconds(0.25f);
            //Debug.Log("Iterating!");

            // Open tiles gets sorted at the end of each iteration therefor putting highest (F) moves first.
            var current = openTiles[openTiles.Count - 1];

            // UPDATE THE LOOK OF TILES
            foreach (var tile in openTiles)
            {
                var pos = grid.walkableMap.GetTilePositionFromIndexPosition(tile.Position);
                grid.walkableMap.SetTileFlags(pos, TileFlags.None);
                grid.walkableMap.SetColor(pos, Color.magenta);
            }
            
            foreach (var tile in closedTiles)
            {
                var pos = grid.walkableMap.GetTilePositionFromIndexPosition(tile.Position);
                grid.walkableMap.SetTileFlags(pos, TileFlags.None);
                grid.walkableMap.SetColor(pos, Color.grey);
            }
            
            var cPos = grid.walkableMap.GetTilePositionFromIndexPosition(current.Position);
            grid.walkableMap.SetTileFlags(cPos, TileFlags.None);
            grid.walkableMap.SetColor(cPos, Color.yellow);
            
            // Evaluating a tile, close it.
            openTiles.Remove(current);
            closedTiles.Add(current);

            // Check if we've reached the end.
            if (current == endTile)
            {
                Debug.Log("Reached the end!");
                // CREATE A PATH AND RETURN IT
                var path = new List<AStarTile>();

                var iter = current;
                
                while (iter != startTile)
                {
                    path.Add(iter);
                    iter = iter.Parent;
                }

                startTile.IsCorner = true;
                path.Add(startTile);
                
                // Flip it to make it easier to traverse (0th index being the closest tile, exclusive)
                path.Reverse();

                PathfindingEvent.Invoke(PathfindingPostProcessor.ProccesPath(path));
                yield break;
            }

            var neighbors = new List<AStarTile>();
            
            // Find and check neighbors for paths
            foreach (var neighbor in grid.GetNeighbors(current))
            {
                // Check if a neighbor is a (Wall, closed or in openTiles). If so they have either already been evaluated or may be evaluated later.
                // Note we are going for the lowest (F) at all times so we check before we evaluate any tile.
                if (!neighbor.IsWalkable || closedTiles.Contains(neighbor) || openTiles.Contains(neighbor)) continue;

                /*
                * If the position is diagonal from the current node it will return a vector of a diagonal e.g (1,1)
                * Diagonals have the magnitude of sqrt(x^2+y^2) which in this case is sqrt(2) which is greater than 1.
                * Horizontal and vertical paths however have a magnitude of 1 of which the square also results in 1.
                * NOTE that the resultant is not an Absolute value which is due to the fact that squared numbers always
                * result in a positive value unless said number is imaginary of which computers dont deal in.
                */
                var isDiagonal = (current.Position - neighbor.Position).sqrMagnitude > 1;
                
                neighbor.Parent = current;
                neighbor.G = current.G + (isDiagonal ? 14 : 10);
                neighbor.H = ManhattanDistance(neighbor.Position, endPos);
                
                neighbors.Add(neighbor);
            }
            openTiles.AddRange(neighbors);
            
            // Results in a sorted list in descending order.
            openTiles.Sort((a, b) => b.F - a.F);
        }
    }

    private int ManhattanDistance(Vector2Int node, Vector2Int end)
    {
        var delta = end - node;
        return Math.Abs(delta.y) + Math.Abs(delta.x);
    }
}