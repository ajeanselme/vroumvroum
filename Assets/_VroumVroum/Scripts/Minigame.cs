using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Minigame : MonoBehaviour
{
    public Slider slider;
    public GameObject barre;
    [HideInInspector]
    public CarController car1;
    public bool begin = false;

    [Header("UI")]
    public Text compteur;
    public Text compteur2;

    public GameObject decompteaenlever;
    public GameObject decompteaenlever2;
    
    public GameObject m2Debut;
    public GameObject m2Fin;
    
    public GameObject minigame1;
    public GameObject minigame2;

    [Header("Values")]
    public float incrementMG1 = 2f;
    public float resistanceMG1 = .5f;
    public float incrementMG2 = 2f;
    
    private float vitesseBase;

    private float m2Charge;


    private int randomMinigame = -1;

    private bool goRight = true;
    private bool m2actif;

    private float decompteMG1 = 5f;
    private float decompteMG2 = 5f;

    private MGState _mgState = MGState.TOP;
    
    private enum MGState
    {
        TOP,
        RIGHT,
        BOTTOM,
        LEFT
    }

    void Start()
    {
        minigame1.SetActive(false);
        minigame2.SetActive(false);
        decompteaenlever.SetActive(false);
        decompteaenlever2.SetActive(false);
        slider.maxValue = 100;
        slider.value = 0;
    }

    void Update()
    {
        if (randomMinigame > -1)
        {
            if (Input.GetKeyDown(KeyCode.M) || begin)
            {
                if (randomMinigame == 0)
                {
                    minigame1.SetActive(true);
                    minigame2.SetActive(false);
                    decompteaenlever.SetActive(true);
                    decompteaenlever2.SetActive(false);
                    slider.value = 0;
                    decompteMG1 = 5.0f;
                }
                if (randomMinigame == 1)
                {
                    minigame1.SetActive(false);
                    minigame2.SetActive(true);
                    decompteaenlever.SetActive(false);
                    decompteaenlever2.SetActive(true);
                    decompteMG2 = 5.0f;
                    m2actif = true;
                }
                begin = false;
            }

            if (randomMinigame == 0)
            {
                decompteMG1 -= Time.deltaTime;

                if (decompteMG1 > 0)
                    compteur.text = decompteMG1.ToString();
                else
                    compteur.text = "0";

                if (decompteMG1 < -2.5f)
                {
                    resetMG(slider.value);
                }
                else
                if (decompteMG1 > 0)
                {
                    switch (_mgState)
                    {
                        case MGState.TOP:
                            if (TurnManager.instance.cars[TurnManager.instance.indexCarTurn].rewiredPlayer.GetAxis("Horizontal") > .9f)
                            {
                                AddCharge();
                                _mgState = MGState.RIGHT;
                            }
                            break;
                        case MGState.RIGHT:
                            if (TurnManager.instance.cars[TurnManager.instance.indexCarTurn].rewiredPlayer.GetAxis("Vertical") < -.9f)
                            {
                                AddCharge();
                                _mgState = MGState.BOTTOM;
                            }
                            break;
                        case MGState.BOTTOM:
                            if (TurnManager.instance.cars[TurnManager.instance.indexCarTurn].rewiredPlayer.GetAxis("Horizontal") < -.9f)
                            {
                                AddCharge();
                                _mgState = MGState.LEFT;
                            }
                            break;
                        case MGState.LEFT:
                            if (TurnManager.instance.cars[TurnManager.instance.indexCarTurn].rewiredPlayer.GetAxis("Vertical") > .9f)
                            {
                                AddCharge();
                                _mgState = MGState.TOP;
                            }
                            break;
                    }
                }
            }
            else if (randomMinigame == 1)
            {
                decompteMG2 -= Time.deltaTime;

                if (decompteMG2 > 0 && m2actif)
                    compteur2.text = decompteMG2.ToString();
                else
                if (decompteMG2 < 0 && m2actif)
                {
                    m2Charge = 0;
                    compteur2.text = "0";
                    m2actif = false;
                }

                if (decompteMG2 < -2.5f && randomMinigame == 1)
                {
                    resetMG(m2Charge);
                }
            }

            if (TurnManager.instance.cars[TurnManager.instance.indexCarTurn].rewiredPlayer.GetButtonDown("Cross") && randomMinigame == 1)
            {
                m2Charge = (((barre.transform.position.x - m2Debut.transform.position.x) * 100) / (m2Fin.transform.position.x - m2Debut.transform.position.x)) * 2;
                if (m2Charge > 100)
                    m2Charge = 200 - m2Charge;
                
                m2actif = false;
                decompteMG2 = 0f;
            }
        }
    }

    private void FixedUpdate()
    {
        if (randomMinigame == 0)
        {
            if (decompteMG1 > 0)
            {
                RemoveCharge();
            }
        }
        
        if (randomMinigame == 1 && m2actif)
        {
            if (barre.transform.position.x <= m2Debut.transform.position.x || goRight)
            {
                goRight = true;
                barre.transform.position += new Vector3((Screen.width / 100f + incrementMG2), 0, 0);
            }
            if (barre.transform.position.x >= m2Fin.transform.position.x || !goRight)
            {
                goRight = false;
                barre.transform.position -= new Vector3(Screen.width / 100f + incrementMG2, 0, 0);
            }
        }
    }

    public void beginMinigame(CarController player)
    {
        randomMinigame = Random.Range(0, 2);
        begin = true;
        car1 = player;
    }

    public void launchCar(float value)
    {
        TurnManager.instance.BoostCarEffects(1f);
        car1.launchCar(car1.totalTime * value / 100f);
    }

    public void AddCharge()
    {
        float inc = slider.value + incrementMG1;
        if (inc < slider.maxValue)
        {
            slider.value = inc;
        }
    }

    public void RemoveCharge()
    {
        float inc = slider.value - resistanceMG1;
        if (inc > 0)
        {
            slider.value = inc;
        }
    }

    private void resetMG(float value)
    {
        minigame1.SetActive(false);
        minigame2.SetActive(false);
        decompteaenlever.SetActive(false);
        decompteaenlever2.SetActive(false);
        launchCar(value);
        randomMinigame = -1;
    }
}
