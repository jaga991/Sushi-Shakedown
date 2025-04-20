using System;
using UnityEngine;
using UnityEngine.UI;

public class GrillingBoard : BaseContainer
{
    [SerializeField] private GrillingRecipeSO[] grillingRecipeSOArray;

    [Header("Progress UI")]
    [SerializeField] private Image progressFillImage;
    [SerializeField] private GameObject grillingProgressUI;

    private float grillingProgress = 0f;
    private bool isGrilling = false;
    private GrillingRecipeSO activeRecipe;

    public event EventHandler OnAnyObjectGrilled;
    public event EventHandler<IHasProgress.OnProgressChangeEventArgs> OnProgressChanged;

    private void Update()
    {
        HandleHoverAndDrop();
        HandleGrillingProgress();
    }

    private void HandleHoverAndDrop()
    {
        var hovering = GetHoveringDraggableObjectTracking();

        if (hovering != null && !hovering.IsBeingDragged())
        {
            if (GetOwnedDraggable() == null && HasRecipeWithInput(hovering.GetDraggableObjectSO()))
            {
                Debug.Log($"{hovering.name} is a valid GrillingObject");

                // Accept the draggable object
                hovering.SetParentContainer(this);
                SetOwnedDraggable(hovering);
                ClearHoveringDraggableObjectTracking();

                // Start grilling
                activeRecipe = GetRecipeWithInput(hovering.GetDraggableObjectSO());
                grillingProgress = 0f;
                isGrilling = true;
                grillingProgressUI.SetActive(true);
                UpdateUIProgress(0f);
            }
            else
            {
                hovering.ReturnToParentContainer();
            }
        }
        else if (containerVisual != null)
        {
            containerVisual.color = defaultColor;
        }
    }

    private void HandleGrillingProgress()
    {
        if (!isGrilling || GetOwnedDraggable() == null || activeRecipe == null) return;

        grillingProgress += Time.deltaTime;
        float normalized = grillingProgress / activeRecipe.grillingProgressMax;
        UpdateUIProgress(normalized);

        EventManager.Instance.TriggerEvent("ObjectGrilled", this);
        OnAnyObjectGrilled?.Invoke(this, EventArgs.Empty);

        if (grillingProgress >= activeRecipe.grillingProgressMax)
        {
            CompleteGrillingCycle();
        }
    }

    private void CompleteGrillingCycle()
    {
        // Replace current ingredient with grilled output
        Destroy(GetOwnedDraggable().gameObject);
        ClearOwnedDraggable();

        var newObj = Instantiate(activeRecipe.outputIngredient.prefab);
        var grilled = newObj.GetComponent<DraggableObject>();

        SetOwnedDraggable(grilled);
        grilled.SetParentContainer(this);

        // Check if output has another recipe (e.g. grilled -> burnt)
        GrillingRecipeSO nextRecipe = GetRecipeWithInput(activeRecipe.outputIngredient);

        if (nextRecipe != null)
        {
            activeRecipe = nextRecipe;
            grillingProgress = 0f;
            isGrilling = true;
            grillingProgressUI.SetActive(true);
        }
        else
        {
            // No more recipes, stop grilling
            isGrilling = false;
            grillingProgress = 0f;
            grillingProgressUI.SetActive(false);
            activeRecipe = null;
        }
    }

    private void UpdateUIProgress(float normalized)
    {
        normalized = Mathf.Clamp01(normalized);
        progressFillImage.fillAmount = normalized;

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs
        {
            ProgressNormalized = normalized
        });
    }

    private GrillingRecipeSO GetRecipeWithInput(DraggableObjectSO input)
    {
        foreach (var recipe in grillingRecipeSOArray)
        {
            if (recipe.inputIngredient == input)
                return recipe;
        }
        return null;
    }

    private bool HasRecipeWithInput(DraggableObjectSO input)
    {
        return GetRecipeWithInput(input) != null;
    }

    public override void TryGetDraggableToCursor(Vector3 mousePosition)
    {
        if (GetOwnedDraggable())
        {
            GetOwnedDraggable().TryPickUpThis();
            SetHoveringDraggableObjectTracking(GetOwnedDraggable());
            ClearOwnedDraggable();

            // Reset grilling state
            isGrilling = false;
            grillingProgress = 0f;
            activeRecipe = null;
            grillingProgressUI.SetActive(false);
            UpdateUIProgress(0f);
        }
        else
        {
            Debug.Log($"No Owned Draggables in {gameObject.name}");
        }
    }
}
