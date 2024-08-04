using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandSelectionUI : MonoBehaviour
{
    public void SelectMovementCommand(){
        GameManager.Instance.playerInputManager.SetCommand(Command.MOVE);
    }

    public void SelectBasicAttackCommand(){
        GameManager.Instance.playerInputManager.SetCommand(Command.BASIC_ATTACK);
    }
}
