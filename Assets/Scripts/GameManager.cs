// James Struble
// 12/8/2024
// Keeps track of the game's state and points

// Sound Credits:
//      Pick Sound: https://freesound.org/people/el_boss/sounds/665181/
//      Game Over: https://freesound.org/people/deleted_user_877451/sounds/76376/
//      Music: https://freesound.org/people/joshuaempyre/sounds/251461/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject prefabPickPoint; // Prefab of pickpoint to spawn as game progresses
    [SerializeField] AudioSource successAudio; // Audio for when pick point is successfully picked
    [SerializeField] AudioSource failureAudio; // Audio for when pick point is unsuccessfully picked or missed
    public event EventHandler OnScoreChanged; // Event for score being changed
    public event EventHandler OnStateChanged; // Event for game state being changed
    const String HI_SCORE = "HighScore"; // Constant string for saving highscore
    enum State // States the game can be in
    {
        WaitingToStart,
        StartCountdown,
        Level1,
        Level2,
        Level3,
        GameOver
    }

    private State state;
    private GameObject pickPoint; // Uninstantiated pick point for spawning new pick points
    private float startCountdownTimer = 3f; //  Max time for countdown timer at game start
    private int score = 0; // Score variable

    [SerializeField] LockpickBehavior lockpickBehavior; // Reference to LockpickBehavior script for event listening and public function calling
    [SerializeField] Transform lockpickPosition; // Saves position of lockpick in the scene
    [SerializeField] int noSpawnZoneRadius; // Radius of a sphere around lockpick where pickpoints can't spawn

    private void Awake()
    {
        state = State.WaitingToStart; // Set state to WaitingToStart, before doing ANYTHING else
    }
    void Start()
    {
        spawnPickPoint(); // Spawn pick point at start of game
        lockpickBehavior.OnSuccessfulLockPick += LockpickBehavior_OnSuccessfulLockPick; // Set up event listener OnSuccessfulLockPick, will call LockpickBehavior_OnSuccessfulLockPick function when event fires
        lockpickBehavior.OnFailedLockPick += LockpickBehavior_OnFailedLockPick; // Set up event listener OnFailedLockPick, will call LockpickBehavior_OnFailedLockPick function when event fires
    }

    private void LockpickBehavior_OnSuccessfulLockPick(object sender, EventArgs e)
    {
        score++; // Increase score
        OnScoreChanged?.Invoke(this, EventArgs.Empty); // Fire OnScoreChanged event
        Destroy(pickPoint); // Destroy the current pick point
        spawnPickPoint(); // Spawn a new pick point
        lockpickBehavior.reverseLockpickDirection(); // Reverse direction of lockpick through public function in lockpickBehavior
        successAudio.Play(); // Play successful pick sound
    }

    private void LockpickBehavior_OnFailedLockPick(object sender, EventArgs e)
    {
        state = State.GameOver; // Set state to GameOver
        OnStateChanged?.Invoke(this, EventArgs.Empty); // Fire OnStateChanged event
        failureAudio.Play(); // Play failed pick sound
    }

    void Update()
    {
        switch (state) // State machine
        {
            case State.WaitingToStart: // If game hasn't started
                state = State.StartCountdown; // Advance state to StartCountdown
                OnStateChanged?.Invoke(this, EventArgs.Empty); // Fire OnStateChanged event
                break;
            case State.StartCountdown:
                startCountdownTimer -= Time.deltaTime; // Countdown timer
                if (startCountdownTimer < 0f) // When timer ends
                {
                    state = State.Level1; // Advance state to Level1 (start the game)
                    OnStateChanged?.Invoke(this, EventArgs.Empty); // Fire OnStateChanged event
                }
                break;
            case State.Level1:
                if (score  >= 10)
                {
                    state = State.Level2; // Advance state to Level2 (speed up lock)
                    OnStateChanged?.Invoke(this, EventArgs.Empty); // Fire OnStateChanged event
                }
                break;
            case State.Level2:
                if (score >= 20)
                {
                    state = State.Level3; // Advance state to Level3 (speed up lock)
                    OnStateChanged?.Invoke(this, EventArgs.Empty); // Fire OnStateChanged event
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
        float distanceFromPlayer;

        do
        {
            randomPointOnCircle = UnityEngine.Random.insideUnitCircle.normalized * 4.2f; // Choose a random spawn location within the path the lockpick travels
            distanceFromPlayer = (new Vector3(randomPointOnCircle.x, randomPointOnCircle.y, 0f) - lockpickPosition.position).sqrMagnitude; // Get distance between proposed pick point spawn position and the lockpick
        } while (distanceFromPlayer < noSpawnZoneRadius * noSpawnZoneRadius); // If proposed pick point spawn position is within a circle of radius noSpawnRadius around the lockpick, choose a different random spawn location

        // Set pickPoint equal to the newly instantiated pick point
        pickPoint = 
        Instantiate(
            prefabPickPoint, 
            new Vector3(randomPointOnCircle.x, 
            randomPointOnCircle.y, 0), Quaternion.identity
        );
    }

    public int GetScore() // Public funciton that returns current score as int
    {
        return score;
    }
    public float GetStartCountdownTimer() // Public funciton that returns current timer value as float
    {
        return startCountdownTimer;
    }
    public void CheckHighScore()
    {
        if (score > PlayerPrefs.GetInt(HI_SCORE, 0)) // If current score beats saved HI_SCORE in PlayerPrefs
        {
            PlayerPrefs.SetInt(HI_SCORE, score); // Set HI_SCORE to score
        }
    }

    public int GetHighScore() // Checks if previous HI_SCORE in PlayerPrefs has been beaten, then returns HI_SCORE as an int
    {
        CheckHighScore();
        return PlayerPrefs.GetInt(HI_SCORE, 0);
    }

    public bool IsGameOver() // Public function that returns the truth value of whether or not game state is GameOver as a bool
    {
        return state == State.GameOver;
    }
    public bool IsWaitingToStart() // Public function that returns the truth value of whether or not game state is WaitingToStart as a bool
    {
        return state == State.WaitingToStart;
    }
    public bool IsStartCountdown() // Public function that returns the truth value of whether or not game state is StartCountdown as a bool
    {
        return state == State.StartCountdown;
    }
    public bool IsLevel1() // Public function that returns the truth value of whether or not game state is Level1 as a bool
    {
        return state == State.Level1;
    }
    public bool IsLevel2() // Public function that returns the truth value of whether or not game state is Level2 as a bool
    {
        return state == State.Level2;
    }
    public bool IsLevel3() // Public function that returns the truth value of whether or not game state is Level3 as a bool
    {
        return state == State.Level3;
    }
}
