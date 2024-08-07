using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionOnTimer : MonoBehaviour
{
    [SerializeField] private string name = "Timer";
    [SerializeField] private float timer;

    private Action timerCallback;

    public void SetTimer(float timer, Action timerCallback) {
        this.timer = timer;
        this.timerCallback = timerCallback;
    }

    private void Update() {
        if(IsRunning()) {
            timer -= Time.deltaTime;

            if(timer <= 0) timerCallback();
        }
    }

    public bool IsRunning(){
        return timer > 0;
    }

    public static ActionOnTimer GetTimer(GameObject obj, string name){
        ActionOnTimer[] timers = obj.GetComponents<ActionOnTimer>();
        foreach (ActionOnTimer timer in timers)
        {
            if(timer.name == name) return timer;
        }
        ActionOnTimer newTimer = obj.AddComponent<ActionOnTimer>();
        newTimer.name = name;
        return newTimer;
    }
}