using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerManager : NetworkBehaviour
{
    GridManager gridManager;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] GameObject unitPrefab;

    void Awake(){
        gridManager = GameManager.Instance.gridManager;
    }

    public override void OnNetworkSpawn(){
        base.OnNetworkSpawn();
        if(!IsOwner) return;
        SpawnUnitServerRpc();
    }

    void Update(){
        if(!IsOwner) return;

        if(GetHoveredTile() != null){
            if(Input.GetKeyDown(KeyCode.M)) MoveUnitServerRpc(GetHoveredTile().gridPosition);
        }
    }


    [Rpc(SendTo.Server)]
    void MoveUnitServerRpc(Vector2Int targetTilePos, RpcParams rpcParams = default) {
        UnitManager unitManager = GameManager.Instance.unitManager;
        GameObject unit = unitManager.playerDatas[(int)rpcParams.Receive.SenderClientId].unit;
        UnitBehaviour unitBehaviour = unit.GetComponent<UnitBehaviour>();
        unitBehaviour.path = GameManager.Instance.pathfinder.FindPath(unitBehaviour.occupyingTile, gridManager.tiles[targetTilePos.x, targetTilePos.y]);
    }

    [Rpc(SendTo.Server)]
    void SpawnUnitServerRpc(RpcParams rpcParams = default){
        int clientId = (int)rpcParams.Receive.SenderClientId;
        UnitManager unitManager = GameManager.Instance.unitManager;
        unitManager.playerDatas.Add(clientId, new());

        GameObject unit = Instantiate(unitPrefab);
        NetworkObject unitNetworkObject = unit.GetComponent<NetworkObject>();
        unitNetworkObject.Spawn();

        unitManager.playerDatas[clientId].clientId = clientId;
        unitManager.playerDatas[clientId].unit = unit;
    }

    public GridTile GetHoveredTile(){
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, groundLayer)) {
            Vector2Int hoveredTilePos = new Vector2Int((int)Mathf.Floor(hit.point.x), (int)Mathf.Floor(hit.point.z));
            return gridManager.tiles[hoveredTilePos.x, hoveredTilePos.y];
        }
        else return null;
    }
}
