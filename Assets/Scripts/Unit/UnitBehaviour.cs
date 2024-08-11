using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UnitBehaviour : NetworkBehaviour
{
    public NetworkVariable<Vector2Int> occupyingTile;
    public Stack<GridTile> path;
    GridTile tileMovingTo;
    public UnitData unitData;
    public UnitStats unitStats;
    public NetworkVariable<ulong> ownerClientId;
    public NetworkVariable<bool> isPerformingAction;
    [SerializeField] Image teamIndicator;
    [SerializeField] Image healthBarFill;
    public GameObject selectionDecal;
    Animator animator;
    public ClientAuthNetworkAnimator clientAuthNetworkAnimator;
    public Ability selectedAbility;

    void Start(){
        unitStats = GetComponent<UnitStats>();
        animator = GetComponent<Animator>();
        clientAuthNetworkAnimator = GetComponent<ClientAuthNetworkAnimator>();
        if(!IsServer) return;
        unitStats.health.Value = unitData.maxHealth;
    }

    void SetPlayerIndicatorColor(){
        if(NetworkManager.Singleton.LocalClientId == ownerClientId.Value){
            teamIndicator.color = Color.green;
            healthBarFill.color = Color.green;
        }
        else {
            teamIndicator.color = Color.red;
            healthBarFill.color = Color.red;
        }
    }

    void Update()
    {
        if (GameStateManager.CompareCurrentState(GameStateKey.PLAYING)) SetSelectionDecalVisibility();
        SetPlayerIndicatorColor(); 

        LockInput();

        if (!IsServer) return;

        if (unitStats.health.Value <= 0) UnitDeathRpc();
        Move();
        HasReachedNextTile();
        SetAnimatorParameters();
    }

    private void LockInput()
    {
        if (ownerClientId.Value == NetworkManager.Singleton.LocalClientId)
        {
            // Is Unit Moving
            if ((path != null && path.Count > 0) || tileMovingTo != null || isPerformingAction.Value) GameManager.Instance.playerInputManager.lockInput = true;
            else GameManager.Instance.playerInputManager.lockInput = false;

        }
    }

    [Rpc(SendTo.Everyone)]
    public void UnitDeathRpc(){
        GameManager.Instance.unitManager.playerUnitMap[ownerClientId.Value].Remove(gameObject);
        if(!IsServer) return;
        GetComponent<NetworkObject>().Despawn();
    }

    private void SetAnimatorParameters()
    {
        animator.SetBool("IsMoving", path != null && path.Count != 0);
    }

    void SetSelectionDecalVisibility(){
        if(UnitManager.GetSelectedUnit() == gameObject) selectionDecal.SetActive(true);
        else selectionDecal.SetActive(false);
    }

    void Move(){
        if(path != null){
            if(tileMovingTo == null && path.Count > 0){
                tileMovingTo = path.Pop();
            }
        }

        if(tileMovingTo != null){
            transform.position = Vector3.MoveTowards(transform.position,
                tileMovingTo.worldPosition,
                unitData.speed * Time.deltaTime
            );
            transform.LookAt(tileMovingTo.worldPosition, Vector3.up);
        }
    }

    void HasReachedNextTile(){
        if(tileMovingTo == null) return;

        if(Vector3.Distance(transform.position, tileMovingTo.worldPosition) < 0.05f){
            occupyingTile.Value = tileMovingTo.gridPosition;
            if(path.Count > 0) tileMovingTo = path.Pop();
            else tileMovingTo = null;
        }
    }
}
