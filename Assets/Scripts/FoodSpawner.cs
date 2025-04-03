using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodPrefab; // Assign your food prefab in Inspector
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            Vector3 mouseWorldPos = GetMouseWorldPosition();

            // Check if mouse is over this spawner's collider
            Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);
            if (hit != null && hit.gameObject == gameObject)
            {
                SpawnFoodAtCursor(mouseWorldPos);
            }
        }
    }

    private void SpawnFoodAtCursor(Vector3 position)
    {
        GameObject newFood = Instantiate(foodPrefab, position, Quaternion.identity);

        // Optionally auto-pickup and drag immediately
        DraggableObject draggable = newFood.GetComponent<DraggableObject>();
        if (draggable != null)
        {
            draggable.ForcePickUp();
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -mainCamera.transform.position.z;
        return mainCamera.ScreenToWorldPoint(mousePos);
    }
}
