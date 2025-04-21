using System.Collections.Generic;
using UnityEngine;

public class CupDraggable : DraggableObject
{
    [SerializeField] private CupIngredientSO[] cupIngredientSOArray;
    [SerializeField] private List<DraggableObjectSO> currentIngredients = new List<DraggableObjectSO>(); // actual ingredients

    [SerializeField] private SpriteRenderer bottomDrinkSprite;
    [SerializeField] private SpriteRenderer middleDrinkSprite;
    [SerializeField] private SpriteRenderer topDrinkSprite;

    [SerializeField] private List<SpriteRenderer> drinkSpriteArray;

    private void Start()
    {
        drinkSpriteArray.Add(bottomDrinkSprite);
        drinkSpriteArray.Add(middleDrinkSprite);
        drinkSpriteArray.Add(topDrinkSprite);
    }

    public bool TryHandleIngredient(DraggableObject draggableObject)
    {
        DraggableObjectSO incomingSO = draggableObject.GetDraggableObjectSO();
        CupIngredientSO ingredientInfo = GetCupIngredientSO(incomingSO);
        if (ingredientInfo == null)
        {
            Debug.LogWarning("Ingredient not valid for this plate.");
            return false;
        }

        //check the amount of drink ingredient already present, add drink if length < 3
        string ingredientType = ingredientInfo.ingredientType;

        if (currentIngredients.Count < 3)
        {
            currentIngredients.Add(incomingSO);
            EventManager.Instance.Trigger<object>("AddCupIngredientAudio", this);
            Debug.Log($"Added {ingredientInfo.ingredientType} ({incomingSO.name}) to cup.");
            UpdateDrinkSprites();
            return true;
        }

        Debug.Log("Cup already has maximum ingredients.");
        return false;
    }

    private void UpdateDrinkSprites()
    {
        for (int i = 0; i < drinkSpriteArray.Count; i++)
        {
            if (i < currentIngredients.Count)
            {
                drinkSpriteArray[i].sprite = currentIngredients[i].sprite;
                drinkSpriteArray[i].color = Color.white; // Reset in case it was hidden
            }
            else
            {
                drinkSpriteArray[i].sprite = null;
                drinkSpriteArray[i].color = new Color(1, 1, 1, 0); // Hide sprite
            }
        }
    }
    private CupIngredientSO GetCupIngredientSO(DraggableObjectSO inputSO)
    {
        foreach (var cupIngredientSO in cupIngredientSOArray)
        {
            if (cupIngredientSO.draggableObjectSO == inputSO)
                return cupIngredientSO;
        }
        return null;
    }

    public int GetIngredientCount()
    {
        return currentIngredients.Count;
    }

    public List<DraggableObjectSO> GetCurrentIngredients()
    {
        return new List<DraggableObjectSO>(currentIngredients);
    }
}
