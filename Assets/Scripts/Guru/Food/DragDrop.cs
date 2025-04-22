using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private static DraggableObject currentlyDragging = null; // Prevents multiple objects from being dragged at the same time

    private bool isDragging = false;
    private Vector3 offset;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        Debug.Log($"[DraggableObject] {gameObject.name} initialized.");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            TryPickUpObject();
        }

        if (isDragging)
        {
            Vector3 newPosition = GetMouseWorldPosition() + offset;
            transform.position = newPosition; // Follow the mouse smoothly
        }

        if (Input.GetMouseButtonUp(0)) // Release mouse button
        {
            if (isDragging)
            {
                isDragging = false;
                currentlyDragging = null; // Allow another object to be picked up
                Debug.Log($"[DraggableObject] {gameObject.name} dropped at {transform.position}");
            }
        }
    }

    private void TryPickUpObject()
    {
        if (currentlyDragging != null) return; // Prevents multiple objects from being dragged

        RaycastHit2D[] hits = Physics2D.RaycastAll(GetMouseWorldPosition(), Vector2.zero);

        if (hits.Length > 0)
        {
            DraggableObject topObject = null;
            int highestLayerPriority = int.MinValue;

            foreach (RaycastHit2D hit in hits)
            {
                DraggableObject obj = hit.collider.GetComponentInParent<DraggableObject>(); // Get Parent Object if Available
                if (obj != null)
                {
                    // PRIORITIZE PARENT OBJECT FIRST
                    DraggableObject parentObj = obj.transform.parent != null ? obj.transform.parent.GetComponent<DraggableObject>() : obj;

                    BoxCollider2D col = obj.GetComponent<BoxCollider2D>();
                    int layerPriority = col ? col.layerOverridePriority : 0;

                    if (topObject == null || layerPriority > highestLayerPriority)
                    {
                        topObject = parentObj; // Select the Parent Object for dragging
                        highestLayerPriority = layerPriority;
                    }
                }
            }

            if (topObject != null)
            {
                currentlyDragging = topObject;
                topObject.PickUp();
            }
        }
    }


    private void PickUp()
    {
        isDragging = true;
        offset = transform.position - GetMouseWorldPosition();
        Debug.Log($"[DraggableObject] {gameObject.name} picked up.");
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -mainCamera.transform.position.z; // Adjust depth for 2D
        return mainCamera.ScreenToWorldPoint(mousePos);
    }
}
