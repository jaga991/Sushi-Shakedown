using UnityEngine;

public class AssemblerContainer : BaseContainer
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    private void Update()
    {

        // On mouse release, check if draggable is inside and was just dropped
        if (GetHoveringDraggableObjectTracking() != null) //
        {
            DraggableObject trackingHoveringDraggableObject = GetHoveringDraggableObjectTracking();
            if (!trackingHoveringDraggableObject.IsBeingDragged())
            {
                Debug.Log($"{trackingHoveringDraggableObject.name} released in {gameObject.name}");

                if (GetOwnedDraggable() == null) //if container does not contain draggables
                {
                    if (trackingHoveringDraggableObject.GetComponent<IngredientDraggable>() != null)
                    {
                        // It is an IngredientDraggable
                        Debug.Log($"{trackingHoveringDraggableObject.name} is a valid IngredientDraggable");
                        
                        //set draggable object parent container to this
                        //set ownedDraggable to this draggableObject
                        trackingHoveringDraggableObject.SetParentContainer(this); 
                        //setowned draggable
                        SetOwnedDraggable(trackingHoveringDraggableObject);
                        ClearHoveringDraggableObjectTracking();   
                    }
                } else //container contains draggables
                {
                    //check if its a plate or cup
                    //if it is, trigger function in plate or cup that verifies if possble to add
                    trackingHoveringDraggableObject.ReturnToParentContainer();
                }
                //!TODO check if assembler owns any draggables
                //if empty, check if draggable is valid for assembler

                //if not empty, check if its a plate or cup
                //if not, means area is occupied, return the currentlydragging to parent container

            }
        }
    }
}
