using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockpickBehavior : MonoBehaviour
{
    [SerializeField] Transform rotationTarget;
    [SerializeField] float speed;

    private int lockpickDirection = 1;
    private bool inLock;
    [SerializeField] GameManager gameManager;
    public event EventHandler OnSuccessfulLockPick;
    public event EventHandler OnFailedLockPick;

    // Start is called before the first frame update
    void Start()
    {
        gameManager.OnStateChanged += GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (gameManager.IsLevel1())
        {
            speed = 100;
        }

        if (gameManager.IsLevel2())
        {
            speed += 20;
        }

        if (gameManager.IsLevel3())
        {
            speed += 20;
        }

        if (gameManager.IsGameOver() || gameManager.IsStartCountdown() || gameManager.IsWaitingToStart())
        {
            speed = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(rotationTarget.transform.position, new Vector3(0,0,-1), speed * lockpickDirection * Time.deltaTime);

        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //    if (inLock)
        //     {
        //         inLock = false;
        //         Debug.Log("Success!");
        //     }
        //     else
        //     {
        //         Debug.Log("Fail!");
        //     }
        // }
        
        if (Input.GetKeyDown(KeyCode.Space) && !(gameManager.IsGameOver() || gameManager.IsStartCountdown() || gameManager.IsWaitingToStart()))
        {
            if (inLock)
            {
                inLock = false;
                OnSuccessfulLockPick?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                OnFailedLockPick?.Invoke(this, EventArgs.Empty);
            }

        }
    }

    public void reverseLockpickDirection()
    {
        lockpickDirection *= -1;
    }

    private void OnTriggerEnter2D()
    {
        inLock = true;
    }
    private void OnTriggerExit2D()
    {
        if(inLock)
        {
            OnFailedLockPick?.Invoke(this, EventArgs.Empty);
        }
    }
}
