using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class SetupState : GameState
{
    bool spawnzonesShown = false;
    protected override void Initialize()
    {
        stateUI = new SetupStateUI();
        key = GameStateKey.SETUP;
    }

    public override void Update(){
        if(GameManager.Instance.gameStateManager.state.Value != key) return;
        base.Update();

        if(!spawnzonesShown && GameManager.Instance.playerManager.playerDatas.ContainsKey(NetworkManager.Singleton.LocalClientId)){
            GameManager.Instance.gridManager.ShowSpawnZones();
        }

        if(!IsServer) return;

        GameManager gameManager = GameManager.Instance;
        gameManager.playerPartyManager.IsClientsHandEmptyRpc(RpcTarget.Single(gameManager.turnManager.currentPlayerClientId.Value, RpcTargetUse.Temp));
    }

}