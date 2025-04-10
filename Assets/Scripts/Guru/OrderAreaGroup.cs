using System.Collections.Generic;
using UnityEngine;

public class OrderAreaGroup : MonoBehaviour
{
    public List<OrderArea> orderAreas;

    void Awake()
    {
        // Automatically get all OrderArea components from child objects.
        orderAreas = new List<OrderArea>(GetComponentsInChildren<OrderArea>());
    }

    // Sequential version that marks the area as occupied if needed.
    public OrderArea GetFreeOrderArea()
    {
        foreach (var area in orderAreas)
        {
            if (!area.isOccupied)
            {
                // Reservation should be handled by the caller.
                return area;
            }
        }
        return null; // No free area available.
    }

    public void ReleaseOrderArea(OrderArea area)
    {
        if (orderAreas.Contains(area))
        {
            area.isOccupied = false;
        }
    }

    void Start()
    {
        // Benchmarking to compare sequential vs parallel performance
        OrderArea area = GetFreeOrderArea();
        if (area != null)
        {
            Debug.Log($"Free Order Area found: {area.name}");
            Debug.Log("Located at coordinates: " + area.GetCoordinates());
        }
        else
        {
            Debug.Log("No free Order Area available.");
        }
    }
}
