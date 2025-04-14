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

    void Start()
    {
        Debug.Log("Day started random spawns until " + requiredServedCount + " customers are served.");
        isActive = true;
    }

    void OnEnable()
    {
        // Subscribe to the event.
        // customerDataSO.OnCustomerServed += CheckCustomerCount;
        waveManager.OnWavesCompleted += OnWavesCompleted;
    }

    void OnDisable()
    {
        // Unsubscribe when this object is disabled/destroyed.

        // customerDataSO.OnCustomerServed -= CheckCustomerCount;
        waveManager.OnWavesCompleted -= OnWavesCompleted;
    }

    void Update()
    {
        if (!isActive) return;
        if (wavesStarted) return;
        if (customerDataSO.gameMode == GameMode.Waves)
        {
            wavesStarted = true;
            Debug.Log("Waves mode detected. Starting waves immediately.");
            waveManager.StartWaves();
        }

    }
    void PrintDaySummary()
    {
        Debug.Log("----- Day Summary -----");
        Debug.Log("Day: " + customerDataSO.Day);
        Debug.Log("Total Score: " + customerDataSO.score);
        Debug.Log("Total Customers Served: " + customerDataSO.customersServed);
        Debug.Log("Normal/Happy Customers: " + customerDataSO.normalCustomersCount);
        Debug.Log("Angry/Failed Customers: " + customerDataSO.angryCustomersCount);
        Debug.Log("-----------------------");
    }
    private void OnWavesCompleted()
    {
        Debug.Log("Waves completed. Summarizing the day...");
        isActive = false;
        wavesStarted = false;
        PrintDaySummary();
    }

    private void CheckCustomerCount(int servedCount)
    {
        if (wavesStarted) return;
        if (servedCount >= requiredServedCount && customerDataSO.gameMode == GameMode.Waves)
        {
            Debug.Log("Initial phase complete. Starting waves for the day!");
            wavesStarted = true;
            // Optionally, disable NPCSpawner’s auto-spawn mechanism.
            // npcSpawner.enableAutoSpawn = false;
            // Then start your waves:b
            waveManager.StartWaves();
        }
    }

}
