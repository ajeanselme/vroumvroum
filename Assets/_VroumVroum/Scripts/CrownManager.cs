using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using Unity.Mathematics;
using UnityEngine;

public class CrownManager : MonoBehaviour
{
    public static CrownManager instance;

    [SerializeField] private GameObject crownPrefab;

    private Rewired.Player player;
    private GameObject crown;

    [HideInInspector] public bool isSelectingLoaded = false;
    private bool isCrownDone = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // private void Start()
    // {
    //     if (!isSelectingLoaded || isCrownDone || player == null) return;
    //
    //     GameObject car = MenuManager.instance.IsPlayerSet(player);
    //     
    //     if (car != null)
    //     {
    //         SetCrown(car);
    //     }
    // }

    private void FixedUpdate()
    {
        if (!isSelectingLoaded || isCrownDone || player == null) return;
    
        GameObject car = MenuManager.instance.IsPlayerSet(player);
        
        if (car != null)
        {
            SetCrown(car);
        }
    }

    private void SetCrown(GameObject _car)
    {
        isCrownDone = true;

        GameObject nCrown = Instantiate(crownPrefab, _car.transform);
        nCrown.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        nCrown.transform.localPosition = new Vector3(-0.284f,1.073f,0.25f);
        nCrown.transform.localEulerAngles = new Vector3(6.35f,1.8f,15.8f);
        // nCrown.AddComponent<CrownLevitate>().Initialize(0.2f, 1f);
        
        crown = nCrown;
        
        Destroy(gameObject);
    }

    public void SetWinnerPlayer(Rewired.Player _player)
    {
        player = _player;
    }
}
