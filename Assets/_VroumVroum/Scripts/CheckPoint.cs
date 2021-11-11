using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private int index = 0;

    private void Start()
    {
        /*
         * For unknown reasons the index get incremented on start.
         * Instead of losing my mind I chose to do that,
         * or maybe I already am.
         */
        index--;
    }

    public void setIndex(int i)
    {
        index = i;
    }
}
