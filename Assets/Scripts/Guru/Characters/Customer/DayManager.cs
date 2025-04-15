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

    private bool freePlayActive = false; // For FreePlay mode, we need to track if we are in free play or not

    // For testing, we fake served customers by counting key-presses


    private bool isActive = false;
    public int requiredServedCount = 3; // initial phase threshol

    private LogSettings logSettings;
    private bool isDebugEnabled = false;


    public void Awake()
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

    public void OnEnable()
    {
        logSettings.OnSettingsChanged += UpdateLogStatus;
        waveManager.OnWavesCompleted += OnWavesCompleted;
        UpdateLogStatus();
    }

    public void OnDisable()
    {
        logSettings.OnSettingsChanged -= UpdateLogStatus;
        waveManager.OnWavesCompleted -= OnWavesCompleted;
    }

    private void UpdateLogStatus()
    {
        isDebugEnabled = logSettings.WaveManagerLogs;
    }



    public void Start()
    {
        Log("Day started random spawns until " + requiredServedCount + " customers are served.");
        isActive = true;
    }

    public void Update()
    {
        if (!isActive) return;
        if (wavesStarted) return;
        if (customerDataSO.gameMode == GameMode.Waves)
        {
            wavesStarted = true;
            Log("Waves mode detected. Starting waves immediately.");
            waveManager.StartWaves();
        }

        // If user picks FreePlay
        if (customerDataSO.gameMode == GameMode.FreePlay)
        {
            if (!freePlayActive)
            {
                freePlayActive = true;
                Log("FreePlay mode detected. Starting endless customer spawning.");
                waveManager.StartEndlessCustomers();
            }
            // Do nothing special here, 
            // because NPCSpawner is already spawning randomly via its Update().
            // If you want a condition to “end” free play, 
            // you could check for a key press or some milestone, and then do a summary.

            // Example: if you want to let the player press ESC to exit free play
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
