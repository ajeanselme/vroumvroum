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

    private bool isJoined = false;
    public bool isColorChose = false;
    public bool isLocked = false;

    public int colorIndex = 0;
    public int motifIndex = 0;

    [SerializeField] private Text statusText;
    [SerializeField] private float moveForceEffect;
    [SerializeField] private float timePlayerEffect;

    private void Start()
    {
        mrCar = gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>();
        mrKey = gameObject.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>();
        
        // Default materials
        mrCar.materials = ColorManager.instance.defaultColor.materials;
        mrKey.material = ColorManager.instance.defaultColor.keyMaterial;
        
        statusText.text = "To Join";
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
                statusText.text = "Chose motif";
                return;
            }
            
            isLocked = !isLocked;
            statusText.text = isLocked ? "Ready" : "";
        }
        
        if (isLocked) return;

        if (rewiredPlayer.GetButtonDown("Left")) ChangePrevious();
        if (rewiredPlayer.GetButtonDown("Right")) ChangeNext();
        
    }

    public void PlayerJoin(Rewired.Player _player)
    {
        if (isJoined || _player == null) return;

        isJoined = true;
        statusText.text = "";
        rewiredPlayer = _player;
        ChangeNext();
    }

    public void PlayerLeave(Rewired.Player _player)
    {
        rewiredPlayer = null;
        statusText.text = "To Join";
        isJoined = false;
        isLocked = false;
        
        // Default materials
        mrCar.materials = ColorManager.instance.defaultColor.materials;
        mrKey.material = ColorManager.instance.defaultColor.keyMaterial;
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
        StopAllCoroutines();
        StartCoroutine(MoveEffect(_dir));
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
