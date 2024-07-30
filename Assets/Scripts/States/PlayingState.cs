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
}
