using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class DayManager : DebuggableMonoBehaviour
{

    public CustomerData customerDataSO;
    public NPCSpawner npcSpawner; // assign via the Inspector
    public WaveManager waveManager; // assign via the Inspector
    public int requiredServedCount = 3; // initial phase thresho

    public int maxDays = 7;

    public OverLayManager OM;


    protected override void OnEnable()
    {
        base.OnEnable(); // Call the base class 
        waveManager.OnWavesCompleted += OnWavesCompleted;
        customerDataSO.OnGameModeChanged += OnModeChanged;
        OM.OnPreDayClosed += HandleStartWaves;
        OM.OnInfoClosed += OnInfoClosed;
        UpdateLogStatus();

    }

    protected override void OnDisable()
    {
        base.OnDisable(); // Call the base class method to clean up logging
        waveManager.OnWavesCompleted -= OnWavesCompleted;
        customerDataSO.OnGameModeChanged -= OnModeChanged;
        OM.OnPreDayClosed -= HandleStartWaves;
        OM.OnInfoClosed -= OnInfoClosed;
    }
    // encapsulate starting each day
    void StartDay()
    {
        Debug.Log($"--- Starting Day {customerDataSO.Day} ---");
        // reset daily stats if you want fresh per-day metrics:
        customerDataSO.WaveCount = 0;
        customerDataSO.customersServed = 0;
        customerDataSO.score = 0;
        customerDataSO.normalCustomersCount = 0;
        customerDataSO.angryCustomersCount = 0;

        // kick off waves (or freeplay) based on current mode
        // OnModeChanged(customerDataSO.gameMode);
        if (customerDataSO.gameMode == GameMode.Waves)
        {
            OM.ShowPreDayUI(customerDataSO.Day);
        }
        else // FreePlay
        {
            OnModeChanged(customerDataSO.gameMode);
        }
    }

    public void HandleStartWaves()
    {
        OnModeChanged(customerDataSO.gameMode);
    }


    private void OnModeChanged(GameMode mode)
    {
        if (mode == GameMode.Waves)
        {
            Debug.Log("Day is ." + customerDataSO.Day + " in Waves mode.");

            waveManager.StopEndlessCustomers();
            waveManager.StartWaves();
        }
        else // FreePlay
        {
            waveManager.StopWaves();
            waveManager.StartEndlessCustomers();
            // OnWavesCompleted();
        }
    }
    // fired by OverLayManager.CloseInfoUI()
    private void OnInfoClosed()
    {
        if (customerDataSO.Day < maxDays)
        {
            customerDataSO.Day++;
            StartDay();
        }
        else
        {
            Debug.Log("All days complete! Transition to endgame...");
            // TODO: show final results / return to menu / quit
        }
    }



    protected override void UpdateLogStatus()
    {
        isDebugEnabled = logSettings.DayManagerLogs;
    }

    public void Start()
    {
        StartDay();
        // OnModeChanged(customerDataSO.gameMode);
    }
    private void OnWavesCompleted()
    {
        Log("Waves completed. Summarizing the day...");
        PrintDaySummary();
        OM.ShowInfoUI(customerDataSO.Day, customerDataSO.score, customerDataSO.customersServed, customerDataSO.HappyCustomerCount, customerDataSO.angryCustomersCount);
    }

    void PrintDaySummary()
    {
        string summary = $"Day Summary - Day: {customerDataSO.Day}, Score: {customerDataSO.score}, Total Served: {customerDataSO.customersServed}, Happy: {customerDataSO.HappyCustomerCount}, Angry: {customerDataSO.angryCustomersCount}";
        Log(summary);
    }

}
