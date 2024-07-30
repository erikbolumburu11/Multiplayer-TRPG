using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndState : GameState
{
    protected override void Initialize()
    {
        stateUI = new GameEndStateUI();
        key = GameStateKey.GAME_END;
    }
}