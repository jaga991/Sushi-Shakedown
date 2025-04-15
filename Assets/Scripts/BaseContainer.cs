using UnityEngine;

public class BaseContainer : MonoBehaviour
{
    //base container defined as something that can store draggables as a child (not including plate/cups)
    //condiments and drinks ingredient container will inherit basecontainer directly
    //assembler will inherit base container, then add some checking functionality for foodBase (plate, cup) objects
    //cutting board will inherit base container, add the cutting functionality
    //stove will inherit base container, add cooking functionllity


    //commonality of all basecontainers
    //1.)track the draggableobject hovering in their colliders (draggableInZone)
    //2.)keep a record of what child draggables they own (ownedDraggable)
    [SerializeField]
    protected DraggableObject draggableInZone = null;
    protected DraggableObject ownedDraggable= null;

    private void OnTriggerEnter2D(Collider2D other) //if collision detected
    {
        if (other.GetComponent<DraggableObject>() != null) //check if other object is a draggableobject
        {
            Debug.Log($"[{gameObject.name}] {other.name} entered {gameObject.name}.");
            //if other is draggableObject, track the hovering draggableObject 
            draggableInZone = other.GetComponent<DraggableObject>();
        }
    }

    private void OnTriggerExit2D(Collider2D other) //if collider exits
    {
        if (draggableInZone != null && other.gameObject == draggableInZone.gameObject)
        {
            //if draggableInZone exist, reset to null to show no more draggable hovering in collider
            Debug.Log($"[{gameObject.name}] {other.name} left {gameObject.name}.");
            draggableInZone = null;
        }
    }
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
