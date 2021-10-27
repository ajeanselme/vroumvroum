using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minigame : MonoBehaviour
{
    public Slider slider;
    public GameObject barre;

    public Text compteur;
    public Text compteur2;

    public GameObject decompteaenlever;
    public GameObject decompteaenlever2;

    public GameObject m2Debut;
    public GameObject m2Fin;
    public float m2Charge;

    public GameObject minigame1;
    public GameObject minigame2;

    private int randomMinigame = 100;

    private bool up, left, right, down = false;
    private bool goRight = true;
    private bool m2actif = false;

    private float decompte;
    private float decompte2;

    void Start()
    {
        minigame1.SetActive(false);
        minigame2.SetActive(false);
        decompteaenlever.SetActive(false);
        decompteaenlever2.SetActive(false);
        slider.maxValue = 100;
        slider.value = 0;
        decompte = 5.0f;
        decompte2 = 5.0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            randomMinigame = Random.Range(0, 2);

            if (randomMinigame == 0)
            {
                minigame1.SetActive(true);
                minigame2.SetActive(false);
                decompteaenlever.SetActive(true);
                decompteaenlever2.SetActive(false);
                slider.value = 0;
                decompte = 5.0f;
            }
            if (randomMinigame == 1)
            {
                minigame1.SetActive(false);
                minigame2.SetActive(true);
                decompteaenlever.SetActive(false);
                decompteaenlever2.SetActive(true);
                decompte2 = 5.0f;
                m2actif = true;
            }
        }

        float horizontal = Input.GetAxis("Horizontal");


        if (randomMinigame == 1 && m2actif)
        {
            if (barre.transform.position.x <= m2Debut.transform.position.x || goRight)
            {
                goRight = true;
                barre.transform.position += new Vector3(horizontal + 5, 0, 0);
            }
            if (barre.transform.position.x >= m2Fin.transform.position.x || !goRight)
            {
                goRight = false;
                barre.transform.position += new Vector3(horizontal - 5, 0, 0);
            }
        }


        if (randomMinigame == 0)
        {
            decompte -= Time.deltaTime;
            Debug.Log(decompte);

            if (decompte > 0)
                compteur.text = decompte.ToString();
            else
                compteur.text = "0";

            if (decompte < -3)
            {
                minigame1.SetActive(false);
                minigame2.SetActive(false);
                decompteaenlever.SetActive(false);
                decompteaenlever2.SetActive(false);
                Debug.Log("BOOM");
                Debug.Log(slider.value);
                randomMinigame = 100;
            }
        }

        if (randomMinigame == 1)
        {

            decompte2 -= Time.deltaTime;

            if (decompte2 > 0 && m2actif)
                compteur2.text = decompte2.ToString();
            else
            if (decompte2 < 0 && m2actif)
            {
                m2Charge = 0;
                compteur2.text = "0";
                m2actif = false;
            }

            if (decompte2 < -3 && randomMinigame == 1)
            {
                minigame1.SetActive(false);
                minigame2.SetActive(false);
                decompteaenlever.SetActive(false);
                decompteaenlever2.SetActive(false);
                Debug.Log("BOOM2");
                Debug.Log(m2Charge);
                randomMinigame = 100;
            }
        }


        if (Input.GetKeyDown(KeyCode.UpArrow) && decompte > 0 && randomMinigame == 0)
        {
            if (!up && !down)
            {
                Up();
                SetCharge();
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && decompte > 0 && randomMinigame == 0)
        {
            if (!left && !right)
            {
                Left();
                SetCharge();
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && decompte > 0 && randomMinigame == 0)
        {
            if (!left && !right)
            {
                Right();
                SetCharge();
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && decompte > 0 && randomMinigame == 0)
        {
            if (!up && !down)
            {
                Down();
                SetCharge();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && randomMinigame == 1)
        {
            m2Charge = (((barre.transform.position.x - m2Debut.transform.position.x) * 100) / (m2Fin.transform.position.x - m2Debut.transform.position.x)) * 2;
            if (m2Charge > 100)
                m2Charge = 200 - m2Charge;
            
            m2actif = false;
            decompte2 = 0f;
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

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if (barre.gameObject.CompareTag ("bordDroit"))
        {
            goRight = false;
        }
        if (barre.gameObject.CompareTag("bordGauche"))
        {
            goRight = true;
        }
    }*/

}
