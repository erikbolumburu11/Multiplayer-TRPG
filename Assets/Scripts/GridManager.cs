using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridTile {
    public Vector2Int gridPosition;
    public Vector3 worldPosition;
    public bool walkable;
    public GridTile parent;
    public Image overlayImage;

    public int gCost;
    public int hCost;
    public int fCost {
        get {
            return gCost + hCost;
        }
    }

    public GridTile(Vector2Int gridPosition, float tileSize){
        this.gridPosition = gridPosition;
        worldPosition = new Vector3((gridPosition.x * tileSize) + tileSize / 2, 0, (gridPosition.y * tileSize) + tileSize / 2);
        walkable = !Physics.CheckSphere(worldPosition, (tileSize / 2) - 0.1f, GameManager.Instance.gridManager.unwalkableMask);
    }

    public void ShowOverlay(Color color){
        overlayImage.enabled = true;
        overlayImage.color = color;
    }

    public void HideOverlay(){
        overlayImage.enabled = false;
    }

    public static int Distance(GridTile a, GridTile b){
        int dstX = Mathf.Abs(a.gridPosition.x - b.gridPosition.x);
        int dstY = Mathf.Abs(a.gridPosition.y - b.gridPosition.y);

        if(dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        else
            return 14 * dstX + 10 * (dstY - dstX);
    }

    public static float DiagonalDistance(Vector2 a, Vector2 b){
        float dx = b.x - a.x;
        float dy = b.y - a.y;
        return Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));
    }
}

public class GridManager : MonoBehaviour
{
    public Vector2Int gridSize;
    public float tileSize;
    public GridTile[,] tiles;
    public LayerMask unwalkableMask;
    [SerializeField] GameObject tileOverlayPrefab;
    [SerializeField] Transform tileOverlayParentTransform;

    void Start(){
        InitializeGrid();
    }

    void InitializeGrid(){
        tiles = new GridTile[gridSize.x, gridSize.y];
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                tiles[x, y] = new GridTile(new Vector2Int(x, y), tileSize);
                tiles[x, y].overlayImage = Instantiate(tileOverlayPrefab, tiles[x, y].worldPosition, Quaternion.identity, tileOverlayParentTransform).GetComponentInChildren<Image>();
            }
        }
    }

    public static List<GridTile> GetAdjacentTiles(Vector2Int tileGridPos){
        GridManager gridManager = GameManager.Instance.gridManager;
        GridTile tile = gridManager.tiles[tileGridPos.x, tileGridPos.y];
        List<GridTile> adjacencies = new List<GridTile>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if(x == 0 && y == 0) continue;
                if(x != 0 && y != 0) continue;

                int checkX = tile.gridPosition.x + x;
                int checkY = tile.gridPosition.y + y;

                if(checkX >= 0 && checkX < gridManager.gridSize.x && checkY >= 0 && checkY < gridManager.gridSize.y){
                    adjacencies.Add(gridManager.tiles[checkX, checkY]);
                }
            }
        }
        return adjacencies;
    }

    public static void ShowTileOverlays(List<GridTile> tiles, Color color){
        foreach (GridTile tile in tiles)
        {
            tile.ShowOverlay(color);
        }
    }

    public static void HideTileOverlays(){
        foreach (GridTile tile in GameManager.Instance.gridManager.tiles)
        {
            tile.HideOverlay();
        }
    }

    public Stack<GridTile> path;
    void OnDrawGizmos(){
        if(tiles != null){
            foreach(GridTile tile in tiles){
                Gizmos.color = tile.walkable ? new Color(0, 1, 0, 0.15f) : new Color(1, 0, 0, 0.15f);
                if(path != null) if(path.Contains(tile)) Gizmos.color = new Color(0, 0, 1, 0.15f);
                Gizmos.DrawCube(tile.worldPosition, new Vector3(tileSize, 0.2f, tileSize));
            }
        }
    }
}
