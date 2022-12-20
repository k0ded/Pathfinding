using UnityEngine;
using UnityEngine.Tilemaps;

public static class ExtensionMethods
{
    public static Vector3Int GetTilePositionFromIndexPosition(this Tilemap tilemap, Vector2Int position)
    {
        var cellBounds = tilemap.cellBounds;
        var x = cellBounds.xMin + position.x;
        var y = cellBounds.yMin + position.y;

        return new Vector3Int(x, y, 0);
    }
}