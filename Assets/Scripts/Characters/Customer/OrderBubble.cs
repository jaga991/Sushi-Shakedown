using System.Collections.Generic;
using UnityEngine;

public class OrderBubble : MonoBehaviour
{
    public FoodManager foodManager;
    private readonly int maxSlots = 3;
    public float slotSpacing = 0f; // Adjust this to control vertical distance

    private SpriteRenderer selfSpriteRenderer;
    // List to store ordered food items.
    private List<Food> orderedFoods = new List<Food>();

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

    // New StartOrder method that accepts a number parameter.
    public void StartOrder(int numberOfOrders = 1)
    {
        // Determine how many orders can be spawned based on remaining slots.
        int availableSlots = maxSlots - orderedFoods.Count;
        int spawnCount = Mathf.Min(numberOfOrders, availableSlots);

        for (int i = 0; i < spawnCount; i++)
        {
            // Get a new Food instance from FoodManager.
            Food food = foodManager.GetRandomFood();
            // Make the food a child of OrderBubble.
            food.transform.SetParent(transform);

            // Determine slot index based on the current count.
            int slotIndex = orderedFoods.Count;
            Vector3 spawnPosition = CalculateOrderPosition(slotIndex);
            food.transform.position = spawnPosition;

            Debug.Log("OrderedFood is " + food.foodName);
            // Store the new order in the list.
            orderedFoods.Add(food);
        }
    }

    private Vector3 CalculateOrderPosition(int slotIndex)
    {
        Vector3 spawnPosition = transform.position;
        // Adjust the x position as before.
        spawnPosition.x -= selfSpriteRenderer.bounds.size.x * 1.5f / 10;

        // Get the bubble's height and compute the bottom Y coordinate.
        float bubbleHeight = selfSpriteRenderer.bounds.size.y;
        float bottomY = transform.position.y - bubbleHeight / 2;

        // Fixed percentages for 3 slots: 10%, 40%, and 70%.
        float[] percentages = new float[3] { 0.80f, 0.50f, 0.20f };
        spawnPosition.y = bottomY + percentages[slotIndex] * bubbleHeight;

        return spawnPosition;
    }
    // Optional: a method to retrieve the current list of orders.
    public List<Food> GetOrders()
    {
        return orderedFoods;
    }
}
