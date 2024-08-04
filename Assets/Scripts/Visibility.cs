using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Visibility : MonoBehaviour
{
    public static List<GridTile> GetVisibleTilesFromList(GridTile startTile, List<GridTile> tiles){
        List<GridTile> visibleTiles = new();
        foreach (GridTile tile in tiles)
        {
            if(IsTileVisibleFromTile(startTile.gridPosition, tile.gridPosition)){
                visibleTiles.Add(tile);
            }
        }
        return visibleTiles.Distinct().ToList();
    }

    public static bool IsTileVisibleFromTile(Vector2Int startTile, Vector2Int checkTile){
        GridManager gridManager = GameManager.Instance.gridManager;

        float dx = checkTile.x - startTile.x;
        float dy = checkTile.y - startTile.y;
        
        float nx = Mathf.Abs(dx);
        float ny = Mathf.Abs(dy);

        float signX = dx > 0 ? 1 : -1;
        float signY = dy > 0 ? 1 : -1;

        Vector2Int checkPos = startTile;
        for (int ix = 0, iy = 0; ix < nx || iy < ny;)
        {
            if((0.5 + ix) / nx < (0.5 + iy) / ny){
                // Horizontal Step
                checkPos.x += (int)signX;
                ix++;
            }
            else {
                checkPos.y += (int)signY;
                iy++;
            }
            bool visible = gridManager.tiles[checkPos.x, checkPos.y].walkable;
            if(!visible) return false; 
        }
        return true;
    }
}