using UnityEngine;

public class OrderBubble : MonoBehaviour
{
    public FoodManager foodManager;

    public int slots = 2;
    private int currentSlot = 0;
    public float slotSpacing = 1f; // Adjust this to control vertical distance

    private SpriteRenderer selfSpriteRenderer;

    void Awake()
    {
        selfSpriteRenderer = GetComponent<SpriteRenderer>();
        if (selfSpriteRenderer == null)
        {
            Debug.Log("OrderBubble: No SpriteRenderer found!");
        }
        if (foodManager == null)
        {
            Debug.Log("OrderBubble: No FoodManager found!");
        }

    }
    void Start()
    {

    }
    public void StartOrder()
    {
        Food food = foodManager.GetRandomFood();

        Vector3 spawnPosition = transform.position;
        spawnPosition.x -= (selfSpriteRenderer.bounds.size.x) * 1.5f / 10;
        float midSlot = (slots - 1) / 2.0f;
        float yOffset = slotSpacing * (midSlot - currentSlot);
        spawnPosition.y += yOffset;
        food.transform.position = spawnPosition;
        Debug.Log("OrderedFood is " + food.foodName);

    }

}
