using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public GameObject npcTemplate;
    // Use your scene object as a template. Make sure to disable it in the scene. 
    public float spawnInterval = 3.0f;

    public float spawnMargin = 1.0f;

    private float timer;

    public SpawnZoneData spawnZone;
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
    }
    void SpawnNPC()
    {
        bool spawnFromLeft = Random.value > 0.5f;
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
}