using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupState : GameState
{
    protected override void Initialize()
    {
        stateUI = new SetupStateUI();
        key = GameStateKey.SETUP;
    }

}