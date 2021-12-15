using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    public static PauseController instance;
    
    public RectTransform pauseTranform;
    private bool isPaused;

    private void Awake()
    {
        instance = this;
    }

    public void pauseGame()
    {
        Time.timeScale = 0;
        pauseTranform.position = new Vector2(Screen.width / 2f, Screen.height / 2f);
        pauseTranform.gameObject.SetActive(true);
    }
    
    public void resumeGame()
    {
        Time.timeScale = 1f;
        pauseTranform.gameObject.SetActive(false);
    }
}
