using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UnitTurnListUI : MonoBehaviour
{
    [SerializeField] Transform listGridObject;
    [SerializeField] GameObject portraitPrefab;
    [SerializeField] ulong portraitCount;

    List<GameObject> spawnedPortraits = new();

    public void UpdateDisplay(){

        foreach (GameObject obj in spawnedPortraits)
        {
            Destroy(obj);
        }

        spawnedPortraits.Clear();

        TurnManager turnManager = GameManager.Instance.turnManager;
        UnitManager unitManager = GameManager.Instance.unitManager;

        ulong playerId = turnManager.currentPlayerClientId.Value;
        Dictionary<ulong, int> playerDisplayedUnit = new(turnManager.playersCurrentUnitMap);
        Dictionary<ulong, List<GameObject>> playerUnitMap = unitManager.playerUnitMap;

        playerId++;
        if(playerId >= (ulong)GameManager.Instance.playerManager.playerDatas.Count){
            playerId = 0;
        }

        // playerDisplayedUnit[playerId]++;

        for (ulong i = 0; i < portraitCount; i++)
        {
            playerId++;
            if(playerId >= (ulong)GameManager.Instance.playerManager.playerDatas.Count){
                playerId = 0;
            }

            if(playerDisplayedUnit[playerId] >= unitManager.playerUnitMap[playerId].Count) playerDisplayedUnit[playerId] = 0;

            UnitBehaviour displayedUnitsBehaviour = unitManager.playerUnitMap[playerId][playerDisplayedUnit[playerId]]
                .GetComponent<UnitBehaviour>();

            playerDisplayedUnit[playerId]++;

            GameObject portraitObject = Instantiate(portraitPrefab, listGridObject);
            UnitPortraitUI portraitComponent = portraitObject.GetComponent<UnitPortraitUI>();

            portraitComponent.SetPortrait(
                playerId,
                displayedUnitsBehaviour.unitData.portraitSprite
            );

            spawnedPortraits.Add(portraitObject);
        }
    }
}
