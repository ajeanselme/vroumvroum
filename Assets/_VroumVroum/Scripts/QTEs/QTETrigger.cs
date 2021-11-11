using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTETrigger : MonoBehaviour
{
    public bool turnOffOnLeave = false;
    public QTEIcon parent;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.transform.root.tag);
        if (other.transform.root.CompareTag("Player"))
        {
            parent.Toggle();
            Debug.Log("call");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (turnOffOnLeave)
        {
            if (other.transform.root.CompareTag("Player"))
            {
                parent.Toggle();
            }
        }
    }
}
