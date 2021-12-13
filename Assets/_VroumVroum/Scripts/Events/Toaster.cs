using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toaster : MonoBehaviour
{
    [Serializable]
    public class Slice
    {
        public GameObject gameObject;
        public Vector3 targetPosition;
        public float force = 1f;
    }

    public Slice[] slices;

    public void trigger()
    {
        for (int i = 0; i < slices.Length; i++)
        {
            slices[i].gameObject.GetComponent<Rigidbody>().AddForce(slices[i].targetPosition.normalized * slices[i].force, ForceMode.VelocityChange);
        }
    }
}
