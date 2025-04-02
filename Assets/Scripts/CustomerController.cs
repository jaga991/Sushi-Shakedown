using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Kept for Text component on textBubble

public class CustomerController : MonoBehaviour
{
    public float speed = 3f;
    public Vector2 targetPosition; // Will be assigned from OrderArea
    public GameObject textBubble;
    public GameObject OrderBubble;

    public PatienceBar patienceBar;

    // Maximum patience value (e.g., 100)
    public int maxPatience = 100;
    private Text bubbleText;
    public OrderArea assignedOrderArea; // Exposed if you still want to check later
    private Coroutine progressRoutine;
    private bool hasArrived = false;

    // State flag for off-screen movement
    private bool isWalkingOffScreen = false;
    private Vector2 offScreenTarget;

    public void SetOrderArea(OrderArea area)
    {
        assignedOrderArea = area;
        targetPosition = area.GetCoordinates();
    }

    void Awake()
    {
        if (textBubble != null)
        {
            bubbleText = textBubble.GetComponentInChildren<Text>();
            if (bubbleText != null)
            {
                bubbleText.text = "..."; // Display initial three dots.
            }
        }

        if (OrderBubble != null)
        {
            OrderBubble.SetActive(false); // Hide the order bubble initially
        }
        else
        {
            Debug.Log("OrderBubble is not active.");
        }

        if (patienceBar != null)
        {
            patienceBar.SetMaxHealth(maxPatience);
        }
    }

    void Update()
    {
        // Move towards the assigned order area if not arrived.
        if (!hasArrived && !isWalkingOffScreen)
        {
            Vector2 currentPosition = transform.position;
            Vector2 direction = (targetPosition - currentPosition).normalized;
            transform.Translate(direction * speed * Time.deltaTime);

            if (Vector2.Distance(currentPosition, targetPosition) < 0.1f)
            {
                ArrivedAtCounter();
                hasArrived = true;
            }
        }
        // Handle off-screen movement.
        else if (isWalkingOffScreen)
        {
            Vector2 currentPosition = transform.position;
            Vector2 direction = (offScreenTarget - currentPosition).normalized;
            transform.Translate(direction * speed * Time.deltaTime);

            if (Vector2.Distance(currentPosition, offScreenTarget) < 0.1f)
            {
                Debug.Log("Customer has walked off screen.");
                isWalkingOffScreen = false;
                // Optionally, destroy the customer:
                Destroy(gameObject);
            }
        }
    }

    void ArrivedAtCounter()
    {
        if (assignedOrderArea != null)
        {
            Debug.Log($"Customer reached assigned OrderArea: {assignedOrderArea.gameObject.name}");
        }

        if (bubbleText != null)
        {
            bubbleText.text = "Order";
        }

        textBubble.SetActive(false);
        OrderBubble.SetActive(true);

        // Start the fake progress count (0 to 100) over 10 seconds.
        progressRoutine = StartCoroutine(CountTo100());
    }

    IEnumerator CountTo100()
    {
        // We'll decrease the patience value as progress increases.
        int patience = maxPatience;
        for (int i = 0; i <= 100; i++)
        {
            // Decrease patience gradually (for example, linearly)
            patience = maxPatience - (int)((maxPatience / 100f) * i);
            if (patienceBar != null)
            {
                patienceBar.SetHeath(patience);
            }

            yield return new WaitForSeconds(0.1f); // 0.1s per step; 100 steps = 10s total.
        }
        OrderFailed();
    }

    void OrderFailed()
    {
        Debug.Log("Times up! Order failed.");
        OrderBubble.SetActive(false);
        SetOffScreenTarget();
        isWalkingOffScreen = true;
        assignedOrderArea.UpdateState(false);
    }

    // Updated method to set the off-screen target based on viewport bounds.
    void SetOffScreenTarget()
    {
        Camera cam = Camera.main;
        Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);
        Vector3 targetViewportPos;

        // If the customer is in the left half, target a point off-screen to the left.
        if (viewportPos.x < 0.5f)
        {
            targetViewportPos = new Vector3(-0.1f, viewportPos.y, viewportPos.z);
        }
        // Otherwise, target a point off-screen to the right.
        else
        {
            targetViewportPos = new Vector3(1.1f, viewportPos.y, viewportPos.z);
        }

        // Convert the target viewport position back to world space.
        Vector3 worldTarget = cam.ViewportToWorldPoint(targetViewportPos);
        // Preserve current Z position.
        worldTarget.z = transform.position.z;
        offScreenTarget = worldTarget;
    }

    void OnMouseDown()
    {
        if (hasArrived)
        {
            OrderCompleted();
        }
    }

    void OrderCompleted()
    {
        Debug.Log("Order Completed!");

        if (progressRoutine != null)
        {
            StopCoroutine(progressRoutine);
            progressRoutine = null;
        }

        OrderBubble.SetActive(false);

        SetOffScreenTarget();

        isWalkingOffScreen = true;

        if (assignedOrderArea != null)
        {
            assignedOrderArea.UpdateState(false);
        }
    }
}
