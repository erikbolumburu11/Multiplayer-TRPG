using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitHealthBar : MonoBehaviour
{
    Slider slider;
    UnitBehaviour unitBehaviour;

    void Start(){
        slider = GetComponent<Slider>();
        unitBehaviour = GetComponentInParent<UnitBehaviour>();
    }

    void Update(){
        float maxHealth = unitBehaviour.unitData.maxHealth;
        float currentHealth = unitBehaviour.unitStats.health.Value;
        float sliderFillAmount = currentHealth / maxHealth;
        slider.value = sliderFillAmount;
    }
}