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

    public float minScale = 0.7f;
    [Tooltip("Sprite scale at the closest spawn Y")]
    public float maxScale = 0.9f;

    public float Normal = 3f;
    public bool spawnCustomerDebug = false;

    public OrderAreaGroup orderAreaGroup;
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
        bool spawnFromLeft = Random.value > 0.5f;
        Camera cam = Camera.main;
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        float spawnX = spawnFromLeft
            ? cam.transform.position.x - (camWidth / 2) - spawnMargin
            : cam.transform.position.x + (camWidth / 2) + spawnMargin;

        // pick a Y within the zone
        float spawnY = Random.Range(spawnZone.MinY, spawnZone.MaxY);
        Vector3 spawnPosition = new(spawnX, spawnY, 0f);

        // instantiate
        GameObject npc = Instantiate(npcTemplate, spawnPosition, Quaternion.identity, transform);
        npc.SetActive(true);

        // 1) compute depth factor
        float t = Mathf.InverseLerp(spawnZone.MinY, spawnZone.MaxY, spawnY);

        // 2) interpolate scale (closest= maxScale, furthest= minScale)
        float scale = Mathf.Lerp(maxScale * Normal, minScale * Normal, t);
        npc.transform.localScale = new Vector3(scale, scale, 1f);

        // 3) set movement & align bottom
        if (npc.TryGetComponent<NPCController>(out var controller))
        {
            controller.SetDirection(new Vector2(spawnFromLeft ? 1f : -1f, 0f));
            controller.AlignBottomToY(spawnY);
        }
    }


    void SpawnCustomer()
    {

        OrderArea orderArea = orderAreaGroup.GetFreeOrderArea();
        if (orderArea == null)
        {
            Debug.Log("All order areas are occupied. Customer Not Spawned !!");
            return;
        }

        orderArea.UpdateState(true); // Mark the order area as occupied.


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
            customerController.SetOrderArea(orderArea); // Set the order area for the customer.
        }
    }

}