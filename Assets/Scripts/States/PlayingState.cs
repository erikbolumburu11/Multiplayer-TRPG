using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingState : GameState
{
    protected override void Initialize()
    {
        stateUI = new PlayingStateUI();
        key = GameStateKey.PLAYING;
    }

    public override void Update()
    {
        if(GameManager.Instance.gameStateManager.state.Value != key) return;
        base.Update();

        if(TurnManager.IsMyTurn()){
            GameManager.Instance.UIElements.playingUIObject.SetActive(true);
        }
        else{
            GameManager.Instance.UIElements.playingUIObject.SetActive(false);
        }

        if(!IsServer) return;

        TurnManager turnManager = GameManager.Instance.turnManager;
        PlayerInputManager playerInputManager = GameManager.Instance.playerInputManager;

        if(turnManager.turn.hasPerformedAction &&
           turnManager.turn.hasMoved &&
           !playerInputManager.lockInput)
        {
            turnManager.NextTurnRpc();
        }
    }
}
