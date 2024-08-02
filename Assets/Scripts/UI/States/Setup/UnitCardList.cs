using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCardList : MonoBehaviour
{
    public List<UnitCard> cards;

    public void RemoveCard(UnitData cardDataToRemove){
        for (int i = 0; i < cards.Count; i++)
        {
            if(cards[i].unitData == cardDataToRemove){
                Destroy(cards[i].gameObject);
                cards.Remove(cards[i]);
                break;
            } 
        }
    }
}
