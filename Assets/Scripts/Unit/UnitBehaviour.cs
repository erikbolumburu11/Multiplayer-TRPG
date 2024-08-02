using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UnitBehaviour : NetworkBehaviour
{
    public GridTile occupyingTile;
    public Stack<GridTile> path;
    GridTile tileMovingTo;
    public UnitData unitData;
    public NetworkVariable<ulong> ownerClientId;
    Image playerIndicator;
    public GameObject selectionDecal;

    void Start(){
        SetPlayerIndicatorColor(); 
        if(!IsServer) return;
    }

    void SetPlayerIndicatorColor(){
        playerIndicator = GetComponentInChildren<Image>();
        if(NetworkManager.Singleton.LocalClientId == ownerClientId.Value) playerIndicator.color = Color.green;
        else playerIndicator.color = Color.red;
    }

    void Update(){
        SetSelectionDecalVisibility();
        if(!IsServer) return;
        Move();
        HasReachedNextTile();
    }

    void SetSelectionDecalVisibility(){
        if(GameManager.Instance.unitManager.selectedUnit == gameObject) selectionDecal.SetActive(true);
        else selectionDecal.SetActive(false);
    }

    void Move(){
        if(path != null){
            if(tileMovingTo == null && path.Count > 0){
                tileMovingTo = path.Pop();
            }
        }

        if(tileMovingTo != null)
            transform.position = Vector3.MoveTowards(transform.position,
                UnitManager.GridWorldPosToGameObjectPos(tileMovingTo.worldPosition, gameObject),
                unitData.speed * Time.deltaTime
            );
    }

    void HasReachedNextTile(){
        if(tileMovingTo == null) return;

        if(Vector3.Distance(transform.position, UnitManager.GridWorldPosToGameObjectPos(tileMovingTo.worldPosition, gameObject)) < 0.05f){
            occupyingTile = tileMovingTo;
            if(path.Count > 0) tileMovingTo = path.Pop();
            else tileMovingTo = null;
        }
    }
}
