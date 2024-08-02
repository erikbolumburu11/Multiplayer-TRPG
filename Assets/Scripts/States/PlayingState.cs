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
    }
}
