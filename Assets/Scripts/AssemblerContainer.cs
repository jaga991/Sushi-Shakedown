using UnityEngine;

public class AssemblerContainer : BaseContainer
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    private void Update()
    {
        // On mouse release, check if draggable is inside and was just dropped
        if (draggableInZone != null) //
        {
            if (!draggableInZone.IsBeingDragged()) //
            {
                Debug.Log($"{draggableInZone.name} released in {gameObject.name}");

                if (ownedDraggableble == null) //if container does not contain draggables
                {
                    if (draggableInZone.GetComponent<IngredientDraggable>() != null)
                    {
                        // It is an IngredientDraggable
                        Debug.Log($"{draggableInZone.name} is a valid IngredientDraggable");
                        draggableInZone.SetParentContainer(this);
                        //setowned draggable
                        ownedDraggableble = draggableInZone;
                        
                    }
                }
                //!TODO check if assembler owns any draggables
                //if empty, check if draggable is valid for assembler

                //if not empty, check if its a plate or cup
                //if it is, trigger function in plate or cup that vverifies if possble to add
                //if not, means area is occupied, return the currentlydragging to parent container
                
            }
        }
    }
}
