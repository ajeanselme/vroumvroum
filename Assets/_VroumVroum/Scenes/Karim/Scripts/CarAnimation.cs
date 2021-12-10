using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAnimation : MonoBehaviour
{
    [SerializeField] private Vector3 finalPosition;
    
    private Vector3 initialposition;
    

    private void Awake()
    {
        initialposition = transform.position;
        

    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, finalPosition, 0.1f);
       

    }

    private void OnDisable()
    {
        transform.position = initialposition;
    }

    
   

}
