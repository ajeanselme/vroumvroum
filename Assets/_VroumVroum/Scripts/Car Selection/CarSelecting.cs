using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Unity.Mathematics;
using UnityEngine.UI;

public class CarSelecting : MonoBehaviour
{
    public Rewired.Player rewiredPlayer;

    private MeshRenderer mrCar, mrKey;

    private Coroutine moveCoroutine;

    private float timerLRJoystick = 0f;
    
    private bool isJoined = false;
    public bool isColorChose = false;
    public bool isLocked = false;

    public int colorIndex = 0;
    public int motifIndex = 0;

    [SerializeField] private TextMesh statusText;
    [SerializeField] private float moveForceEffect;
    [SerializeField] private float timePlayerEffect;

    [Space]
    [SerializeField] private GameObject lArrow;
    [SerializeField] private GameObject rArrow;
    [Space]
    [SerializeField] private GameObject carSilouhette;
    [SerializeField] private float yCarStart;

    private void Start()
    {
        mrCar = gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>();
        mrKey = gameObject.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>();
        
        // Default materials
        mrCar.materials = ColorManager.instance.defaultColor.materials;
        mrKey.material = ColorManager.instance.defaultColor.keyMaterial;
        
        lArrow.SetActive(false);
        rArrow.SetActive(false);
        carSilouhette.SetActive(true);
        
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }
        
        statusText.text = "Start\nTo Join";
    }
    
    private void Update()
    {
        if (rewiredPlayer == null || MenuManager.instance.isGameLaunched) return;

        if (!isJoined) return;

        if (rewiredPlayer.GetButtonDown("Lock"))
        {
            if (!isColorChose)
            {
                isColorChose = true;
                statusText.text = "Choose\nDesign";
                return;
            }
            
            isLocked = !isLocked;
            MenuManager.instance.SetLockState(this, isLocked);
            statusText.text = isLocked ? "Ready" : "Choose\nDesign";
        }
        
        if (isLocked) return;

        if (timerLRJoystick > 0f)
        {
            timerLRJoystick -= Time.deltaTime;
        }

        float lrValue = rewiredPlayer.GetAxis("LRMenu");
        
        if (lrValue != 0f && timerLRJoystick <= 0f)
        {
            if (lrValue < 0f) ChangePrevious();
            else ChangeNext();

            timerLRJoystick = 0.5f;
            return;
        }
        if (lrValue == 0f && timerLRJoystick > 0f)
        {
            timerLRJoystick = 0f;
        }
        
        /*if (rewiredPlayer.GetButtonDown("Left")) ChangePrevious();
        if (rewiredPlayer.GetButtonDown("Right")) ChangeNext();*/
    }

    public void PlayerJoin(Rewired.Player _player)
    {
        if (isJoined || _player == null) return;
        
        lArrow.SetActive(true);
        rArrow.SetActive(true);
        carSilouhette.SetActive(false);

        StartCoroutine(JoinShowCar());

        isJoined = true;
        statusText.text = "Choose\nColor";
        rewiredPlayer = _player;
        ChangeNext();
        MenuManager.instance.SetLockState(this, isLocked);
    }

    public void PlayerLeave(Rewired.Player _player)
    {
        lArrow.SetActive(false);
        rArrow.SetActive(false);
        carSilouhette.SetActive(true);
        
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }

        rewiredPlayer = null;
        statusText.text = "To Join";
        isJoined = false;
        isLocked = false;
        
        // Default materials
        mrCar.materials = ColorManager.instance.defaultColor.materials;
        mrKey.material = ColorManager.instance.defaultColor.keyMaterial;
    }

    private IEnumerator JoinShowCar()
    {
        float t = 0f;
        Vector3 carPos;
        carPos.x = gameObject.transform.position.x;
        carPos.y = yCarStart;
        carPos.z = gameObject.transform.position.z;

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(true);
        }

        while (t < 1f)
        {
            t += Time.deltaTime;

            carPos.y = Mathf.Lerp(yCarStart, 0f, t);
            gameObject.transform.position = carPos;
            
            yield return null;
        }
    }
    
    public void ChangeNext()
    {
        CarMoveEffect(-1f);
        
        if (!isColorChose)
        {
            if (colorIndex >= 0)
                ColorManager.instance.colorChanger[colorIndex].isSet = false;
            
            colorIndex++;
            
            if (colorIndex >= ColorManager.instance.colorChanger.Count)
                colorIndex = 0;

            colorIndex = GetNextAvailableColorIndex();

            ColorManager.instance.colorChanger[colorIndex].isSet = true;

            mrCar.materials = ColorManager.instance.colorChanger[colorIndex].Datas[0].materials;
        }
        else
        {
            motifIndex++;

            if (motifIndex == ColorManager.instance.colorChanger[colorIndex].Datas.Length)
                motifIndex = 0;

            mrCar.materials = ColorManager.instance.colorChanger[colorIndex].Datas[motifIndex].materials;
            mrKey.material = ColorManager.instance.colorChanger[colorIndex].Datas[motifIndex].keyMaterial;
        }
    }

    private int GetNextAvailableColorIndex()
    {
        int index = colorIndex;

        while (ColorManager.instance.colorChanger[index].isSet)
        {
            index++;

            if (index >= ColorManager.instance.colorChanger.Count)
                index = 0;
        }

        return index;
    }

    public void ChangePrevious()
    {
        CarMoveEffect(1f);
        
        if (!isColorChose)
        {
            if (colorIndex >= 0)
                ColorManager.instance.colorChanger[colorIndex].isSet = false;
            
            colorIndex--;
            
            if (colorIndex < 0)
                colorIndex = ColorManager.instance.colorChanger.Count - 1;

            colorIndex = GetPreviousAvailableColorIndex();

            ColorManager.instance.colorChanger[colorIndex].isSet = true;

            mrCar.materials = ColorManager.instance.colorChanger[colorIndex].Datas[0].materials;
        }
        else
        {
            motifIndex--;

            if (motifIndex < 0)
                motifIndex = ColorManager.instance.colorChanger[colorIndex].Datas.Length - 1;

            mrCar.materials = ColorManager.instance.colorChanger[colorIndex].Datas[motifIndex].materials;
            mrKey.material = ColorManager.instance.colorChanger[colorIndex].Datas[motifIndex].keyMaterial;
        }
    }
    
    private int GetPreviousAvailableColorIndex()
    {
        int index = colorIndex;

        while (ColorManager.instance.colorChanger[index].isSet)
        {
            index--;

            if (index < 0)
                index =  ColorManager.instance.colorChanger.Count - 1;
        }

        return index;
    }

    private void CarMoveEffect(float _dir)
    {
        
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        
        moveCoroutine = StartCoroutine(MoveEffect(_dir));
    }

    private IEnumerator MoveEffect(float _dir)
    {
        float t = 0f;

        float startZRot = transform.localRotation.eulerAngles.z;

        if (startZRot > 180f)
            startZRot = startZRot - 360f;

        while (t < 1f)
        {
            t += Time.deltaTime / timePlayerEffect;

            float zRot = Mathf.Lerp(startZRot, _dir * moveForceEffect, t);
            
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, zRot);

            yield return null;
        }

        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / timePlayerEffect;

            float zRot = Mathf.Lerp(_dir * moveForceEffect, 0f, t);
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, zRot);

            yield return null;
        }
    }
}
