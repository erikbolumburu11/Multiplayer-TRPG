using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class JoinUI : NetworkBehaviour
{
    [SerializeField] GameObject joinSettingsObject;
    [SerializeField] NetworkManager networkManager;

    public void HostJoin(){
        networkManager.StartHost();
        joinSettingsObject.SetActive(false);

        GameManager.Instance.gameStateManager.RequestStateChangeRpc(GameStateKey.SETUP);
    }

    public void ClientJoin(){
        networkManager.StartClient();
        joinSettingsObject.SetActive(false);

        GameManager.Instance.gameStateManager.stateMap[GameStateKey.SETUP].EnterState();
        // GameManager.Instance.gameStateManager.RequestStateChangeRpc(GameStateKey.SETUP);
    }
}
