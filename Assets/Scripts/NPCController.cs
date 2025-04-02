using UnityEngine;

public class NPCController : MonoBehaviour
{
    public float speed = 3f;
    private Vector2 moveDirection = Vector2.right; // Default movement direction.
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        // Debug.Log($"[NPCController] {gameObject.name} initialized.");
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.Log("NPCController: No SpriteRenderer found!");
        }
    }

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
        // Flip sprite based on horizontal direction
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = moveDirection.x > 0;
        }
        else
        {
            Debug.Log("NPCController: No SpriteRenderer found, cannot flip sprite.");
        }
    }

    void Update()
    {
        // Move the NPC.
        transform.Translate(speed * Time.deltaTime * moveDirection);

        // Check if the NPC has moved off screen.
        Camera cam = Camera.main;
        Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);
        // If NPC goes far off-screen (x < -0.1 or > 1.1), destroy it.
        if (viewportPos.x < -0.1f || viewportPos.x > 1.1f)
        {
            Destroy(gameObject);
        }
    }

    public void AlignBottomToY(float spawnY)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            // Calculate the sprite's height in world units.
            float spriteHeight = sr.bounds.size.y;
            // The current bottom is at (transform.position.y - spriteHeight/2)
            float currentBottomY = transform.position.y - spriteHeight * 0.5f;
            // Calculate the offset needed so that current bottom aligns with spawnY.
            float offset = spawnY - currentBottomY;
            // Only modify the Y position
            transform.position = new Vector3(transform.position.x, transform.position.y + offset, transform.position.z);
        }
        else
        {
            Debug.Log("NPCController: No SpriteRenderer found, cannot adjust spawn Y.");
        }
    }

}