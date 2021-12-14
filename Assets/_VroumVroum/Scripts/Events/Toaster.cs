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

    private void Start()
    {
        StartCoroutine(co());
    }

    private IEnumerator co()
    {
        yield return new WaitForSeconds(3f);

        trigger();
        
        yield return null;
    }
    
    public void trigger()
    {
        Debug.Log("size " + slices.Length);
        for (int i = 0; i < slices.Length; i++)
        {
            Debug.Log(slices[i].gameObject);
            Debug.Log(slices[i].targetPosition);
            Debug.Log(slices[i].force);
            Vector3 direction = slices[i].targetPosition - slices[i].gameObject.transform.position;
            direction = direction.normalized;
            Debug.Log(direction);
            slices[i].gameObject.GetComponent<Rigidbody>().AddForce(direction * slices[i].force, ForceMode.VelocityChange);
        }
    }
}
