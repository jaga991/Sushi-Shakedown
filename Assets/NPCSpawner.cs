using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCSpawner : MonoBehaviour
{
    public GameObject npcTemplate;
    public GameObject customerTemplate;
    // Use your scene object as a template. Make sure to disable it in the scene. 
    public float spawnInterval = 2.0f;

    public float spawnMargin = 1.0f;

    private float timer;

    public SpawnZoneData spawnZone;
    public bool spawnCustomerDebug = false;
    void Start()
    {
        // Initialize the timer so that a spawn happens after the first interval.
        timer = spawnInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnNPC();
            timer = spawnInterval;

        }
        if (spawnCustomerDebug)
        {
            SpawnCustomer();
            spawnCustomerDebug = false; // Reset the debug flag after spawning.
        }
    }
    void SpawnNPC()
    {
        bool spawnFromLeft = UnityEngine.Random.value > 0.5f;
        Camera cam = Camera.main;
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        float spawnX;
        float direction;
        if (spawnFromLeft)
        {
            spawnX = cam.transform.position.x - (camWidth / 2) - spawnMargin;
            direction = 1f;  // NPC moves right.
        }
        else
        {
            spawnX = cam.transform.position.x + (camWidth / 2) + spawnMargin;
            direction = -1f; // NPC moves left.
        }

        // Use the spawn zone's y limits instead of the entire camera height.
        float spawnY = Random.Range(spawnZone.MinY, spawnZone.MaxY);

        Vector3 spawnPosition = new(spawnX, spawnY, 0f);
        GameObject npc = Instantiate(npcTemplate, spawnPosition, Quaternion.identity);
        npc.SetActive(true);

        if (npc.TryGetComponent<NPCController>(out var controller))
        {
            controller.SetDirection(new Vector2(direction, 0));
            controller.AlignBottomToY(spawnY);
        }
    }


    void SpawnCustomer()
    {
        bool spawnFromLeft = Random.value > 0.5f;
        Camera cam = Camera.main;
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        float spawnX;
        if (spawnFromLeft)
        {
            spawnX = cam.transform.position.x - (camWidth / 2) - spawnMargin;
        }
        else
        {
            spawnX = cam.transform.position.x + (camWidth / 2) + spawnMargin;
        }

        // Use the spawn zone's y limits for vertical spawn.
        float spawnY = Random.Range(spawnZone.MinY, spawnZone.MaxY);
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);
        GameObject customer = Instantiate(customerTemplate, spawnPosition, Quaternion.identity);
        customer.SetActive(true);

        if (customer.TryGetComponent<CustomerController>(out var customerController))
        {
            // Set the target counter position.
            // You can modify this target as needed; here it is set to (0, spawnY).
            Vector2 counterPosition = new Vector2(0, spawnY);
            customerController.SetTargetCounterPosition(counterPosition);
        }
    }

}