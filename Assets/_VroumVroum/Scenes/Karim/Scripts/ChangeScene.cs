using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void MoveToScene(int Test )
    {
        CarSelecting[] cars = GameObject.FindObjectsOfType<CarSelecting>();
        UInt16[] carsIndex = new ushort[cars.Length];

        for (int i = 0; i < carsIndex.Length; i++)
            carsIndex[i] = Convert.ToUInt16(cars[i].currentCarIndex);
            
        //LaunchGameScene(carsIndex);
    }
}
