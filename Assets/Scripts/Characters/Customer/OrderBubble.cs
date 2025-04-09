using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderBubble : MonoBehaviour
{
    public FoodManager foodManager;
    private readonly int maxSlots = 3;
    public float slotSpacing = 0f; // Adjust this to control vertical distance
    public float deliveryAnimationDuration = 0.2f;
    public CustomerController customerController;
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
        if (customerController == null)
        {
            // try auto‑find on parent
            customerController = GetComponentInParent<CustomerController>();
        }
        if (customerController == null)
        {
            Debug.LogWarning("OrderBubble: No CustomerController assigned or found in parents!");
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

            // Debug.Log("OrderedFood is " + food.foodName);
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        if (other.TryGetComponent<FoodDraggable>(out var foodDraggable))
        {
            Debug.Log($"Food draggable entered order bubble: {foodDraggable.foodName}");
            ProcessFoodDelivery(foodDraggable);
        }
    }

    /// <summary>
    /// Checks for a matching ordered food by name, removes it from the bubble,
    /// destroys its icon, repositions the rest, and optionally destroys the delivered object.
    /// </summary>
    private void ProcessFoodDelivery(FoodDraggable delivered)
    {
        string name = delivered.foodName;
        bool isFinal = (orderedFoods.Count == 1);
        // Debug.Log("Checking for matching order: " + name);

        for (int i = 0; i < orderedFoods.Count; i++)
        {
            if (orderedFoods[i].foodName == name)
            {

                Food matchedFood = orderedFoods[i];
                GameObject iconObj = matchedFood.gameObject;
                GameObject foodObj = delivered.gameObject;
                orderedFoods.RemoveAt(i);

                Debug.Log($"OrderBubble: Fulfilled and removed order '{name}'");
                StartCoroutine(AnimateDeliveryAndCleanup(foodObj, iconObj));


                customerController?.OnCorrectDelivery();


                if (isFinal)
                    customerController?.OnAllOrdersFulfilled();
                return; // only one match per delivery
            }
        }

        Debug.Log($"OrderBubble: No matching order found for '{name}'");
        customerController?.OnWrongDelivery(name);
    }


    private IEnumerator AnimateDeliveryAndCleanup(GameObject deliveredObj, GameObject iconObj)
    {
        Vector3 startPos = deliveredObj.transform.position;
        Vector3 endPos = iconObj.transform.position;
        float elapsed = 0f;

        // Optional: bring the delivered sprite to front
        var sr = deliveredObj.GetComponent<SpriteRenderer>();
        if (sr != null) sr.sortingOrder = selfSpriteRenderer.sortingOrder + 1;

        while (elapsed < deliveryAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / deliveryAnimationDuration);
            deliveredObj.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        // ensure exact alignment
        deliveredObj.transform.position = endPos;

        // cleanup
        Destroy(iconObj);
        Destroy(deliveredObj);
    }


}
