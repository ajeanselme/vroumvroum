using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEventTrigger : MonoBehaviour
{
    private MapEvent mapEvent;

    public void Initialize(MapEvent _mapEvent)
    {
        mapEvent = _mapEvent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        mapEvent.PlayerEnterTrigger();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        mapEvent.PlayerExitTrigger();
    }
}
