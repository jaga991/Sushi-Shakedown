using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Kept for Text component on textBubble

public class CustomerController : MonoBehaviour
{
    public float speed = 3f;
    public Vector2 targetPosition; // Will be assigned from OrderArea
    public GameObject textBubble;
    public GameObject OrderBubble;

    public CustomerData CustomerData;

    public PatienceBar patienceBar;

    public AudioClip orderCompletedSound;
    public AudioClip orderFailedSound;
    public AudioSource audioSource;

    public OrderBubble orderBubble;


    private SpriteRenderer spriteRenderer;
    public Sprite happySprite;
    public Sprite frustratedSprite;
    public Sprite angrySprite;

    // Maximum patience value (e.g., 100)
    public int maxPatience = 100;
    private Text bubbleText;
    public OrderArea assignedOrderArea; // Exposed if you still want to check later
    private Coroutine progressRoutine;

    public int patienceBoostOnCorrect = 10;

    // internal tracker
    private int currentPatience;
    private bool hasArrived = false;

    // State flag for off-screen movement
    private bool isWalkingOffScreen = false;
    private Vector2 offScreenTarget;

    /// <summary>
    /// Called by OrderBubble when the player delivers the right item.
    /// </summary>
    public void OnCorrectDelivery()
    {
        Debug.Log("CustomerController: Correct delivery! , boosted patience.");

        currentPatience = Mathf.Min(maxPatience, currentPatience + patienceBoostOnCorrect);

        // reflect immediately in the UI
        patienceBar.SetHealth(currentPatience);
        // Optionally, you can also play a sound or show feedback here.

    }

    /// <summary>
    /// Called by OrderBubble when the player delivers the wrong item.
    /// Shows feedback, deducts a bit of patience, then reverts.
    /// </summary>
    public void OnWrongDelivery(string foodName)
    {
        Debug.Log($"CustomerController: Wrong delivery of {foodName}!");
        OrderFailed(2);
    }

    public void SetOrderArea(OrderArea area)
    {
        assignedOrderArea = area;
        targetPosition = area.GetCoordinates();
    }

    void Awake()
    {

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.Log("NPCController: No SpriteRenderer found!");
        }
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

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
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
        // if (assignedOrderArea != null)
        // {
        //     Debug.Log($"Customer reached assigned OrderArea: {assignedOrderArea.gameObject.name}");
        // }

        textBubble.SetActive(false);
        OrderBubble.SetActive(true);

        // orderBubble.StartOrder(Random.Range(1, 4));
        orderBubble.StartOrder(1);
        currentPatience = maxPatience;

        // Start the fake progress count (0 to 100) over 10 seconds.\
        if (progressRoutine != null) StopCoroutine(progressRoutine);
        progressRoutine = StartCoroutine(PatienceCountdown());
    }

    private IEnumerator PatienceCountdown()
    {
        // keep ticking down until zero
        while (currentPatience > 0)
        {
            // update the bar
            patienceBar.SetHealth(currentPatience);

            // wait
            yield return new WaitForSeconds(0.1f);

            // decrement
            currentPatience--;
        }

        // out of patience!
        OrderFailed(1);
    }



    public void OnAllOrdersFulfilled()
    {
        // 1) stop patience timer
        if (progressRoutine != null)
            StopCoroutine(progressRoutine);

        OrderCompleted();
    }


    void OrderFailed(int reason)
    {
        if (reason == 1)
        {
            // customer ran out of patience 
            CustomerData.DeductScore(1);
            Debug.Log("Times up! Order failed.");
        }
        else if (reason == 2)
        {
            CustomerData.DeductScore(5);
            CustomerData.DeductScore(5);
            Debug.Log("Customer Received Wrong Order !!");
            // on wrong delivery 
        }
        spriteRenderer.sprite = angrySprite;
        PlayOrderFailed();

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
        // Determine customer mood based on patience level
        int patiencePercent = patienceBar.GetHealth();

        spriteRenderer.sprite = patiencePercent switch
        {
            > 75 => happySprite,
            < 35 => frustratedSprite,
            _ => spriteRenderer.sprite
        };

        PlayOrderSuccess();

        CustomerData.Increment();

        Debug.Log("Order Completed!");
        // Calculate score based on patience percentage (1-10)
        int score = Mathf.Clamp(1 + Mathf.FloorToInt(patiencePercent * 9f / 100f), 1, 10);
        // ScoreCounter.instance.AddScore(score);
        CustomerData.AddScore(score);
        Debug.Log("Added Score is " + score);


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

    public void PlayOrderSuccess()
    {
        if (orderCompletedSound != null)
        {
            audioSource.PlayOneShot(orderCompletedSound);
        }
    }

    public void PlayOrderFailed()
    {
        if (orderFailedSound != null)
        {
            audioSource.PlayOneShot(orderFailedSound);
        }
    }
}
