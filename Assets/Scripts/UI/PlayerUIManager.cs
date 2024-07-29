using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerUIManager : NetworkBehaviour
{
    TMP_Text gameStateDisplay;
    TMP_Text turnStateDisplay;

    void Awake(){
        gameStateDisplay = GameObject.Find("Game State Display").GetComponentInChildren<TMP_Text>();
        turnStateDisplay = GameObject.Find("Turn State Display").GetComponentInChildren<TMP_Text>();
    }

    void Update(){
        switch (GameManager.Instance.gameStateManager.state.Value){
            case GameState.SETUP:
                SetupUpdate();
                break;
            case GameState.PLAYING:
                PlayingUpdate();
                break;
            case GameState.GAME_OVER:
                GameOverUpdate();
                break;

        }
    }

    void SetupUpdate(){
        gameStateDisplay.text = "SETUP";
        
        ulong currentTurnsPlayerId = GameManager.Instance.turnManager.currentPlayerClientId.Value;
        if(currentTurnsPlayerId == NetworkManager.Singleton.LocalClientId) turnStateDisplay.text = "YOUR TURN";
        else turnStateDisplay.text = $"{GameManager.Instance.playerManager.playerDatas[currentTurnsPlayerId].name}'S TURN";
    }

    void PlayingUpdate(){
        gameStateDisplay.text = "GAME";
    }

    void GameOverUpdate(){
        gameStateDisplay.text = "GAME OVER";
    }
}
