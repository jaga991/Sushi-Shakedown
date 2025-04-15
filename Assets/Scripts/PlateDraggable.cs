using UnityEngine;

public class PlateDraggable : DraggableObject
{
    //A SO List of valid items to put onto it
    [SerializeField] private PlateIngredientSO[] plateIngredientSOArray;
    [SerializeField] private string[] currentIngredientTypeArray;
    //An array to track items currently on it (bluff, no actual gameobject, use sprite to simulate)


    //rice, fish, condiment (only one of each type, if already available, cannot handle this)
    //current max recipe types [rice, fish, condiments]
    public bool TryHandleIngredient(DraggableObject draggableObject)
    {
        Debug.Log($"Try handle object{draggableObject}");
        //check if its valid item in plateIngredientSOArray
        //then check if plate can handle it, check currentIngredientTypeArray and ensure no matches (means theres space for that ingredient)

        return true;
    }

}
