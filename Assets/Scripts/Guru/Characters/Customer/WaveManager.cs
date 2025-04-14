using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public NPCSpawner npcSpawner; // Reference to NPCSpawner.
    public float waveCountdownDuration = 2f; // Countdown before each wave.
    public CustomerData customerData; // Reference to CustomerData.

    public int[] waveSizes = { 3, 5, 7 };
    private int tempWaveLimit = 1;


    /// <summary>
    /// Starts the three waves for the day.
    /// </summary>
    public void StartWaves()
    {
        StartCoroutine(RunWaves());
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
        // This is a placeholder. Replace with actual logic to get wave data.
        return new WaveStats
        {
            waveNumber = customerData.WaveCount,
            customersServed = customerData.customersServed,
            score = customerData.score,
            normalCustomersCount = customerData.normalCustomersCount,
            angryCustomersCount = customerData.angryCustomersCount
        };
    }

    IEnumerator RunWaves()
    {
        // Loop over the configured wave sizes.
        for (int i = 0; i < tempWaveLimit; i++)
        {
            int waveNumber = i + 1;
            Debug.Log($"--- Wave {waveNumber}: Preparing to start ---");

            // Capture stats before the wave begins.
            WaveStats startStats = GetWaveStats();

            // Count down.
            yield return StartCoroutine(WaveCountdown(waveCountdownDuration));

            // Set the current wave number in global data if needed.
            customerData.WaveCount = waveNumber;

            // Spawn the wave.
            yield return StartCoroutine(SpawnWave(waveSizes[i], 1f));

            // Wait a moment between waves.
            yield return new WaitForSeconds(2f);

            // Print a summary using the difference between start and current stats.
            PrintWaveSummary(startStats, GetWaveStats());
        }

        Debug.Log("All waves for the day are complete!");
    }


    // A simple countdown routine that logs the time left.
    IEnumerator WaveCountdown(float seconds)
    {
        float count = seconds;
        while (count > 0)
        {
            // Debug.Log("Wave starting in: " + count.ToString("F0") + " seconds");
            yield return new WaitForSeconds(1f);
            count -= 1f;
        }
    }

    // Spawns a wave of customers using NPCSpawner.SpawnCustomer().
    public IEnumerator SpawnWave(int customerCount, float interval)
    {
        int remaining = customerCount;
        while (remaining > 0)
        {
            // try to spawn
            bool didSpawn = npcSpawner.SpawnCustomer();
            if (didSpawn)
            {
                remaining--;
                Debug.Log($"Spawned wave customer. {remaining} left in this wave.");
            }
            else
            {
                Debug.Log("Spawn failed — will retry next tick.");
                yield return null;
            }

            // wait before next attempt (even if spawn failed, we wait to avoid tight loops)
            yield return new WaitForSeconds(interval);
        }

        Debug.Log("Wave complete!");
    }

    void PrintWaveSummary(WaveStats StartwaveData, WaveStats EndWaveStats)
    {
        Debug.Log(
            $"--- Wave {EndWaveStats.waveNumber} Summary ---\n" +
            $"Total Score: {EndWaveStats.score - StartwaveData.score}\n" +
            $"Customers Served: {EndWaveStats.customersServed - StartwaveData.customersServed}\n" +
            $"  • Happy/Normal: {EndWaveStats.normalCustomersCount - StartwaveData.normalCustomersCount}\n" +
            $"  • Angry/Failed: {EndWaveStats.angryCustomersCount - StartwaveData.angryCustomersCount}\n" +
            $"------------------------------"
        );
    }

}
