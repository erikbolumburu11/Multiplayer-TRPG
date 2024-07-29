using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum GameState {
    SETUP,
    PLAYING,
    GAME_OVER 
}

public class GameStateManager : NetworkBehaviour { 
    public NetworkVariable<GameState> state;

    void Awake(){
        if(!IsServer) return;
        ChangeState(GameState.SETUP);
    }

    public void ChangeState(GameState state){
        this.state.Value = state;
        Debug.Log($"State changed to: {state}");
    }
}