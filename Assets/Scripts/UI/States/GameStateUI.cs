using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class GameStateUI
{
    TMP_Text gameStateDisplay;
    TMP_Text turnStateDisplay;


    public virtual void EnterState(){
        gameStateDisplay = GameObject.Find("Game State Display").GetComponentInChildren<TMP_Text>();
        turnStateDisplay = GameObject.Find("Turn State Display").GetComponentInChildren<TMP_Text>();
    }

    public virtual void Update(){
        GameStateManager gameStateManager = GameManager.Instance.gameStateManager;
        gameStateDisplay.text = gameStateManager.stateMap[gameStateManager.state.Value].stateName;

        ulong currentTurnsPlayerId = GameManager.Instance.turnManager.currentPlayerClientId.Value;

        if(TurnManager.IsMyTurn()) turnStateDisplay.text = "YOUR TURN";
        else turnStateDisplay.text = $"{GameManager.Instance.playerManager.playerDatas[currentTurnsPlayerId].name}'S TURN";
    }

    public virtual void ExitState(){}
}
