using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangeFinder : MonoBehaviour
{
    public static List<GridTile> GetTilesInRangeFromTile(GridTile originTile, int range){
        List<GridTile> inRangeTiles = new();

        List<GridTile> openSet = new();
        HashSet<GridTile> closedSet = new();

        int stepCount = range + 1;

        openSet.Add(originTile);

        for (int i = 0; i < stepCount; i++)
        {
            List<GridTile> openSetCopy = new(openSet);
            foreach (GridTile openTile in openSetCopy)
            {
                closedSet.Add(openTile);
                openSet.Remove(openTile);
                inRangeTiles.Add(openTile);

                foreach (GridTile adjacency in GridManager.GetAdjacentTiles(openTile.gridPosition))
                {
                    if(closedSet.Contains(adjacency)) continue;
                    openSet.Add(adjacency);
                }
            }
        }

        return inRangeTiles.Distinct().ToList();
    }

    public static List<GridTile> GetWalkableTilesInRange(GridTile originTile, int range){
        List<GridTile> inRangeTiles = new();

        List<GridTile> openSet = new();
        HashSet<GridTile> closedSet = new();

        int stepCount = range + 1;

        openSet.Add(originTile);

        for (int i = 0; i < stepCount; i++)
        {
            List<GridTile> openSetCopy = new(openSet);
            foreach (GridTile openTile in openSetCopy)
            {
                closedSet.Add(openTile);
                openSet.Remove(openTile);
                inRangeTiles.Add(openTile);

                foreach (GridTile adjacency in GridManager.GetAdjacentTiles(openTile.gridPosition))
                {
                    if(closedSet.Contains(adjacency)) continue;
                    if(!adjacency.walkable) continue;
                    openSet.Add(adjacency);
                }
            }
        }

        return inRangeTiles.Distinct().ToList();
    }

}
