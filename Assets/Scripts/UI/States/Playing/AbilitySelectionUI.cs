using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AbilitySelectionUI : MonoBehaviour
{
    [SerializeField] Transform abilityListParent;
    [SerializeField] GameObject abilityUIPrefab;
    List<GameObject> abilityUIItems;
    void Awake(){
        abilityUIItems = new();
    }

    void Update(){
        Ability selectedAbility = UnitManager.GetSelectedUnitBehaviour().selectedAbility;
        TileOverlayLayer rangeIndicatorLayer = GameManager.Instance.gridManager.tileOverlayLayersMap[TileOverlayLayerID.RANGE_INDICATOR];
        TileOverlayLayer affectedTilesLayer = GameManager.Instance.gridManager.tileOverlayLayersMap[TileOverlayLayerID.AFFECTED_TILES];

        if(selectedAbility == null || 
            PlayerInputManager.GetHoveredTile() == null ||
            !PlayerInputManager.GetHoveredTile().walkable ||
            !rangeIndicatorLayer.highlightedTiles.Contains(PlayerInputManager.GetHoveredTile()))
        {
            GameManager.Instance.gridManager.tileOverlayLayersMap[TileOverlayLayerID.AFFECTED_TILES].HideTileOverlays();
            return;
        } 

        if(!selectedAbility.requiresTargetUnit){
            List<GridTile> inRangeTiles = Visibility.GetVisibleTilesFromList(
                PlayerInputManager.GetHoveredTile(),
                RangeFinder.GetTilesInRangeFromTile(
                    PlayerInputManager.GetHoveredTile(),
                    selectedAbility.effectRange,
                    true
                )
            ); 

            affectedTilesLayer.HideTileOverlays();
            affectedTilesLayer.color = Color.red;
            affectedTilesLayer.highlightedTiles = inRangeTiles;
            affectedTilesLayer.ShowTileOverlays();
        }
    }

    public void InstantiateAbilities(){
        foreach (GameObject abilityUIItem in abilityUIItems)
        {
            Destroy(abilityUIItem);            
        }
        abilityUIItems.Clear();

        foreach (Ability ability in GameManager.Instance.unitManager.selectedUnit.GetComponent<UnitBehaviour>().unitData.abilities)
        {
            GameObject abilityUIItem = Instantiate(abilityUIPrefab, abilityListParent);
            abilityUIItem.GetComponentInChildren<TMP_Text>().text = ability.name;
            abilityUIItem.GetComponent<AbilityUIItem>().ability = ability;
            abilityUIItems.Add(abilityUIItem);
        }
    }

    public void BackToCommandSelection(){
        GridManager.HideAllTileOverlays();
        GameManager.Instance.UIElements.setupUIObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
