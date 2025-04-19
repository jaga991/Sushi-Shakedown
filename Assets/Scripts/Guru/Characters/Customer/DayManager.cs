using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class DayManager : DebuggableMonoBehaviour
{

    public CustomerData customerDataSO;
    public NPCSpawner npcSpawner; // assign via the Inspector
    public WaveManager waveManager; // assign via the Inspector

    private bool isActive = false;
    public int requiredServedCount = 3; // initial phase thresho

    public void OnEnable()
    {
        base.OnEnable(); // Call the base class 
        waveManager.OnWavesCompleted += OnWavesCompleted;
        customerDataSO.OnGameModeChanged += OnModeChanged;
        UpdateLogStatus();

    }

    public void OnDisable()
    {
        base.OnDisable(); // Call the base class method to clean up logging
        waveManager.OnWavesCompleted -= OnWavesCompleted;
        customerDataSO.OnGameModeChanged -= OnModeChanged;
    }


    private void OnModeChanged(GameMode mode)
    {
        if (mode == GameMode.Waves)
        {
            waveManager.StopEndlessCustomers();
            waveManager.StartWaves();
        }
        else // FreePlay
        {
            waveManager.StopWaves();
            waveManager.StartEndlessCustomers();
            OnWavesCompleted();
        }
    }

    private void UpdateLogStatus()
    {
        isDebugEnabled = logSettings.DayManagerLogs;
    }

    public void Start()
    {
        Log("Day started random spawns until " + requiredServedCount + " customers are served.");
        isActive = true;
        OnModeChanged(customerDataSO.gameMode);
    }
    private void OnWavesCompleted()
    {
        Log("Waves completed. Summarizing the day...");
        isActive = false;

        PrintDaySummary();
    }

    void PrintDaySummary()
    {
        string summary = $"Day Summary - Day: {customerDataSO.Day}, Score: {customerDataSO.score}, Total Served: {customerDataSO.customersServed}, Happy: {customerDataSO.normalCustomersCount}, Angry: {customerDataSO.angryCustomersCount}";
        Log(summary);
    }

}
