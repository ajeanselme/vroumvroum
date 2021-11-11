using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEvent : MonoBehaviour
{
    public enum EEventState
    {
        None,
        Triggered,
        Used
    }

    protected EEventState eventState = EEventState.None;
    
    [SerializeField] protected MapEventTrigger mapEventTrigger;

    protected virtual void Start()
    {
        mapEventTrigger.Initialize(this);
    }

    public virtual void PlayerEnterTrigger()
    {
        if (eventState == EEventState.None)
        {
            eventState = EEventState.Triggered;
            // Launch QTE
        }
    }
    
    public virtual void PlayerExitTrigger()
    {
        if (eventState == EEventState.Triggered)
            eventState = EEventState.None;
    }

    public virtual void EventSuccess()
    {
        eventState = EEventState.Used;
    }
}
