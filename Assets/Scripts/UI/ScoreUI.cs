using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] TextMeshProUGUI scoreText;
    // Start is called before the first frame update
    void Start()
    {
        Hide();
        gameManager.OnScoreChanged += GameManager_OnScoreChanged;
        gameManager.OnStateChanged += GameManager_OnStateChanged;
    }

    private void GameManager_OnScoreChanged(object sender, EventArgs e)
    {
        scoreText.text = gameManager.GetScore().ToString();
    }

    private void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if(gameManager.IsGameOver() || gameManager.IsStartCountdown() || gameManager.IsWaitingToStart())
        {
            Hide();
        } else 
        {
            Show();
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
