using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class DayManager : MonoBehaviour
{

    public CustomerData customerDataSO;
    public NPCSpawner npcSpawner; // assign via the Inspector
    public WaveManager waveManager; // assign via the Inspector

    public int customerWarmupThreshold = 3;
    private bool wavesStarted = false;

    // For testing, we fake served customers by counting key-presses
    private int servedCount = 0;

    private bool isActive = false;
    public int requiredServedCount = 3; // initial phase threshol

    private LogSettings logSettings;
    private bool isDebugEnabled = false;


    private void Awake()
    {
        if (!logSettings)
        {
            logSettings = Resources.Load<LogSettings>("Guru/ScriptableObjects/LogSettings");

        }
        else
        {
            // Log("LogSettings not found. Please assign it in the inspector.");
            Debug.Log("LogSettings not found. Please assign it in the inspector.");
        }

    }

    private void OnEnable()
    {
        logSettings.OnSettingsChanged += UpdateLogStatus;
        waveManager.OnWavesCompleted += OnWavesCompleted;
        UpdateLogStatus();
    }

    private void OnDisable()
    {
        logSettings.OnSettingsChanged -= UpdateLogStatus;
        waveManager.OnWavesCompleted -= OnWavesCompleted;
    }

    private void UpdateLogStatus()
    {
        isDebugEnabled = logSettings.WaveManagerLogs;
    }



    void Start()
    {
        Log("Day started random spawns until " + requiredServedCount + " customers are served.");
        isActive = true;
    }

    void Update()
    {
        if (!isActive) return;
        if (wavesStarted) return;
        if (customerDataSO.gameMode == GameMode.Waves)
        {
            wavesStarted = true;
            Log("Waves mode detected. Starting waves immediately.");
            waveManager.StartWaves();
        }

    }

    private void OnWavesCompleted()
    {
        Log("Waves completed. Summarizing the day...");
        isActive = false;
        wavesStarted = false;
        PrintDaySummary();
    }

    private void CheckCustomerCount(int servedCount)
    {
        if (wavesStarted) return;
        if (servedCount >= requiredServedCount && customerDataSO.gameMode == GameMode.Waves)
        {
            Log("Initial phase complete. Starting waves for the day!");
            wavesStarted = true;
            // Optionally, disable NPCSpawner’s auto-spawn mechanism.
            // npcSpawner.enableAutoSpawn = false;
            // Then start your waves:b
            waveManager.StartWaves();
        }
    }

    //helper functions 

    private void Log(string message, [CallerMemberName] string caller = "")
    {
        if (isDebugEnabled)
            Debug.Log($"[WaveManager::{caller}] {message}");
    }

    void PrintDaySummary()
    {
        string summary = $"Day Summary - Day: {customerDataSO.Day}, Score: {customerDataSO.score}, Total Served: {customerDataSO.customersServed}, Happy: {customerDataSO.normalCustomersCount}, Angry: {customerDataSO.angryCustomersCount}";
        Log(summary);
    }

}
