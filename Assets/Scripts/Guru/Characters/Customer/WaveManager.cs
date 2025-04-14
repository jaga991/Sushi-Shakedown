using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public NPCSpawner npcSpawner;            // Reference to NPCSpawner.
    public float waveCountdownDuration = 2f;   // Countdown before each wave.
    public CustomerData customerData;          // Reference to CustomerData.
    public int[] waveSizes = { 3, 5, 7 };        // Configurable list of wave sizes.
    private int tempWaveLimit = 1;             // This appears to control the number of waves; adjust as needed.

    public event System.Action OnWavesCompleted;
    public event System.Action<string> OnWaveStatusChanged;

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
            Debug.Log(msg);
            OnWaveStatusChanged?.Invoke(msg);

            // Capture stats before the wave begins.
            WaveStats startStats = GetWaveStats();

            // Countdown.
            yield return StartCoroutine(WaveCountdown(waveCountdownDuration));

            // Update the global wave number.
            customerData.WaveCount = waveNumber;

            msg = $"Wave {waveNumber} started!";
            Debug.Log(msg);
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
        Debug.Log(msg2);
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
                Debug.Log(spawnMsg);
                yield return new WaitForSeconds(interval);
            }
            else
            {
                Debug.Log("Spawn failed — will retry next tick.");
                yield return null;
            }
        }
        OnWaveStatusChanged?.Invoke($"Wave {waveNumber} complete!");
        Debug.Log("Wave complete!");
    }

    void PrintWaveSummary(WaveStats startStats, WaveStats currentStats)
    {
        Debug.Log(
            $"--- Wave {currentStats.waveNumber} Summary ---\n" +
            $"Total Score: {currentStats.score - startStats.score}\n" +
            $"Customers Served: {currentStats.customersServed - startStats.customersServed}\n" +
            $"  • Happy/Normal: {currentStats.normalCustomersCount - startStats.normalCustomersCount}\n" +
            $"  • Angry/Failed: {currentStats.angryCustomersCount - startStats.angryCustomersCount}\n" +
            $"------------------------------"
        );
    }
}
