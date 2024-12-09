// James Struble
// 12/8/2024
// Moves the lockpick around the lock

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockpickBehavior : MonoBehaviour
{
    [SerializeField] Transform rotationTarget; // Target in center of look which lockpick rotates around
    [SerializeField] float speed; // Speed lockpick moves

    private int lockpickDirection = 1; // Determines whether lockpick should move clockwise or counterclockwise
    private bool inLock; // Bool to track whether or not lockpick is on the pick point
    [SerializeField] GameManager gameManager; // Reference to GameManger script for event listening and state checking
    public event EventHandler OnSuccessfulLockPick; // Event for a successful pick
    public event EventHandler OnFailedLockPick; // Event for a failed pick

    void Start()
    {
        gameManager.OnStateChanged += GameManager_OnStateChanged; // Set up event listener for OnStateChanged, will call GameManager_OnStateChanged function when event fires
    }

    private void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        // Speeds up the lockpick if game level has increased
        // Stops lockpick if game has ended
        if (gameManager.IsLevel1())
        {
            speed = 100;
        }

        if (gameManager.IsLevel2())
        {
            speed += 30;
        }

        if (gameManager.IsLevel3())
        {
            speed += 30;
        }

        if (gameManager.IsGameOver() || gameManager.IsStartCountdown() || gameManager.IsWaitingToStart() || gameManager.IsGameWon())
        {
            speed = 0;
        }
    }

    void Update()
    {
        transform.RotateAround(rotationTarget.transform.position, new Vector3(0,0,-1), speed * lockpickDirection * Time.deltaTime); // Moves lockpick in a circle around the center of the lock.
        
        if (Input.GetKeyDown(KeyCode.Space) && !(gameManager.IsGameOver() || gameManager.IsStartCountdown() || gameManager.IsWaitingToStart() || gameManager.IsGameWon())) // If player presses Space and game has started and hasn't ended
        {
            if (inLock) // If lockpick is overlapping a pick point
            {
                inLock = false; // lockpick is no longer in pick point
                OnSuccessfulLockPick?.Invoke(this, EventArgs.Empty); // Fire OnSuccessfulLockPick event
            }
            else
            {
                OnFailedLockPick?.Invoke(this, EventArgs.Empty); // Fire OnFailedLockPick event
            }

        }
    }

    public void reverseLockpickDirection()
    {
        lockpickDirection *= -1; // Reverse direction lockpick is travellign in
    }

    private void OnTriggerEnter2D()
    {
        inLock = true; // Lockpick is overlapping pick point
    }
    private void OnTriggerExit2D()
    {
        if(inLock) // If exited pick point and lock wasn't picked
        {
            OnFailedLockPick?.Invoke(this, EventArgs.Empty); // Fire OnFailedLockPick event
        }
    }
}
