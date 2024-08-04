using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingStateUI : GameStateUI
{
    public override void EnterState()
    {
        base.EnterState();
        uiElements.playingUIObject.SetActive(true);
    }

    public override void ExitState()
    {
        base.ExitState();
        uiElements.playingUIObject.SetActive(false);
    }

    public override void Update()
    {
        base.Update();
    }
}
