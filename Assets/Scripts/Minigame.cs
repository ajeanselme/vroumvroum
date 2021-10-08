using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minigame : MonoBehaviour
{
    public Slider slider;
    public Text compteur;

    private bool up, left, right, down = false;

    private float decompte;

    void Start()
    {
        slider.maxValue = 100;
        slider.value = 0;
        decompte = 5.0f;
    }

    void Update()
    {
        if (decompte > 0)
        {
            decompte -= Time.deltaTime;
            compteur.text = decompte.ToString();
        }
        else
            compteur.text = "0";

        if (Input.GetKeyDown(KeyCode.UpArrow) && decompte > 0)
        {
            if (!up && !down)
            {
                Up();
                SetCharge();
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && decompte > 0)
        {
            if (!left && !right)
            {
                Left();
                SetCharge();
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && decompte > 0)
        {
            if (!left && !right)
            {
                Right();
                SetCharge();
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && decompte > 0)
        {
            if (!up && !down)
            {
                Down();
                SetCharge();
            }
        }

        if (decompte == 0)
        {
            Debug.Log("BOOM");
            //jouer animation, départ ect
        }

    }

    public void SetCharge()
    {
        slider.value += 5;
    }

    public void Up()
    {
        up = true;
        left = false;
        right = false;
        down = false;
    }

    public void Left()
    {
        up = false;
        left = true;
        right = false;
        down = false;
    }

    public void Right()
    {
        up = false;
        left = false;
        right = true;
        down = false;
    }

    public void Down()
    {
        up = false;
        left = false;
        right = false;
        down = true;
    }

}
