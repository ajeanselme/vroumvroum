using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToasterTrigger : MonoBehaviour
{
    public Toaster toaster;
    [Range(0,1)]
    public float triggerChance = .5f;

    private bool triggered;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!triggered)
        {
            if (other.name.Equals("TheRB"))
            {
                float random = Random.Range(0f, 1f);
                if (random <= triggerChance)
                {
                    toaster.trigger();
                    triggered = true;
                }
            }
        }
    }
}
