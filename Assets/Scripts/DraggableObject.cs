using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private bool isColliding = false;

    [SerializeField] private BaseContainer parentContainer;

    [SerializeField] private GameDataSO gameDataSO;

    private void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //check if other GameObject is BaseContainer
        BaseContainer container = other.GetComponent<BaseContainer>();
        TrashBin trashBin = other.GetComponent<TrashBin>();
        if (container != null)
        {
            //set isColliding to true
            isColliding = true;
        }
        else if (trashBin != null)
        {
            Debug.Log($"[DraggableObject] {gameObject.name} entered trash zone.");
            isColliding = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isColliding = false;
    }


    private void UpdatePosition()
    {
        Vector3 newPosition = gameDataSO.mousePosition + offset;
        transform.position = newPosition; // Follow the mouse smoothly
    }
    private void Update()
    {
        if (isDragging) 
        {
            //continuous position updating
            UpdatePosition();

        }
    }


    public void TryPickUpThis()
    {
        offset = transform.position - gameDataSO.mousePosition;
        isDragging = true;

        GameManager.Instance.currentlyDragging = this;

        Debug.Log($"[DraggableObject] {gameObject.name} picked up.");
    }

    public bool IsBeingDragged()
    {
        return isDragging;
    }

    public void ReturnToParentContainer()
    {
        if (parentContainer != null)
        {
            transform.position = parentContainer.transform.position;
            Debug.Log($"[DraggableObject] {gameObject.name} returned to {parentContainer.name}");
            isDragging = false;
        }
        else
        {
            Debug.LogWarning($"[DraggableObject] {gameObject.name} has no parentContainer assigned!");
        }
    }

    public BaseContainer GetParentContainer()
    {
        return parentContainer;
    }

    public void SetParentContainer(BaseContainer container)
    {
        parentContainer = container;
    }

    public void HandleRelease()
    {
        Debug.Log($"[DraggableObject] {gameObject.name} released. isColliding = {isColliding}");

        if (!isColliding)
        {
            Debug.Log($"[DraggableObject] {gameObject.name} is not colliding with any valid container.");

            if (parentContainer)
            {
                Debug.Log($"[DraggableObject] Returning {gameObject.name} to its parent container: {parentContainer.name}");
                ReturnToParentContainer();
            }
            else
            {
                Debug.LogWarning($"[DraggableObject] {gameObject.name} has no parent container and was dropped in an invalid area. Destroying...");
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.Log($"[DraggableObject] {gameObject.name} was released while colliding with a valid container. set isDragging to False.");
            this.isDragging = false;
        }
    }


}
