using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Pathfinding
{
    public class PathfindingPostProcessor : MonoBehaviour
    {
        public AStarGrid map;
        [NonSerialized] public BoxCollider2D collider;
        private static PathfindingPostProcessor Instance;

        private void Awake()
        {
            Instance = this;
            collider = GetComponent<BoxCollider2D>();
        }

        public static List<AStarTile> ProccesPath(List<AStarTile> path)
        {
            return Instance.FindNecessaryCorners(path.Where(tile => tile.IsCorner), Enumerable.Empty<AStarTile>());
        }

        private List<AStarTile> FindNecessaryCorners(IEnumerable<AStarTile> corners,
            IEnumerable<AStarTile> foundTiles)
        {
            var c = new List<AStarTile>(corners);
            var fT = new List<AStarTile>(foundTiles);

            if (c.Count <= 2)
            {
                fT.AddRange(c);
                return fT;
            }

            // if you can walk to future scrap next.
            var current = c[0];
            var next = c[1];
            var future = c[2];

            var cellBounds = map.walkableMap.cellBounds;
            var cellMinX = cellBounds.xMin;
            var cellMinY = cellBounds.yMin;

            transform.position = map.walkableMap.GetCellCenterWorld(new Vector3Int(current.Position.x + cellMinX, current.Position.y + cellMinY, 0));
            var delta = future.Position - current.Position;
            
            // Sweep test
            if (TestSweep(delta))
            {
                fT.Add(current);
                fT.Add(next);
                c.Remove(current);
                c.Remove(next);
                return FindNecessaryCorners(c, fT);
            }
            c.Remove(next);
            return FindNecessaryCorners(c, fT);
        }

        private List<AStarTile> CleanUp(List<AStarTile> path)
        {
            if (path.Count <= 2)
                return path;
            var curr = path[0]
        }

        private bool TestSweep(Vector2Int relativeTo)
        {
            var relative = new Vector2(relativeTo.x, relativeTo.y);
            return GetStartingPointsOnBox().Any(vector3 => Physics2D.Raycast(vector3, relative, relative.magnitude));
        }

        private List<Vector2> GetStartingPointsOnBox()
        {
            var bounds = GetWorldBounds(collider);
            var center = bounds.center;
            var min = bounds.min;
            var max = bounds.max;

            return new List<Vector2>
            {
                center,
                min,
                max,
                new Vector3(min.x, max.y),
                new Vector3(max.x, min.y)
            };
        }
        
        public Rect GetWorldBounds(BoxCollider2D boxCollider2D)
        {
            var worldRight = boxCollider2D.transform.TransformPoint(boxCollider2D.offset + new Vector2(boxCollider2D.size.x * 0.5f, 0)).x;
            var worldLeft = boxCollider2D.transform.TransformPoint(boxCollider2D.offset - new Vector2(boxCollider2D.size.x * 0.5f, 0)).x;
 
            var worldTop = boxCollider2D.transform.TransformPoint(boxCollider2D.offset + new Vector2(0, boxCollider2D.size.y * 0.5f)).y;
            var worldBottom = boxCollider2D.transform.TransformPoint(boxCollider2D.offset - new Vector2(0, boxCollider2D.size.y * 0.5f)).y;
 
            return new Rect(
                worldLeft,
                worldBottom,
                worldRight - worldLeft,
                worldTop - worldBottom
            );
        }
    }
}