using System;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "CustomerData", menuName = "ScriptableObjects/CustomerData")]
public class CustomerData : ScriptableObject
{
    public GameMode gameMode;

    // Existing
    public int customersServed;
    public event Action<int> OnCustomerServed;

    public int Day = 0;
    public int WaveCount = 0;

    // NEW: score tracking
    public int score = 0;
    public int normalCustomersCount = 0;
    public int HappyCustomerCount = 0;

    public int angryCustomersCount = 0;
    public event Action<int> OnScoreChanged;
    public event Action<GameMode> OnGameModeChanged;

    public void ResetCustomerCount() => customersServed = 0;
    public void ResetDay() => Day = 0;
    public void ResetWaveCount() => WaveCount = 0;

    private void OnEnable()
    {
        // Initialize the data when the scriptable object is enabled.
        OnStartup();
        // gameMode = GameMode.Waves; // Default mode
    }

    public void SetGameMode(GameMode mode)
    {
        gameMode = mode;
        Debug.Log($"Game mode set to: {gameMode}");
        OnGameModeChanged?.Invoke(gameMode);
    }

    public void OnStartup()
    {

        customersServed = 0;
        WaveCount = 0;
        score = 0;
        normalCustomersCount = 0;
        HappyCustomerCount = 0;
        angryCustomersCount = 0;
        Debug.Log("CustomerData initialized.");
    }

    public void Increment()
    {
        customersServed++;
        OnCustomerServed?.Invoke(customersServed);
    }


    // NEW METHODS:
    public void AddScore(int amount = 1)
    {

        score += amount;
        if (amount > 5)
            HappyCustomerCount++;
        else
            normalCustomersCount++;
        // Debug.Log($"Score +{amount}. Total: {score}. Normal served: {normalCustomersCount}");
        OnScoreChanged?.Invoke(score);
    }

    public void DeductScore(int amount)
    {
        score -= amount;
        angryCustomersCount++;
        // Debug.Log($"Score –{amount}. Total: {score}. Angry served: {angryCustomersCount}");
        OnScoreChanged?.Invoke(score);
    }

    public void ResetScore()
    {
        score = 0;
        normalCustomersCount = 0;
        angryCustomersCount = 0;
        Debug.Log("Score data reset.");
        OnScoreChanged?.Invoke(score);
    }

    public void ResetEverything()
    {
        ResetCustomerCount();
        ResetDay();
        ResetWaveCount();
        ResetScore();
        Debug.Log("All data reset.");
    }
}


public enum GameMode
{
    FreePlay,
    Waves
}