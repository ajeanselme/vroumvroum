using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;

    private Vector3 offset = new Vector3(0, 3.5f, 6f);

    private void Update()
    {
        transform.position = player.transform.position + offset;
    }
}
