using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public enum Command {
    NONE,
    MOVE,
    BASIC_ATTACK
}

public class PlayerInputManager : NetworkBehaviour
{
    public Command selectedCommand;
    GridManager gridManager;
    UnitManager unitManager;

    public bool lockInput;

    void Start(){
        gridManager = GameManager.Instance.gridManager;
        unitManager = GameManager.Instance.unitManager;
    }

    public override void OnNetworkSpawn(){
        base.OnNetworkSpawn();
        InitializePlayerRpc();
        if(!IsOwner) return;
    }


    void Update(){
        if(lockInput){
            SetCommand(Command.NONE);
            return;
        }

        if(Input.GetMouseButtonDown(0)){
            switch(selectedCommand){
                case Command.MOVE: Move(); break;
                case Command.BASIC_ATTACK: BasicAttack(); break;
            }
            SetCommand(Command.NONE);
        }
    }

    public void SetCommand(Command newCommand){
        if(unitManager.selectedUnit == null) return;
        Vector2Int unitTilePos = unitManager.selectedUnit.GetComponent<UnitBehaviour>().occupyingTile.Value;
        GridTile unitTile = gridManager.tiles[unitTilePos.x, unitTilePos.y];

        switch(selectedCommand){
            case Command.NONE: GridManager.HideTileOverlays(); break;
            case Command.MOVE: GridManager.HideTileOverlays(); break;
            case Command.BASIC_ATTACK: GridManager.HideTileOverlays(); break;
        }

        switch(newCommand){
            case Command.NONE: break;

            case Command.MOVE: 
                GridManager.ShowTileOverlays(
                    RangeFinder.GetWalkableTilesInRange(
                        unitTile,
                        unitManager.selectedUnit.GetComponent<UnitBehaviour>().unitData.moveRange,
                        false
                    ), 
                    Color.green
                ); 
                break;

            case Command.BASIC_ATTACK:
                List<GridTile> attackableTiles = Visibility.GetVisibleTilesFromList(
                    unitTile,
                    RangeFinder.GetAttackableTilesInRange(
                        unitTile,
                        unitManager.selectedUnit.GetComponent<UnitBehaviour>().unitData.basicAttackRange,
                        false
                    )
                ); 
                GridManager.ShowTileOverlays(attackableTiles, Color.red);
                break;
        }

        selectedCommand = newCommand;
    }

    private void Move()
    {
        if(GetHoveredTile() == null) return;
        if(!TurnManager.IsMyTurn()) return;

        Vector2Int unitTilePos = unitManager.selectedUnit.GetComponent<UnitBehaviour>().occupyingTile.Value;
        GridTile unitTile = gridManager.tiles[unitTilePos.x, unitTilePos.y];

        if(!RangeFinder.GetWalkableTilesInRange(
                unitTile,
                unitManager.selectedUnit.GetComponent<UnitBehaviour>().unitData.moveRange,
                false
            ).Contains(GetHoveredTile())) return;

        GameManager.Instance.unitManager.MoveSelectedUnitRpc(GetHoveredTile().gridPosition);
    }

    private void BasicAttack()
    {
        if(GetHoveredTile() == null) return;
        if(!TurnManager.IsMyTurn()) return;

        Vector2Int unitTilePos = unitManager.selectedUnit.GetComponent<UnitBehaviour>().occupyingTile.Value;
        GridTile unitTile = gridManager.tiles[unitTilePos.x, unitTilePos.y];
        List<GridTile> attackableTiles = Visibility.GetVisibleTilesFromList(
            unitTile,
            RangeFinder.GetAttackableTilesInRange(
                unitTile,
                unitManager.selectedUnit.GetComponent<UnitBehaviour>().unitData.basicAttackRange,
                false
            )
        ); 
        if(!attackableTiles.Contains(GetHoveredTile())) return;

        GameManager.Instance.unitManager.BasicAttackRpc(GetHoveredTile().gridPosition);
    }

    [Rpc(SendTo.Everyone)]
    public void InitializePlayerRpc(RpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        GameManager.Instance.unitManager.playerUnitMap.Add(clientId, new());
        GameManager.Instance.playerManager.playerDatas.Add(clientId, new(clientId));
    }

    public static GridTile GetHoveredTile(){
        GridManager gridManager = GameManager.Instance.gridManager;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, GameManager.Instance.groundLayer)) {
            Vector2Int hoveredTilePos = new Vector2Int((int)Mathf.Floor(hit.point.x) / (int)gridManager.tileSize, (int)Mathf.Floor(hit.point.z) / (int)gridManager.tileSize);
            return gridManager.tiles[hoveredTilePos.x, hoveredTilePos.y];
        }
        else return null;
    }
}
