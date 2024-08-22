using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public enum Command {
    NONE,
    MOVE,
    BASIC_ATTACK,
    CAST_ABILITY
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
        // if(!IsOwner || !IsSpawned) return;
        if(GameStateManager.CompareCurrentState(GameStateKey.PLAYING)){

            if(UnitManager.GetSelectedUnitBehaviour().isMoving.Value || 
                UnitManager.GetSelectedUnitBehaviour().isPerformingAction.Value)
            {
                SetCommand(Command.NONE);
                GridManager.HideAllTileOverlays();
                return;
            }
            else if(Input.GetMouseButtonDown(0)){
                switch(selectedCommand){
                    case Command.MOVE: Move(); break;
                    case Command.BASIC_ATTACK: BasicAttack(); break;
                    case Command.CAST_ABILITY: CastAbility(); break;
                }
                SetCommand(Command.NONE);
            }
        }
    }

    public void SetCommand(Command newCommand){
        if(unitManager.selectedUnit == null) return;

        if(UnitManager.GetSelectedUnitBehaviour().isMoving.Value || 
            UnitManager.GetSelectedUnitBehaviour().isPerformingAction.Value)
        {
            GridManager.HideAllTileOverlays();
            return;
        } 

        Vector2Int unitTilePos = UnitManager.GetSelectedUnitBehaviour().occupyingTile.Value;
        GridTile unitTile = gridManager.tiles[unitTilePos.x, unitTilePos.y];

        TileOverlayLayer rangeLayer = gridManager.tileOverlayLayersMap[TileOverlayLayerID.RANGE_INDICATOR];
        TileOverlayLayer affectedTilesLayer = gridManager.tileOverlayLayersMap[TileOverlayLayerID.AFFECTED_TILES];

        GridManager.HideAllTileOverlays();

        switch(newCommand){
            case Command.NONE: break;

            case Command.MOVE: 
                rangeLayer.color = Color.green;
                rangeLayer.highlightedTiles = RangeFinder.GetWalkableTilesInRange(
                    unitTile,
                    UnitManager.GetSelectedUnitBehaviour().unitData.moveRange,
                    false
                );
                rangeLayer.ShowTileOverlays(); 
                break;

            case Command.BASIC_ATTACK:
                List<GridTile> attackableTiles = Visibility.GetVisibleTilesFromList(
                    unitTile,
                    RangeFinder.GetAttackableTilesInRange(
                        unitTile,
                        UnitManager.GetSelectedUnitBehaviour().unitData.basicAttackRange,
                        false
                    )
                ); 
                rangeLayer.color = Color.red;
                rangeLayer.highlightedTiles = attackableTiles;
                rangeLayer.ShowTileOverlays();
                break;
        }

        selectedCommand = newCommand;
    }

    private void Move()
    {
        if(GetHoveredTile() == null) return;
        if(!TurnManager.IsMyTurn()) return;

        Vector2Int unitTilePos = UnitManager.GetSelectedUnitBehaviour().occupyingTile.Value;
        GridTile unitTile = gridManager.tiles[unitTilePos.x, unitTilePos.y];

        if(!RangeFinder.GetWalkableTilesInRange(
            unitTile,
            UnitManager.GetSelectedUnitBehaviour().unitData.moveRange,
            false
        ).Contains(GetHoveredTile())) return;

        GameManager.Instance.unitManager.MoveSelectedUnitRpc(GetHoveredTile().gridPosition);
    }

    private void BasicAttack()
    {
        if(GetHoveredTile() == null) return;
        if(!TurnManager.IsMyTurn()) return;

        Vector2Int unitTilePos = UnitManager.GetSelectedUnitBehaviour().occupyingTile.Value;
        GridTile unitTile = gridManager.tiles[unitTilePos.x, unitTilePos.y];
        List<GridTile> attackableTiles = Visibility.GetVisibleTilesFromList(
            unitTile,
            RangeFinder.GetAttackableTilesInRange(
                unitTile,
                UnitManager.GetSelectedUnitBehaviour().unitData.basicAttackRange,
                false
            )
        ); 
        if(!attackableTiles.Contains(GetHoveredTile())) return;

        GameManager.Instance.unitManager.BasicAttackRpc(GetHoveredTile().gridPosition);
    }

    private void CastAbility(){
        if(GetHoveredTile() == null) return;
        if(!TurnManager.IsMyTurn()) return;

        UnitBehaviour unitBehaviour = UnitManager.GetSelectedUnitBehaviour();

        if(unitBehaviour.selectedAbility == null) return;

        List<GridTile> targetableTiles = Visibility.GetVisibleTilesFromList(
            GridManager.GetTileAtVector2Int(unitBehaviour.occupyingTile.Value),
            RangeFinder.GetAttackableTilesInRange(
                GridManager.GetTileAtVector2Int(unitBehaviour.occupyingTile.Value),
                unitBehaviour.selectedAbility.castRange,
                unitBehaviour.selectedAbility.canTargetCaster
            )
        ); 

        if(!targetableTiles.Contains(GetHoveredTile())) return;

        GridManager.HideAllTileOverlays();
        GameManager.Instance.unitManager.CastAbilityRpc(GetHoveredTile().gridPosition, unitBehaviour.selectedAbility.path);
        UnitManager.GetSelectedUnitBehaviour().selectedAbility = null;

        GameManager.Instance.UIElements.abilityMenuUIObject.GetComponent<AbilitySelectionUI>().BackToCommandSelection();
    }

    [Rpc(SendTo.Everyone)]
    public void InitializePlayerRpc(RpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        if(!GameManager.Instance.unitManager.playerUnitMap.ContainsKey(clientId)){

            GameManager.Instance.unitManager.playerUnitMap.Add(clientId, new());
            GameManager.Instance.playerManager.playerDatas.Add(clientId, new(clientId, (int)clientId % 2 == 0 ? Team.TEAM_ONE : Team.TEAM_TWO));
            GameManager.Instance.turnManager.playersCurrentUnitMap.Add(clientId, -1);

        }
    }

    public static GridTile GetHoveredTile(){
        GridManager gridManager = GameManager.Instance.gridManager;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        int groundlayerMask = GameManager.Instance.groundLayer;

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundlayerMask)) {

            Vector2Int hoveredTilePos = new Vector2Int(
                Mathf.FloorToInt(hit.point.x / (int)gridManager.tileSize),
                Mathf.FloorToInt(hit.point.z / (int)gridManager.tileSize)
            );

            return gridManager.tiles[hoveredTilePos.x, hoveredTilePos.y];
        }
        else return null;
    }
}
