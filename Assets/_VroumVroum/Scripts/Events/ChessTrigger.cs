using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChessTrigger : MonoBehaviour
{
    [Range(0,1)]
    public float triggerChance = .5f;
    public bool setValue;
    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Equals("TheRB"))
        {
            float random = Random.Range(0f, 1f);
            if (random <= triggerChance)
            {
                EventsManager.instance.isOn = setValue;
            }
        }
    }
}
