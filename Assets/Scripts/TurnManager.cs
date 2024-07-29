using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TurnManager : NetworkBehaviour
{
    public NetworkVariable<ulong> currentPlayerClientId = new();

    void Awake(){
        currentPlayerClientId.Value = 0;
        if(!IsServer) return;
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.B)) 
            Debug.Log("Breakmpoint");
    }

    public void NextTurn(){
        if(!IsOwner) return;
        if(!IsServer) return;
        currentPlayerClientId.Value++;
        int connectedPlayerCount = NetworkManager.Singleton.ConnectedClientsList.Count;
        if((int)currentPlayerClientId.Value >= connectedPlayerCount) currentPlayerClientId.Value = 0;
    }
}
