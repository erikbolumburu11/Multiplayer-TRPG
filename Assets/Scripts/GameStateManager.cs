using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum GameStateKey {
    SETUP,
    PLAYING,
    GAME_END
}

public class GameStateManager : NetworkBehaviour { 
    public NetworkVariable<GameStateKey> state;
    public Dictionary<GameStateKey, GameState> stateMap;
    bool firstStateChange = true;

    void Start(){
        if(IsServer) state = new();
        stateMap = new();


        SetupState setupState = GetComponent<SetupState>();
        stateMap.Add(GameStateKey.SETUP, setupState);

        PlayingState playingState = GetComponent<PlayingState>();
        stateMap.Add(GameStateKey.PLAYING, playingState);

        GameEndState gameEndState = GetComponent<GameEndState>();
        stateMap.Add(GameStateKey.GAME_END, gameEndState);

        if(!IsServer) return;
    }

    void Update(){
        stateMap[state.Value].Update();
    }

    [Rpc(SendTo.Server)]
    public void RequestStateChangeRpc(GameStateKey gameStateKey, RpcParams rpcParams = default){
        ChangeStateOnClientsRpc(gameStateKey);
        state.Value = gameStateKey;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ChangeStateOnClientsRpc(GameStateKey gameStateKey, RpcParams rpcParams = default){
        if(!firstStateChange) stateMap[state.Value].ExitState();
        firstStateChange = true;
        stateMap[gameStateKey].EnterState();
    }
}