using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public bool defaultButton;
    public GameObject selector;

    private void Start()
    {
        selector.SetActive(defaultButton);
    }

    public void select()
    {
        selector.SetActive(true);
    }

    public void unselect()
    {
        selector.SetActive(false);
    }

    
    public void OnSelect(BaseEventData eventData)
    {
        selector.SetActive(true);
    }
    
    public void OnDeselect(BaseEventData eventData)
    {
        selector.SetActive(false);
    }
}
