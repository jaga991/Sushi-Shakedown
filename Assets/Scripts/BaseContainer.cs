using UnityEngine;

public class BaseContainer : MonoBehaviour
{
    //base container defined as something that can store draggables as a child (not including plate/cups)
    //condiments and drinks ingredient container will inherit basecontainer directly
    //assembler will inherit base container, then add some checking functionality for foodBase (plate, cup) objects
    //cutting board will inherit base container, add the cutting functionality
    //stove will inherit base container, add cooking functionllity
    //
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
