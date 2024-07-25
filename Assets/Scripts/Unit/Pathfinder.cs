using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    GridManager gridManager;


    void Start(){
        gridManager = GameManager.Instance.gridManager;
    }

    public Stack<GridTile> FindPath(GridTile startTile, GridTile targetTile){
        List<GridTile> openSet = new List<GridTile>();
        HashSet<GridTile> closedSet = new HashSet<GridTile>();

        openSet.Add(startTile);

        while (openSet.Count > 0) {
            GridTile currentTile = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if(openSet[i].fCost < currentTile.fCost || openSet[i].fCost == currentTile.fCost && openSet[i].hCost < currentTile.hCost) {
                    currentTile = openSet[i]; 
                } 
            }
            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            if(currentTile == targetTile) {
                return RetracePath(startTile, targetTile);
            }

            foreach(GridTile adjacency in gridManager.GetAdjacentTiles(currentTile)){
                if(!adjacency.walkable || closedSet.Contains(adjacency)) continue;

                int newMovementCostToAdjacency = currentTile.gCost + GridTile.Distance(currentTile, adjacency);
                if(newMovementCostToAdjacency < adjacency.gCost || !openSet.Contains(adjacency)){
                    adjacency.gCost = newMovementCostToAdjacency;
                    adjacency.hCost = GridTile.Distance(currentTile, targetTile);
                    adjacency.parent = currentTile;

                    if(!openSet.Contains(adjacency)) openSet.Add(adjacency);
                }
            }
        }

        return null;
    }

    Stack<GridTile> RetracePath(GridTile startTile, GridTile targetTile){
        Stack<GridTile> path = new Stack<GridTile>();
        GridTile currentTile = targetTile;

        while(currentTile != startTile) {
            path.Push(currentTile);
            currentTile = currentTile.parent;
        }
        gridManager.path = path;
        return path;
    }
}

