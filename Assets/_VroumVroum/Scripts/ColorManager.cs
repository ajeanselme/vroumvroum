using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public static ColorManager instance { get; private set; }

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    
    [System.Serializable]
    public class  colorPimp
    {
        public string color;
        public bool isSet = false;
        [System.Serializable]
        public struct data
        {
            public Material[] materials;
            public Material keyMaterial;

        }
        public data[] Datas;
    }
    public List<colorPimp> colorChanger; 
    
}