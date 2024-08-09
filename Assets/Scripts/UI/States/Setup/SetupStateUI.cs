using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupStateUI : GameStateUI
{
    
    public override void EnterState()
    {
        base.EnterState();
        uiElements.setupUIObject.SetActive(true);
        GameManager.Instance.playerPartyManager.InstantiateCardsUI();
        
    }

    public override void ExitState()
    {
        base.ExitState();
        uiElements.setupUIObject.SetActive(false);
    }

    public override void Update()
    {
        base.Update();
        if(TurnManager.IsMyTurn()){
            GameManager.Instance.UIElements.unitCards.SetActive(true);
        }
        else {
            GameManager.Instance.UIElements.unitCards.SetActive(false);
        }
    }
}
