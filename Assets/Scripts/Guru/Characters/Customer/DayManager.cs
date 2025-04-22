using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class DaySnapshot
{
    public int day;
    public int score;
    public int customersServed;
    public int happyCustomers;
    public int angryCustomers;
}


public class DayManager : DebuggableMonoBehaviour
{
    public CustomerData customerDataSO;
    public NPCSpawner npcSpawner; // assign via the Inspector
    public WaveManager waveManager; // assign via the Inspector
    public OverLayManager OM;

    public List<DaySnapshot> dayHistory = new List<DaySnapshot>();
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
        // if we've already completed the maximum days, go straight to final screen
        if (customerDataSO.Day > customerDataSO.maxDays)
        {
            OM.ShowFinalDayUI();
            return;
        }

        Debug.Log($"--- Starting Day {customerDataSO.Day} ---");
        // reset daily stats
        customerDataSO.WaveCount = 0;
        customerDataSO.customersServed = 0;
        customerDataSO.score = 0;
        customerDataSO.normalCustomersCount = 0;
        customerDataSO.angryCustomersCount = 0;

        if (customerDataSO.gameMode == GameMode.Waves)
            OM.ShowPreDayUI(customerDataSO.Day);
        else
            OnModeChanged(customerDataSO.gameMode);
    }

    public void HandleStartWaves()
    {
        if (customerDataSO.Day > customerDataSO.maxDays)
        {
            Debug.Log("Max days reached in HandleStartWaves — showing final UI.");
            OM.ShowFinalDayUI();
            return;
        }

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

        dayHistory.Add(new DaySnapshot
        {
            day = customerDataSO.Day,
            score = customerDataSO.score,
            customersServed = customerDataSO.customersServed,
            happyCustomers = customerDataSO.HappyCustomerCount,
            angryCustomers = customerDataSO.angryCustomersCount
        });

        if (customerDataSO.Day < customerDataSO.maxDays)
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
        Debug.Log("DayManager: Start() called.");
        StartDay();
        // OnModeChanged(customerDataSO.gameMode);
    }
    private void OnWavesCompleted()
    {
        Log("Waves completed. Summarizing the day...");
        PrintDaySummary();
        customerDataSO.IncrementCustomerCoins(customerDataSO.score);
        int dailyRansom = customerDataSO.GetRansom(customerDataSO.Day);
        customerDataSO.DecrementCustomerCoins(dailyRansom);
        if (customerDataSO.Day == customerDataSO.maxDays - 1)
        {
            OM.ShowFinalDayUI();

            // OM.ShowInfoUI(customerDataSO.Day, customerDataSO.score, customerDataSO.customersServed, customerDataSO.HappyCustomerCount, customerDataSO.angryCustomersCount, customerDataSO.CustomerCoins);
        }
        else
        {

            OM.ShowInfoUI(customerDataSO.Day, customerDataSO.score, customerDataSO.customersServed, customerDataSO.HappyCustomerCount, customerDataSO.angryCustomersCount, customerDataSO.CustomerCoins);
        }
    }

    void PrintDaySummary()
    {
        string summary = $"Day Summary - Day: {customerDataSO.Day}, Score: {customerDataSO.score}, Total Served: {customerDataSO.customersServed}, Happy: {customerDataSO.HappyCustomerCount}, Angry: {customerDataSO.angryCustomersCount}";
        Log(summary);
    }

}
