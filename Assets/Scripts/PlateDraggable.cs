using System.Collections.Generic;
using UnityEngine;

public class PlateDraggable : DraggableObject
{
    //A SO List of valid items to put onto it
    [SerializeField] private PlateIngredientSO[] plateIngredientSOArray;
    [SerializeField] private List<string> currentIngredientTypes = new List<string>(); // tracks types like "rice", "fish"
    [SerializeField] private List<DraggableObjectSO> currentIngredients = new List<DraggableObjectSO>(); // actual ingredients

    //An array to track items currently on it (bluff, no actual gameobject, use sprite to simulate)


    //rice, fish, condiment (only one of each type, if already available, cannot handle this)
    //current max recipe types [rice, fish, condiments]
    public bool TryHandleIngredient(DraggableObject draggableObject)
    {
        Debug.Log($"Try handle object{draggableObject}");
        //check if its valid item in plateIngredientSOArray
        Debug.Log($"[PlateDraggable] Try handle object {draggableObject.name}");

        DraggableObjectSO incomingSO = draggableObject.GetDraggableObjectSO();
        PlateIngredientSO ingredientInfo = GetPlateIngredientSO(incomingSO);
        if (ingredientInfo == null)
        {
            Debug.LogWarning("Ingredient not valid for this plate.");
            return false;
        }
        // Check if the type is already on the plate
        string ingredientType = ingredientInfo.ingredientType;
        if (currentIngredientTypes.Contains(ingredientType))
        {
            Debug.LogWarning($"Plate already contains a {ingredientType}.");
            return false;
        }

        // Add to plate
        currentIngredientTypes.Add(ingredientType);
        currentIngredients.Add(incomingSO);

        Debug.Log($"Added {ingredientType} ({incomingSO.name}) to plate.");

        // You can trigger a UI update or visual stacking here
        return true;
    }
    private PlateIngredientSO GetPlateIngredientSO(DraggableObjectSO inputSO)
    {
        foreach (var plateIngredientSO in plateIngredientSOArray)
        {
            if (plateIngredientSO.draggableObjectSO == inputSO)
                return plateIngredientSO;
        }
        return null;
    }
    public List<DraggableObjectSO> GetCurrentIngredients()
    {
        return new List<DraggableObjectSO>(currentIngredients);
    }
    public void ClearPlate()
    {
        currentIngredientTypes.Clear();
        currentIngredients.Clear();
        Debug.Log("Plate cleared.");
    }

    public int GetIngredientCount()
    {
        return currentIngredients.Count;
    }
}
