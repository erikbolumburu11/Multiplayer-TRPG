using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class PlayerPartyManager : MonoBehaviour
{
    public List<UnitData> unitHand;
    public UnitData selectedUnitCard;

    [SerializeField] GameObject unitPlacementPreviewPrefab;
    GameObject instantiatedUnitPlacementPreview;

    void Update(){
        if(selectedUnitCard == null && instantiatedUnitPlacementPreview != null) Destroy(instantiatedUnitPlacementPreview);

        if(selectedUnitCard != null && instantiatedUnitPlacementPreview == null){
            instantiatedUnitPlacementPreview = Instantiate(unitPlacementPreviewPrefab);
        }

        if(selectedUnitCard != null && instantiatedUnitPlacementPreview != null){
            if(PlayerInputManager.GetHoveredTile() != null){
                instantiatedUnitPlacementPreview.transform.position = PlayerInputManager.GetHoveredTile().worldPosition;
            }
        }

        if(Input.GetMouseButtonDown(0) && 
            selectedUnitCard != null && 
            PlayerInputManager.GetHoveredTile() != null &&
            PlayerInputManager.GetHoveredTile().walkable
        ){
            GameManager.Instance.unitManager.SpawnUnitRpc(selectedUnitCard.prefabResourceDir, PlayerInputManager.GetHoveredTile().gridPosition);
            unitHand.Remove(selectedUnitCard);
            GameManager.Instance.UIElements.unitCards.GetComponent<UnitCardList>().RemoveCard(selectedUnitCard);
            selectedUnitCard = null;
        }
    }

    public void InstantiateCardsUI(){
        UIElements uiElements = GameManager.Instance.UIElements;
        foreach (UnitData unit in GameManager.Instance.playerPartyManager.unitHand)
        {
            GameObject card = Instantiate(uiElements.unitCardPrefab, uiElements.unitCards.transform);
            card.GetComponent<UnitCard>().unitNameObject.text = unit.unitName;
            card.GetComponent<UnitCard>().unitData = unit;
            uiElements.unitCards.GetComponent<UnitCardList>().cards.Add(card.GetComponent<UnitCard>());
        }
    }
}
