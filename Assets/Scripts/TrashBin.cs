using UnityEngine;

public class TrashBin : MonoBehaviour
{
    private DraggableObject draggableInZone = null;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<DraggableObject>() != null) //might need to change to prevent condiments and drink source from being trashed
        {
            draggableInZone = other.GetComponent<DraggableObject>();
            Debug.Log($"[TrashBin] {other.name} entered trash zone.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (draggableInZone != null && other.gameObject == draggableInZone.gameObject)
        {
            Debug.Log($"[TrashBin] {other.name} exited trash zone.");
            draggableInZone = null;
        }
    }

    private void Update()
    {
        // On mouse release, check if draggable is inside and was just dropped
        if (draggableInZone != null) //Condition: if a draggable in trashbin collider
        {
            if (!draggableInZone.IsBeingDragged()) //
            {
                Debug.Log($"[TrashBin] Destroying {draggableInZone.name}");
                //!TODO, need to do additional checker to make sure dont destroy condiments and drinks ingredients
                Destroy(draggableInZone.gameObject);
                draggableInZone = null;
            }
        }
    }
}
