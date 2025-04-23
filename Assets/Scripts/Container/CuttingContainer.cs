using System;
using UnityEngine;
using UnityEngine.UI;

public class CuttingContainer : BaseContainer   //cutting board will inherit base container, add the cutting functionality

{
    [SerializeField] private int cuttingProgress;
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;


    [SerializeField] private GameObject cuttingProgressUI;  // Still keep the GameObject reference



    private void Update()
    {
        // On mouse release, check if draggable is inside and was just dropped
        if (GetHoveringDraggableObjectTracking() != null) //
        {
            DraggableObject trackingHoveringDraggableObject = GetHoveringDraggableObjectTracking();
            TriggerBaseContainerSelectedVisualEvent(this);

            if (!trackingHoveringDraggableObject.IsBeingDragged())
            {
                Debug.Log($"{trackingHoveringDraggableObject.name} released in {gameObject.name}");

                if (GetOwnedDraggable() == null) //if container does not contain draggables
                {
                    if (trackingHoveringDraggableObject.GetComponent<DraggableObject>() != null) //TODO NEED TO UPDATE
                    {
                        Debug.Log($"{trackingHoveringDraggableObject.name} is a valid IngredientDraggable");

                        //set draggable object parent container to this
                        //set ownedDraggable to this draggableObject
                        //clear tracking of hovering object
                        trackingHoveringDraggableObject.SetParentContainer(this);
                        SetOwnedDraggable(trackingHoveringDraggableObject);
                        ClearHoveringDraggableObjectTracking();
                        //audio
                        EventManager.Instance.Trigger<object>("PlaceItemAudio", this);


                        //start cutting progress
                        cuttingProgress = 0;
                        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetOwnedDraggable().GetDraggableObjectSO());

                        // NEW: trigger UI event
                        EventManager.Instance.Trigger("updateProgressUI", new ProgressBarUpdateData(cuttingProgressUI, 0f));
                        EventManager.Instance.Trigger("showProgressUI", cuttingProgressUI);

                    }
                }
                else //container contains draggables
                {
                    trackingHoveringDraggableObject.ReturnToParentContainer();
                }
            }
        }
        else
        {
            TriggerBaseContainerDeselectedSelectedVisualEvent(this);
            //if (containerVisual != null)
            //{
            //    //TODO UI
            //    containerVisual.color = defaultColor;
            //}
        }
    }

    public override void TryGetDraggableToCursor(Vector3 mousePosition)
    {
        //first check if there is draggables within itself
        //if no draggables, ignore
        //if draggables, trigger draggable trytopickup
        if (GetOwnedDraggable())
        {
            GetOwnedDraggable().TryPickUpThis();
            SetHoveringDraggableObjectTracking(GetOwnedDraggable());
            ClearOwnedDraggable();

            //then reset any cutting status
            cuttingProgress = 0;
            //UI reset
            EventManager.Instance.Trigger("hideProgressUI", cuttingProgressUI);

        }
        else
        {
            Debug.Log($"No Owned Draggables in {gameObject.name}");
        }
    }

    public void HandleRightClick()
    {
        Debug.Log("Cutting board triggered");
        //check cutting board has ingredient and valid to cut
        if (GetOwnedDraggable() && HasRecipeWithInput(GetOwnedDraggable().GetDraggableObjectSO()))
        {
            cuttingProgress += 1;
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetOwnedDraggable().GetDraggableObjectSO());
            Debug.Log($"cutting progress: {cuttingProgress}");
            //audio
            EventManager.Instance.Trigger<object>("ObjectCutAudio", this);

            //Update Progress UI
            float normalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax;
            EventManager.Instance.Trigger("updateProgressUI", new ProgressBarUpdateData(cuttingProgressUI, normalized));
            if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
            {
                DraggableObjectSO outputDraggableObjectSO = GetOutputForInput(GetOwnedDraggable().GetDraggableObjectSO());
                Destroy(GetOwnedDraggable().gameObject);
                ClearOwnedDraggable();
                Transform newFood = Instantiate(outputDraggableObjectSO.prefab);
                GameObject outputFood = newFood.gameObject;
                DraggableObject draggable = outputFood.GetComponent<DraggableObject>();
                Debug.Log("NEW FOOD SPAWNED");


                SetOwnedDraggable(draggable);
                draggable.SetParentContainer(this);
                ClearHoveringDraggableObjectTracking();

                //UI progress bar reset TODO UI
                EventManager.Instance.Trigger("updateProgressUI", new ProgressBarUpdateData(cuttingProgressUI, 0f));
                EventManager.Instance.Trigger("hideProgressUI", cuttingProgressUI);
            }
        }
    }

    private CuttingRecipeSO GetCuttingRecipeSOWithInput(DraggableObjectSO inputDraggableObjectSO)
    {
        //check if inputFoodObjectSO exist in any of the CuttingRecipeSOArr inputs

        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if (cuttingRecipeSO.inputIngredient == inputDraggableObjectSO)
            {
                return cuttingRecipeSO;
            }
        }
        return null;
    }

    private bool HasRecipeWithInput(DraggableObjectSO inputDraggableObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputDraggableObjectSO);
        return cuttingRecipeSO != null;
    }

    private DraggableObjectSO GetOutputForInput(DraggableObjectSO inputDraggableObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputDraggableObjectSO);
        if (cuttingRecipeSO != null)
        {
            return cuttingRecipeSO.outputIngredient;
        }
        else
        {
            return null;
        }
    }
}
