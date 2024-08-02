using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UnitManager : NetworkBehaviour
{
    public Dictionary<ulong, List<GameObject>> playerUnitMap;
    public GameObject selectedUnit;

    void Awake(){
        playerUnitMap = new();
    }

    [Rpc(SendTo.Server)]
    public void SpawnUnitRpc(string prefabResourceDir, Vector2Int spawnGridPos, RpcParams rpcParams = default){
        ulong clientId = rpcParams.Receive.SenderClientId;
        UnitManager unitManager = GameManager.Instance.unitManager;
        GridTile tile = GameManager.Instance.gridManager.tiles[spawnGridPos.x, spawnGridPos.y];

        GameObject unit;
        GameObject unitPrefab = Resources.Load(prefabResourceDir, typeof(GameObject)) as GameObject;
        unit = Instantiate(unitPrefab, GridWorldPosToGameObjectPos(tile.worldPosition, unitPrefab), Quaternion.identity);
        unit.GetComponent<UnitBehaviour>().occupyingTile = tile;
        unit.GetComponent<UnitBehaviour>().ownerClientId = new(){Value = clientId};
        NetworkObject unitNetworkObject = unit.GetComponent<NetworkObject>();
        unitNetworkObject.Spawn();

        AddUnitToPlayerUnitMapRpc(unit, clientId);

        GameManager.Instance.turnManager.NextTurnRpc();
    }

    [Rpc(SendTo.Everyone)]
    public void AddUnitToPlayerUnitMapRpc(NetworkObjectReference networkObjectReference, ulong clientId, RpcParams rpcParams = default){
        GameManager.Instance.unitManager.playerUnitMap[clientId].Add(networkObjectReference);
    }

    [Rpc(SendTo.Server)]
    public void MoveSelectedUnitRpc(Vector2Int targetTilePos, RpcParams rpcParams = default) {
        UnitBehaviour unitBehaviour = TurnManager.GetCurrentTurnsUnit().GetComponent<UnitBehaviour>();
        Stack<GridTile> path = GameManager.Instance.pathfinder.FindPath(unitBehaviour.occupyingTile, GameManager.Instance.gridManager.tiles[targetTilePos.x, targetTilePos.y]);
        unitBehaviour.path = path;
        if(path != null) GameManager.Instance.turnManager.NextTurnRpc();
    }

    // Adds half the height of the GameObject so it isn't half way in the ground
    public static Vector3 GridWorldPosToGameObjectPos(Vector3 gridWorldPos, GameObject gameObject){
        return new Vector3(
            gridWorldPos.x,
            gridWorldPos.y + (gameObject.transform.localScale.y / 2),
            gridWorldPos.z
        );
    }

}