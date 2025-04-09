using UnityEngine;

public class BaseContainer : MonoBehaviour
{
    //base container defined as something that can store draggables as a child (not including plate/cups)
    //assembler will inherit base container, then add some checking functionality for foodBase (plate, cup) objects
    //cutting board will inherit base container, add the cutting functionality
    //stove will inherit base container, add cooking functionllity
    //
    [SerializeField] private DraggableObject ownedDraggable;
    void Update()
    {
        //if (Input.GetMouseButtonDown(0)) // Left-click
        //{
        //    Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    mouseWorldPos.z = 0f;

        //    RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        //    if (hit.collider != null)
        //    {
        //        Debug.Log($"[Raycast] Hit object: {hit.collider.name}");

        //        if (hit.collider.gameObject == gameObject)
        //        {
        //            DraggableObject childDraggable = GetComponentInChildren<DraggableObject>();
        //            if (childDraggable != null)
        //            {
        //                Debug.Log($"[BaseContainer] Picked up child: {childDraggable.gameObject.name}");
        //                childDraggable.TryPickUpThis();
        //            }
        //        }
        //    }
        //    else
        //    {
        //        Debug.Log("[Raycast] Hit nothing.");
        //    }
        //}
    }

    public void GetDraggableToCursor(Vector3 mousePosition)
    {
        //first check if there is draggables within itself
        if(ownedDraggable)
        {
            ownedDraggable.TryPickUpThis();
        }
        else
        {
            Debug.Log($"No Owned Draggables in {gameObject.name}");
        }

        //if no draggables, ignore
        //if draggables, trigger draggable trytopickup
    }

}
