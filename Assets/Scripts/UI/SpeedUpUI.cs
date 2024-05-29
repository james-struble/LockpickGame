using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeedUpUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameManager gameManager;
    private float displayTimer;
    private bool showing = false;
    // Start is called before the first frame update
    void Start()
    {
        Hide();
        gameManager.OnStateChanged += GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if(gameManager.IsLevel2() || gameManager.IsLevel3())
        {
            Show();
            displayTimer = 3f;
            showing = true;
            while (showing)
            {
                displayTimer -= Time.deltaTime;
                if (displayTimer == 0)
                {
                    Hide();
                    showing = false;
                }
            }
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
