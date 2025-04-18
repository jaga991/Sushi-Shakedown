using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private Camera mainCamera;

    public DraggableObject currentlyDragging = null;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // prevent duplicate singletons
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // optional: keep this GameManager across scenes
    }
    [SerializeField]
    private GameDataSO gameDataSO;
    void Start()
    {
        // get main camera for position calculation
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("MainCamera not found! Make sure your main camera is tagged 'MainCamera'.");
        } else
        {
            Debug.Log("MainCamera set in GameManager");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //always keep mouse position updated
        UpdateGameDataSOMousePosition();
        //if left mouse button down
        if (Input.GetMouseButtonDown(0))
        {
            HandleLeftMouseDown();
        }

        if(Input.GetMouseButtonUp(0))
        {
            HandleLeftMouseUp();
        }
    }

    private void HandleLeftMouseUp()
    {
        Debug.Log("HandleLeftMouseUp Triggered");

        //get the currentDragging Draggable Object from gamedataso
        if (currentlyDragging != null)
        {
            currentlyDragging.HandleRelease();
        }
    }

    private void HandleLeftMouseDown()
    {
        Debug.Log("HandleLeftMouseDown Triggered");
        //first execute a raycast hit
        RaycastHit2D[] hits = Physics2D.RaycastAll(gameDataSO.mousePosition, Vector2.zero, Mathf.Infinity, gameDataSO.interactableLayers);
        BaseContainer baseContainer = null;
        DraggableObject draggableObject = null;
        FoodSpawner foodSpawner = null;

        Debug.Log($"HandleLeftMouseDown: {hits.Length} hits");

        foreach (var hit in hits)
        {
            Debug.Log($"Hit object: {hit.collider.gameObject.name}, Layer: {hit.collider.gameObject.layer}");
        }
        foreach (RaycastHit2D hit in hits)
        {
            if (baseContainer == null && hit.collider.TryGetComponent(out BaseContainer bc))
                baseContainer = bc;

            if (draggableObject == null && hit.collider.TryGetComponent(out DraggableObject d))
                draggableObject = d;
            if (foodSpawner == null && hit.collider.TryGetComponent(out FoodSpawner fs))
                foodSpawner = fs;
        }


        if (foodSpawner != null)
        {
            foodSpawner.SpawnFoodAtCursor(gameDataSO.mousePosition);
        }

        if (baseContainer != null)
        {
            //container trigger pickup on its draggable object
            baseContainer.GetDraggableToCursor(gameDataSO.mousePosition);
        }
           
        else if (draggableObject != null)
            draggableObject.TryPickUpThis();
    }

    private Vector3 GetMousePosition()
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        return mouseWorldPos;
    }

    private void UpdateGameDataSOMousePosition()
    {
        gameDataSO.mousePosition = GetMousePosition();
    }
}
