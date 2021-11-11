using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarSelection2 : MonoBehaviour
{
    

    [SerializeField] private Button previousButton2;
    [SerializeField] private Button nextButton2;

    private int currentCar2;
    private void Awake()
    {
        SelectCar(0);
    }
    private void SelectCar(int _index)
    {
       
        previousButton2.interactable = (_index != 0);
        nextButton2.interactable = (_index != transform.childCount - 1);

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == _index);
        }
    }
    public void ChangeCar(int _change)
    {
        currentCar2 += _change;
        SelectCar(currentCar2);
    }
}
