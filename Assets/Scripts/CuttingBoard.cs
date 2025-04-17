using System;
using UnityEngine;
using UnityEngine.UIElements;

public class CuttingBoard : BaseContainer   //cutting board will inherit base container, add the cutting functionality

{
    [SerializeField] private CuttingRecipeSO cuttingRecipeSO;

    [SerializeField] private int cuttingProgress;
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    public event EventHandler OnAnyObjectCut;

    //create a event for onprogresschange
    public event EventHandler<IHasProgress.OnProgressChangeEventArgs> OnProgressChanged;
    public class OnProgressChangeEventArgs : EventArgs
    {
        public float ProgressNormalized;
    }
    private void Update()
    {

        // On mouse release, check if draggable is inside and was just dropped
        if (GetHoveringDraggableObjectTracking() != null) //
        {
            DraggableObject trackingHoveringDraggableObject = GetHoveringDraggableObjectTracking();
            if (containerVisual != null)
            {
                Color faded = containerVisual.color;
                faded.a = 0.5f; // semi-transparent
                containerVisual.color = faded;
            }
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

                        //start cutting progress
                        cuttingProgress = 0;
                        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetOwnedDraggable().GetDraggableObjectSO());
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs
                        {
                            ProgressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
                        });
                    }
                }
                else //container contains draggables
                {
                    trackingHoveringDraggableObject.ReturnToParentContainer();
                }
            }
        } else
        {
            if (containerVisual != null)
            {
                containerVisual.color = defaultColor;
            }
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
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs
            {
                ProgressNormalized = (float)0
            });
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
        if(GetOwnedDraggable() && HasRecipeWithInput(GetOwnedDraggable().GetDraggableObjectSO()))
        {
            cuttingProgress++;
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetOwnedDraggable().GetDraggableObjectSO());
            Debug.Log($"cutting progress: {cuttingProgress}");
            EventManager.Instance.TriggerEvent("ObjectCut", this);
            OnAnyObjectCut?.Invoke(this, EventArgs.Empty);
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
