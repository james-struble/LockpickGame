// James Struble
// 12/8/2024
// Controls the Countdown UI at the start of the game

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartCountdownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText; 
    [SerializeField] private GameManager gameManager; // Reference to GameManger script for event listening, state checking, and function calling
    //private int previousCountdownNumber; // Int for saving the last value of the countdown text

    private void Start()
    {
        gameManager.OnStateChanged += GameManager_OnStateChanged; // Set up event listener for OnStateChanged, will call GameManager_OnStateChanged function when event fires

        Hide(); // Hide Countdown UI elements on game start
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if(gameManager.IsStartCountdown()) // If game state has changed and state is now StartCountdown
        {
            Show(); // Show Countdown UI Elements
        }
        else
        {
            Hide(); // Hide Countdown UI Elements
        }
    }

    private void Update()
    {
        int countdownNumber = Mathf.CeilToInt(gameManager.GetStartCountdownTimer()); // Countdown number = GetCountdownTimer() value rounded up
        countdownText.text = countdownNumber.ToString(); // Change text to current value of countdownNumber
    }

    private void Show() // Show Countdown UI elements
    {
        gameObject.SetActive(true);
    }

    private void Hide() // Hide Countdown UI elements
    {
        gameObject.SetActive(false);
    }
}