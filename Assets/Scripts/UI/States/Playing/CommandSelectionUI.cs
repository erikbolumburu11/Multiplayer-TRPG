using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandSelectionUI : MonoBehaviour
{
    public void SelectMovementCommand(){
        if(GameManager.Instance.turnManager.turn.hasMoved) return;
        if(GameManager.Instance.playerInputManager.lockInput) return;
        GameManager.Instance.playerInputManager.SetCommand(Command.MOVE);
    }

    public void SelectBasicAttackCommand(){
        if(GameManager.Instance.turnManager.turn.hasPerformedAction) return;
        if(GameManager.Instance.playerInputManager.lockInput) return;
        GameManager.Instance.playerInputManager.SetCommand(Command.BASIC_ATTACK);
    }

    public void OpenAbilityMenu(){
        UIElements UIElements = GameManager.Instance.UIElements;
        UIElements.abilityMenuUIObject.SetActive(true);
        UIElements.abilityMenuUIObject.GetComponent<AbilitySelectionUI>().InstantiateAbilities();
        UIElements.playingUIObject.SetActive(false);
    }

    public void SelectEndTurn(){
        if(GameManager.Instance.playerInputManager.lockInput) return;
        GameManager.Instance.turnManager.NextTurnRpc();
    }
}
