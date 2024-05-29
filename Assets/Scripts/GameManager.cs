using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject prefabPickPoint;
    public event EventHandler OnScoreChanged;
    public event EventHandler OnStateChanged;
    const String HI_SCORE = "HighScore";
    enum State
    {
        WaitingToStart,
        StartCountdown,
        Level1,
        Level2,
        Level3,
        GameOver
    }

    private State state;
    private GameObject pickPoint;
    private float startCountdownTimer = 3f;
    private int score = 0;

    [SerializeField] LockpickBehavior lockpickBehavior;
    // Start is called before the first frame update

    private void Awake()
    {
        state = State.WaitingToStart;
    }
    void Start()
    {
        spawnPickPoint();
        lockpickBehavior.OnSuccessfulLockPick += LockpickBehavior_OnSuccessfulLockPick;
        lockpickBehavior.OnFailedLockPick += LockpickBehavior_OnFailedLockPick;
    }

    private void LockpickBehavior_OnSuccessfulLockPick(object sender, EventArgs e)
    {
        score++;
        OnScoreChanged?.Invoke(this, EventArgs.Empty);
        Destroy(pickPoint);
        spawnPickPoint();
        lockpickBehavior.reverseLockpickDirection();
    }

    private void LockpickBehavior_OnFailedLockPick(object sender, EventArgs e)
    {
        state = State.GameOver;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.WaitingToStart:
                state = State.StartCountdown;
                OnStateChanged?.Invoke(this, EventArgs.Empty);
                break;
            case State.StartCountdown:
                startCountdownTimer -= Time.deltaTime;
                if (startCountdownTimer < 0f)
                {
                    state = State.Level1;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.Level1:
                if (score  >= 10)
                {
                    state = State.Level2;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.Level2:
                if (score >= 20)
                {
                    state = State.Level3;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.Level3:
                break;
            case State.GameOver:
                break;
        }
    }

    private void spawnPickPoint()
    {
        var randomPointOnCircle = UnityEngine.Random.insideUnitCircle.normalized * 4.2f;
        pickPoint = Instantiate(prefabPickPoint, new Vector3(randomPointOnCircle.x, randomPointOnCircle.y, 0), Quaternion.identity);
    }

    public int GetScore()
    {
        return score;
    }
    public  float GetStartCountdownTimer()
    {
        return startCountdownTimer;
    }
    public void CheckHighScore()
    {
        if (score > PlayerPrefs.GetInt(HI_SCORE, 0))
        {
            PlayerPrefs.SetInt(HI_SCORE, score);
        }
    }

    public int GetHighScore()
    {
        CheckHighScore();
        return PlayerPrefs.GetInt(HI_SCORE, 0);
    }

    public bool IsGameOver()
    {
        return state == State.GameOver;
    }
    public bool IsWaitingToStart()
    {
        return state == State.WaitingToStart;
    }
    public bool IsStartCountdown()
    {
        return state == State.StartCountdown;
    }
    public bool IsLevel1()
    {
        return state == State.Level1;
    }
    public bool IsLevel2()
    {
        return state == State.Level2;
    }
    public bool IsLevel3()
    {
        return state == State.Level3;
    }
}
