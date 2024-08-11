using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityUIItem : MonoBehaviour
{
    public Ability ability;

    public void SelectAbility(){
        GameManager.Instance.playerInputManager.SetCommand(Command.CAST_ABILITY);

        Vector2Int unitsTilePos = UnitManager.GetSelectedUnitBehaviour().occupyingTile.Value;
        GridTile unitsTile = GameManager.Instance.gridManager.tiles[unitsTilePos.x, unitsTilePos.y];
        UnitManager.GetSelectedUnitBehaviour().selectedAbility = ability;

        List<GridTile> attackableTiles = Visibility.GetVisibleTilesFromList(
            unitsTile,
            RangeFinder.GetAttackableTilesInRange(
                unitsTile,
                ability.castRange,
                ability.canTargetCasterTile
            )
        ); 

        TileOverlayLayer layer = GameManager.Instance.gridManager.tileOverlayLayersMap[TileOverlayLayerID.RANGE_INDICATOR];
        layer.color = ability.requiresTargetUnit ? Color.red : Color.white;
        layer.highlightedTiles = attackableTiles;
        layer.ShowTileOverlays();
    }
}
