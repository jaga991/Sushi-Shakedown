using System.Collections;
using UnityEngine;
using System.Runtime.CompilerServices;
public class WaveManager : MonoBehaviour
{

    public NPCSpawner npcSpawner;
    public float waveCountdownDuration = 2f;
    public CustomerData customerData;
    public int[] waveSizes = { 3, 5, 7 };
    private int tempWaveLimit = 1;
    public event System.Action OnWavesCompleted;
    public event System.Action<string> OnWaveStatusChanged;

    private LogSettings logSettings;
    private bool isDebugEnabled = false;

    private void OnEnable()
    {
        logSettings.OnSettingsChanged += UpdateLogStatus;
        UpdateLogStatus();
    }

    private void OnDisable()
    {
        logSettings.OnSettingsChanged -= UpdateLogStatus;
    }

    private void UpdateLogStatus()
    {
        isDebugEnabled = logSettings.WaveManagerLogs;
    }

    private void Awake()
    {
        if (logSettings)
        {
            logSettings = Resources.Load<LogSettings>("Guru/ScriptableObjects/LogSettings");
        }
        else
        {
            Debug.Log("LogSettings not found. Please assign it in the inspector.");
        }

    }



    private void Log(string message, [CallerMemberName] string caller = "")
    {
        if (isDebugEnabled)
            Debug.Log($"[WaveManager::{caller}] {message}");
    }

    public class WaveStats
    {
        public int waveNumber;
        public int customersServed;
        public int score;
        public int normalCustomersCount;
        public int angryCustomersCount;
    }

    private WaveStats GetWaveStats()
    {
        return new WaveStats
        {
            waveNumber = customerData.WaveCount,
            customersServed = customerData.customersServed,
            score = customerData.score,
            normalCustomersCount = customerData.normalCustomersCount,
            angryCustomersCount = customerData.angryCustomersCount
        };
    }

    /// <summary>
    /// Starts the waves.
    /// </summary>
    public void StartWaves()
    {
        StartCoroutine(RunWaves());
    }

    IEnumerator RunWaves()
    {
        // Loop over the configured wave sizes.
        for (int i = 0; i < tempWaveLimit; i++)
        {
            int waveNumber = i + 1;
            string msg = $"--- Wave {waveNumber}: Preparing to start ---";

            Log(msg);

            // Capture stats before the wave begins.
            WaveStats startStats = GetWaveStats();

            // Countdown.
            yield return StartCoroutine(WaveCountdown(waveCountdownDuration));

            // Update the global wave number.
            customerData.WaveCount = waveNumber;

            msg = $"Wave {waveNumber} started!";
            Log(msg);
            OnWaveStatusChanged?.Invoke(msg);

            // Spawn the wave.
            yield return StartCoroutine(SpawnWave(waveSizes[i], 1f, waveNumber));

            // Wait a moment between waves.
            yield return new WaitForSeconds(2f);

            // Print a summary using the difference between start and current stats.
            PrintWaveSummary(startStats, GetWaveStats());
        }

        OnWavesCompleted?.Invoke();
        string msg2 = "All waves for the day are complete!";

        Log(msg2);
        OnWaveStatusChanged?.Invoke("All waves over");
    }

    IEnumerator WaveCountdown(float seconds)
    {
        float count = seconds;
        while (count > 0)
        {
            yield return new WaitForSeconds(1f);
            count -= 1f;
        }
    }

    // Modified SpawnWave which tracks and displays the current count as "Wave X: A/B"
    public IEnumerator SpawnWave(int customerCount, float interval, int waveNumber)
    {
        int remaining = customerCount;
        int spawnedCount = 0; // number successfully spawned
        while (remaining > 0)
        {
            bool didSpawn = npcSpawner.SpawnCustomer();
            if (didSpawn)
            {
                spawnedCount++;
                remaining--;
                string spawnMsg = $"Wave {waveNumber}: {spawnedCount}/{customerCount}";
                OnWaveStatusChanged?.Invoke(spawnMsg);
                Log(spawnMsg);
                yield return new WaitForSeconds(interval);
            }
            else
            {
                Log("Spawn failed — will retry next tick.");
                yield return null;
            }
        }
        Log("Wave complete!");
    }

    void PrintWaveSummary(WaveStats startStats, WaveStats currentStats)
    {
        Log(
            $"--- Wave {currentStats.waveNumber} Summary ---\n" +
            $"Total Score: {currentStats.score - startStats.score}\n" +
            $"Customers Served: {currentStats.customersServed - startStats.customersServed}\n" +
            $"  • Happy/Normal: {currentStats.normalCustomersCount - startStats.normalCustomersCount}\n" +
            $"  • Angry/Failed: {currentStats.angryCustomersCount - startStats.angryCustomersCount}\n" +
            $"------------------------------"
        );
    }
}
