using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI hiScoreText;
    [SerializeField] private Button retryButton;

    void Awake()
    {
        retryButton.onClick.AddListener(ResetGame);
    }

    void Start()
    {
        Hide();
        gameManager.OnStateChanged += GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (gameManager.IsGameOver())
        {
            Show();
            scoreText.text = gameManager.GetScore().ToString();
            hiScoreText.text = gameManager.GetHighScore().ToString();
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

    private void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
