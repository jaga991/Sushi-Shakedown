using UnityEngine;

public class BaseContainer : MonoBehaviour
{
    //base container defined as something that can store draggables as a child (not including plate/cups)
    //condiments and drinks ingredient container will inherit basecontainer directly
    //assembler will inherit base container, then add some checking functionality for foodBase (plate, cup) objects
    //cutting board will inherit base container, add the cutting functionality
    //stove will inherit base container, add cooking functionllity


    private DraggableObject draggableInZone = null;
    private DraggableObject ownedDraggableble = null;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<DraggableObject>() != null) //might need to change to prevent condiments and drink source from being trashed
        {
            Debug.Log($"[{gameObject.name}] {other.name} entered {gameObject.name}.");
            draggableInZone = other.GetComponent<DraggableObject>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (draggableInZone != null && other.gameObject == draggableInZone.gameObject)
        {
            Debug.Log($"[{gameObject.name}] {other.name} left {gameObject.name}.");
            draggableInZone = null;
        }
    }
    [SerializeField] private DraggableObject ownedDraggable;
    public void TryGetDraggableToCursor(Vector3 mousePosition)
    {
        //first check if there is draggables within itself
        //if no draggables, ignore
        //if draggables, trigger draggable trytopickup
        if (ownedDraggable)
        {
            ownedDraggable.TryPickUpThis();
        }
        else
        {
            Debug.Log($"No Owned Draggables in {gameObject.name}");
        }

    }


}
