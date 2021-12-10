using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{

    public enum CarColors {Blue,Cyan,Green,Orange,Pink,Purple,Red,White,Yellow}


    public int materialX = 0;
    public bool isgoldKey = false;
    public MeshRenderer key;
    public CarColors currentCarColor;
    MeshRenderer rend;
    private List<ColorManager.colorPimp> colorCombo;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<MeshRenderer>();
        colorCombo = ColorManager.instance.colorChanger;
        UpdateMaterials();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeNext()
    {
        if (materialX + 1 < rend.materials.Length)
        {
            UpdateMaterials();
            materialX++;
        }
    }

    private void UpdateMaterials()
    {
        Material[] temps = rend.materials;
        for (int i = 0; i < temps.Length; i++)
        {
            temps[i] = colorCombo[(int)currentCarColor].Datas[materialX].materials[i];
        }
        Debug.Log(materialX);
        rend.materials = temps;
        key.material = colorCombo[(int)currentCarColor].Datas[materialX].keyMaterial;
    }

    public void ChangePrevious()
    {
        if (materialX > 0)
        {
            UpdateMaterials();
            materialX--;
        }
    }
}
