using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UnitBehaviour : NetworkBehaviour
{
    public GridTile occupyingTile;
    public Stack<GridTile> path;
    GridTile tileMovingTo;

    public float speed;

    void Start(){
        if(!IsServer) return;
        occupyingTile = GameManager.Instance.gridManager.tiles[0,0];
    }

    void Update(){
        if(!IsServer) return;
        Move();
        HasReachedNextTile();
    }

    void Move(){
        if(path != null){
            if(tileMovingTo == null && path.Count > 0){
                tileMovingTo = path.Pop();
            }
        }

        if(tileMovingTo != null)
            transform.position = Vector3.MoveTowards(transform.position, tileMovingTo.worldPosition, speed * Time.deltaTime);
    }

    void HasReachedNextTile(){
        if(tileMovingTo == null) return;

        if(Vector3.Distance(transform.position, tileMovingTo.worldPosition) < 0.05f){
            occupyingTile = tileMovingTo;
            if(path.Count > 0) tileMovingTo = path.Pop();
            else tileMovingTo = null;
        }
    }
}
